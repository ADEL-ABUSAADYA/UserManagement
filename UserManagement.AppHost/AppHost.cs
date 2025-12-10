using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

var builder = DistributedApplication.CreateBuilder(args);

// ================================
// 1. Environment & Configuration
// ================================
var envConfig = new Dictionary<string, string>
{
    // ASP.NET Core runtime
    // SQL Server password stored in env variable
    ["SQLSERVER-PASSWORD"] = "Strong@12345",

    // Connection strings using env variable
    ["ConnectionStrings:DefaultConnection"] = "Server=sqlserver,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",
    ["ConnectionStrings:CAP-SQLConnection"] = "Server=sqlserver,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",
    ["ConnectionStrings:HangfireConnection"] = "Server=sqlserver,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",

    // App secrets
    ["JWTSettings:SecretKey"] = "qwertyuiop[]asd124tgqrweee15364qfwretryutiyuryrterdvfn",
    ["OTPSettings:SecretKey"] = "qwertyuiop[UsersOtp]asd124tgqrweee15364qfwretryutiyuryrterdvfn",
    ["EmailPasswords"] = "vxfdhstkqegcfnei - csxyokaorioanxek - 11223344Upskilling",

    // CAP / RabbitMQ
    ["Cap:RabbitMQ:HostName"] = "rabbitmq",
    ["Cap:RabbitMQ:Port"] = "5672",
    ["Cap:RabbitMQ:UserName"] = "user",
    ["Cap:RabbitMQ:Password"] = "password",
    ["Cap:RabbitMQ:ExchangeName"] = "cap.default.router",
    ["Cap:DefaultGroupName"] = "Cap.queue",
    ["Cap:FailedRetryCount"] = "5",

    // Allow unsecured transport
    ["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true"
};

// Add configuration
builder.Configuration.AddInMemoryCollection(envConfig);

// Set OS environment variables for child containers/processes
foreach (var kv in envConfig)
{
    Environment.SetEnvironmentVariable(kv.Key, kv.Value, EnvironmentVariableTarget.Process);
    Environment.SetEnvironmentVariable(kv.Key.Replace(":", "__"), kv.Value, EnvironmentVariableTarget.Process);
}

// ================================
// 2. Start SQL Server via Package
// ================================
var sqlPasswordParam = builder.AddParameter("SQLSERVER-PASSWORD", "Strong@12345");

var sqlServer = builder.AddSqlServer("sqlserver", port: 1433)
    .WithPassword(sqlPasswordParam)
    .WithDataVolume();

var db = sqlServer.AddDatabase("PMS");

// ================================
// 3. Start RabbitMQ via Package
// ================================
var rabbitContainer = builder.AddRabbitMQ("rabbit");

// ================================
// 4. Start Redis Cache
// ================================
var cache = builder.AddRedis("cache");

// ================================
// 5. API Service
// ================================
var apiService = builder.AddProject<Projects.UserManagement_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(rabbitContainer)
    .WaitFor(rabbitContainer)
    .WithReference(cache)
    .WaitFor(cache);

// ================================
// 6. Web Frontend
// ================================
builder.AddProject<Projects.UserManagement_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

// ================================
// 7. Build & Run
// ================================
builder.Build().Run();