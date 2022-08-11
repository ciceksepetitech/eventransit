using EvenTransit.Data.MongoDb;
using EvenTransit.Domain.Configuration;
using EvenTransit.Logging.Serilog;
using EvenTransit.Messaging.RabbitMq;
using EvenTransit.Service;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.BackgroundServices;
using EvenTransit.Service.Services;
using EvenTransit.UI.Filters;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("EvenTransit_");

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

// Add services to the container.
services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
services.Configure<EvenTransitConfig>(configuration.GetSection("EvenTransit_"));
services.AddScoped<ValidateModelAttribute>();
services.AddRabbitMq(configuration);
services.AddHttpClient();
services.AddAutoMapper(typeof(Program));
services.AddMongoDbDatabase(configuration);
services.AddServices();
services.AddMessaging();
services.AddHealthChecks();

services.AddScoped<IEventService, EventService>();
services.AddHostedService<ConsumerBinderService>();
services.AddHostedService<LogStatisticsService>();
services.AddHostedService<EventLogStatisticsService>();

services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<Program>());

services.AddSwaggerGen(c =>
{
    c.MapType<object>(() => new OpenApiSchema { Type = "object", Nullable = true });
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EvenTransit.Api", Version = "v1" });
    c.ResolveConflictingActions(a => a.First());
});

void App()
{
    var app = builder.Build();

    app.ConfigureSerilogLogger();

    // Configure the HTTP request pipeline.
    if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "EvenTransit.Api v1"); });

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.MapHealthChecks("/healthcheck", new HealthCheckOptions { Predicate = _ => false });
    app.MapHealthChecks("/readiness");
    app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}");

    ConfigureMinThreads();
    
    app.Run();
}

Bootstrapper.Run<SerilogBootstrapLogger>(App);

static void ConfigureMinThreads()
{
    const string minCopThreads = "MIN_IOC_THREAD";
    const string minWorkerThreads = "MIN_WORKER_THREAD";
    const string defaultValue = "100";
            
    var envMinWorker = Environment.GetEnvironmentVariable(minWorkerThreads);
    var envMinIoc = Environment.GetEnvironmentVariable(minCopThreads);

    if (string.IsNullOrEmpty(envMinWorker)) envMinWorker = defaultValue;
    if (string.IsNullOrEmpty(envMinIoc)) envMinIoc = defaultValue;

    ThreadPool.SetMinThreads(Convert.ToInt32(envMinWorker), Convert.ToInt32(envMinIoc));
}