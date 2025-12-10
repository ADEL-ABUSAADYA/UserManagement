using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

var builder = DistributedApplication.CreateBuilder(args);

// Map the Docker Compose environment into AppHost configuration.
// Use colon-separated keys for IConfiguration and also set OS env vars (double-underscore form)
// so child processes/containers that read env-vars can find them.
var envConfig = new Dictionary<string, string>
{
    // ASP.NET Core runtime
    ["ASPNETCORE_ENVIRONMENT"] = "Development",
    ["ASPNETCORE_URLS"] = "http://+:8080",

    // Connection strings (IConfiguration keys use ':' for nesting)
    ["ConnectionStrings:DefaultConnection"] = "Server=sqlserver,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",
    ["ConnectionStrings:CAP-SQLConnection"] = "Server=sqlserver,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",
    ["ConnectionStrings:HangfireConnection"] = "Server=sqlserver,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",

    // App secrets / keys
    ["JWTSettings:SecretKey"] = "qwertyuiop[]asd124tgqrweee15364qfwretryutiyuryrterdvfn",
    ["OTPSettings:SecretKey"] = "qwertyuiop[UsersOtp]asd124tgqrweee15364qfwretryutiyuryrterdvfn",
    ["EmailPasswords"] = "vxfdhstkqegcfnei - csxyokaorioanxek - 11223344Upskilling",

    // CAP / RabbitMQ settings
    ["Cap:RabbitMQ:HostName"] = "rabbitmq",
    ["Cap:RabbitMQ:Port"] = "5672",
    ["Cap:RabbitMQ:UserName"] = "user",
    ["Cap:RabbitMQ:Password"] = "password",
    ["Cap:RabbitMQ:ExchangeName"] = "cap.default.router",
    ["Cap:DefaultGroupName"] = "Cap.queue",
    ["Cap:FailedRetryCount"] = "5",

    // Additional settings
    ["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true"
};

// Add to the DistributedApplication configuration so projects receive these values via IConfiguration
builder.Configuration.AddInMemoryCollection(envConfig);

// Also set OS environment variables — some container runtimes or child processes expect env-style keys.
// We set both "Key" and "Key" with double-underscore replacing ':' for compatibility with Docker/Compose style.
foreach (var kv in envConfig)
{
    // Set the colon form (usable by IConfiguration when reading from env)
    Environment.SetEnvironmentVariable(kv.Key, kv.Value, EnvironmentVariableTarget.Process);

    // Set the Docker-compose style double-underscore form as well (useful for external tools)
    var dockerStyleKey = kv.Key.Replace(":", "__");
    Environment.SetEnvironmentVariable(dockerStyleKey, kv.Value, EnvironmentVariableTarget.Process);
}

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.UserManagement_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

// If your AppHost supports declaring container images for infrastructure services (sqlserver/rabbitmq),
// prefer using built-in helpers (e.g. builder.AddImage/AddContainer) to start them and make apiService WaitFor them.
// If not available, ensure sqlserver/rabbitmq are started externally and reachable by the above connection strings.

builder.AddProject<Projects.UserManagement_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
