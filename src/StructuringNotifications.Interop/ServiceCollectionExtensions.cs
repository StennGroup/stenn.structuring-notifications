using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Seedwork.HttpClientHelpers;
using StructuringNotifications.Interop.Abstractions;
using StructuringNotifications.Interop.OperationsApi;

namespace StructuringNotifications.Interop;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOperationsClient(this IServiceCollection serviceProvider, string env)
    {
        serviceProvider.AddTransient<OperationsApiAuthenticationHandler>();
        serviceProvider.AddTransient<HttpLoggingHandler>();
        var clientBuilder = serviceProvider.AddHttpClient<IOperationsApi, OperationsApiClient>(
                (provider, client) =>
                {
                    var config = provider.GetRequiredService<IOperationsApiConfiguration>();
                    client.BaseAddress = new Uri($"{config.RootUrlOperationsApi}/");
                })
            .AddHttpMessageHandler<HttpLoggingHandler>();

        if (env != "Development")
        {
            clientBuilder.AddHttpMessageHandler<OperationsApiAuthenticationHandler>();
        }
        
        return serviceProvider;
    }
}