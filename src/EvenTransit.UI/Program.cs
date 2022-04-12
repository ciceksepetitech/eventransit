using EvenTransit.Data.MongoDb;
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
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

// Add services to the container.
services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
services.AddScoped<ValidateModelAttribute>();
services.AddRabbitMq(configuration);
services.AddHttpClient();
services.AddAutoMapper(typeof(Program));
services.AddMongoDbDatabase(configuration);
services.AddServices();
services.AddMessaging();
services.AddHealthChecks();

services.AddScoped<IEventService, EventService>();
services.AddHostedService<QueueDeclarationService>();
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

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());

var app = builder.Build();

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

app.Run();
