using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Data.DataServices;
using EvenTransit.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Data.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddScoped<IEventsRepository, EventsMongoRepository>();
            services.AddScoped<ILogsRepository, LogsRepository>();
            services.AddScoped<IEventsDataService, EventsDataService>();
            
            return services;
        }
    }
}