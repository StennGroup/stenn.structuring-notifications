using Microsoft.Extensions.DependencyInjection;
using Stenn.AspNetCore.Versioning;
using Stenn.AspNetCore.Versioning.Extensions.DependencyInjection;

namespace StructuringNotifications.WebApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiVersion(this IServiceCollection services)
        {
            services.AddVersioningForApi<DefaultApiVersionInfoProviderFactory>();
            return services;
        }
    }
}