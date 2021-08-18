using System.Linq;
using EvenTransit.Data;
using EvenTransit.Data.MongoDb;
using EvenTransit.Messaging.RabbitMq;
using EvenTransit.Service;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.BackgroundServices;
using EvenTransit.Service.Services;
using EvenTransit.UI.Filters;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EvenTransit.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.AddScoped<ValidateModelAttribute>();
            services.AddRabbitMq(Configuration);
            services.AddHttpClient();
            services.AddAutoMapper(typeof(Startup));
            services.AddMongoDbDatabase(Configuration);
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
                .AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddSwaggerGen(c =>
            {
                c.MapType<object>(() => new OpenApiSchema { Type = "object", Nullable = true });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EvenTransit.Api", Version = "v1" });
                c.DocumentFilter<SwaggerFilterOutControllers>();
                c.ResolveConflictingActions(a => a.First());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}