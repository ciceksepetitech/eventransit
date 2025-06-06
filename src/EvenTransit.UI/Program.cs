﻿using EvenTransit.Data.MongoDb;
using EvenTransit.Domain.Configuration;
using EvenTransit.Domain.Enums;
using EvenTransit.Logging.Serilog;
using EvenTransit.Messaging.RabbitMq;
using EvenTransit.Service;
using EvenTransit.Service.BackgroundServices;
using EvenTransit.UI.Filters;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Configuration.AddEnvironmentVariables("EvenTransit_");

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

// Add services to the container.
services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
services.Configure<EvenTransitConfig>(configuration.GetSection("EvenTransit"));

var mode = configuration.GetValue<Mode>("EvenTransit:Mode");
var modeConsumer = mode != Mode.Publisher;

services.AddScoped<ValidateModelAttribute>();
services.AddRabbitMq(configuration, modeConsumer);
services.AddHttpClient();
services.AddAutoMapper(typeof(Program));

if (modeConsumer)
{
    services.AddMongoDbDatabase(configuration);
}

services.AddServices(modeConsumer);
services.AddMessaging(modeConsumer);
services.AddHealthChecks();

IMvcBuilder mvcBuilder;

if (modeConsumer)
{
    services.AddHostedService<ConsumerBinderService>();

    mvcBuilder = services.AddControllersWithViews().AddRazorRuntimeCompilation();
}
else
{
    mvcBuilder = services.AddControllers();
}

mvcBuilder.AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<Program>());

services.AddSwaggerGen(c =>
{
    c.MapType<object>(() => new OpenApiSchema
    {
        Type = "object",
        Nullable = true
    });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EvenTransit.Api",
        Version = "v1"
    });
    c.ResolveConflictingActions(a => a.First());
});

void App()
{
    var app = builder.Build();

    ConfigureMinThreads();

    app.ConfigureSerilogLogger();

    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "EvenTransit.Api v1"); });

    app.UseHttpsRedirection();

    if (modeConsumer)
    {
        app.UseStaticFiles();
    }

    app.UseRouting();

    app.MapHealthChecks("/healthcheck", new HealthCheckOptions { Predicate = _ => false });
    app.MapHealthChecks("/readiness");

    if (modeConsumer)
    {
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
    }
    else
    {
        app.MapControllers();
    }

    app.Run();
}

Bootstrapper.Run<SerilogBootstrapLogger>(App);

static void ConfigureMinThreads()
{
    const string minCopThreads = "MIN_IOC_THREAD";
    const string minWorkerThreads = "MIN_WORKER_THREAD";
    const string defaultValue = "200";

    var envMinWorker = Environment.GetEnvironmentVariable(minWorkerThreads);
    var envMinIoc = Environment.GetEnvironmentVariable(minCopThreads);

    if (string.IsNullOrEmpty(envMinWorker))
        envMinWorker = defaultValue;
    if (string.IsNullOrEmpty(envMinIoc))
        envMinIoc = defaultValue;

    ThreadPool.SetMinThreads(Convert.ToInt32(envMinWorker), Convert.ToInt32(envMinIoc));
}
