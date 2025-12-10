using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Reflection;
using System.Text;
using UserManagement.ApiService.Common.Behaviors;
using UserManagement.ApiService.Configrations;
using UserManagement.ApiService.Data;
using UserManagement.ApiService.Filters;
using UserManagement.ApiService.Middlewares;
using UserManagement.ApiService.src.Common.Cap;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule<ApplicationModule>();
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
#region CAP
var capConn = builder.Configuration.GetConnectionString("CAP-SQLConnection");

builder.Services.AddCap(options =>
{
    options.UseSqlServer(capConn);
    options.UseEntityFramework<Context>();

    options.UseRabbitMQ(opt =>
    {
        opt.HostName = builder.Configuration["Cap:RabbitMQ:HostName"];
        opt.Port = int.Parse(builder.Configuration["Cap:RabbitMQ:Port"] ?? "5672");
        opt.UserName = builder.Configuration["Cap:RabbitMQ:UserName"];
        opt.Password = builder.Configuration["Cap:RabbitMQ:Password"];

    });
    options.FailedRetryCount = int.Parse(builder.Configuration["Cap:FailedRetryCount"] ?? "5");

    options.UseDashboard();
});


builder.Services.AddTransient<CapConsumer>();
#endregion
#region JWT
var jwtSettings = builder.Configuration.GetSection("JWTSettings");
var otpSettings = builder.Configuration.GetSection("OTPSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
var otpKey = Encoding.UTF8.GetBytes(otpSettings["SecretKey"]);
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
    };
})
.AddJwtBearer("2FA", opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = otpSettings.GetValue<string>("Issuer"),
        ValidAudience = otpSettings.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(otpKey),
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
    };
});
#endregion
#region Hangfire
builder.Services.AddHangfire(configuration =>
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.FromSeconds(5),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));
builder.Services.AddHangfireServer();
#endregion
builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

    options.AddOpenBehavior(typeof(TransactionBehavior<,>));

});
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<UserInfoFilter>();
    //options.Filters.Add<CancellationTokenFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Context>();
    await db.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(db);
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalErrorHandlerMiddleware>();
//app.UseMiddleware<TimeOutMiddleware>();

app.MapControllers();


app.MapDefaultEndpoints();
app.UseHangfireDashboard("/hangfire");

app.Run();
