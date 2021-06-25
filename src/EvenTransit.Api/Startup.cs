using System.Reflection;
using EvenTransit.Data;
using EvenTransit.Data.MongoDb;
using EvenTransit.Messaging.RabbitMq;
using EvenTransit.Service;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.BackgroundServices;
using EvenTransit.Service.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EvenTransit.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDbDatabase(Configuration);
            services.AddHttpClient();
            services.AddRabbitMq(Configuration);
            services.AddMessaging();

            services.AddScoped<IEventService, EventService>();
            services.AddHostedService<QueueDeclarationService>();
            services.AddHostedService<ConsumerBinderService>();
            services.AddHostedService<LogStatisticsService>();
            services.AddHostedService<EventLogStatisticsService>();

            services.AddHealthChecks();
            
            services.AddControllers().AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "EvenTransit.Api", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EvenTransit.Api v1"));
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}