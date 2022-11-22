using Microsoft.Extensions.DependencyInjection;

namespace StructuringNotifications.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile));
            return serviceCollection;
        }
    }
}