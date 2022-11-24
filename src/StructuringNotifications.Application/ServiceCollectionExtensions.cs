using Microsoft.Extensions.DependencyInjection;
using Seedwork.SystemDate;

namespace StructuringNotifications.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISystemDate, SystemDate>();
            serviceCollection.AddScoped<EssarOverdueNotificationService>();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile));
            return serviceCollection;
        }
    }
}