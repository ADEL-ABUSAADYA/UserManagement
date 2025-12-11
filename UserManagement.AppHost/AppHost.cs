using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

var builder = DistributedApplication.CreateBuilder(args);

var envConfig = new Dictionary<string, string>
{
    ["SQLSERVER-PASSWORD"] = "Strong@12345",

    ["ConnectionStrings:DefaultConnection"] = "Server=localhost,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",
    ["ConnectionStrings:CAP-SQLConnection"] = "Server=localhost,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",
    ["ConnectionStrings:HangfireConnection"] = "Server=localhost,1433;Database=PMS;User Id=sa;Password=Strong@12345;TrustServerCertificate=True;",

    ["JWTSettings:SecretKey"] = "qwertyuiop[]asd124tgqrweee15364qfwretryutiyuryrterdvfn",
    ["OTPSettings:SecretKey"] = "qwertyuiop[UsersOtp]asd124tgqrweee15364qfwretryutiyuryrterdvfn",
    ["EmailPasswords"] = "vxfdhstkqegcfnei - csxyokaorioanxek - 11223344Upskilling",

    // CAP / RabbitMQ: استخدم اسم الخدمة وليس localhost
    ["Cap:RabbitMQ:HostName"] = "rabbit",
    ["Cap:RabbitMQ:Port"] = "5672",
    ["Cap:RabbitMQ:UserName"] = "user",
    ["Cap:RabbitMQ:Password"] = "password",
    ["Cap:RabbitMQ:ExchangeName"] = "cap.default.router",
    ["Cap:DefaultGroupName"] = "Cap.queue",
    ["Cap:FailedRetryCount"] = "5",

    ["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true"
};

builder.Configuration.AddInMemoryCollection(envConfig);

foreach (var kv in envConfig)
{
    Environment.SetEnvironmentVariable(kv.Key, kv.Value, EnvironmentVariableTarget.Process);
    Environment.SetEnvironmentVariable(kv.Key.Replace(":", "__"), kv.Value, EnvironmentVariableTarget.Process);
}

// SQL Server container
var sqlPasswordParam = builder.AddParameter("SQLSERVER-PASSWORD", "Strong@12345");
var sqlServer = builder.AddSqlServer("sqlserver", port: 1433)
    .WithPassword(sqlPasswordParam)
    .WithDataVolume();
var db = sqlServer.AddDatabase("PMS");

// RabbitMQ container: تأكد من أن اسم المستخدم وكلمة المرور يتطابقان
var rabbitContainer = builder.AddRabbitMQ("rabbit");

// API Service
var apiService = builder.AddProject<Projects.UserManagement_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(rabbitContainer)
    .WaitFor(rabbitContainer);

// Web Frontend
builder.AddProject<Projects.UserManagement_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();