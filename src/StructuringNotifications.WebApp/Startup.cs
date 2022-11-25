using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Seedwork.HttpClientHelpers;
using Seedwork.Web.HttpClientHandlers;
using Seedwork.Configuration.Contracts;
using Seedwork.UnitOfWork.DependencyInjection;
using Seedwork.Web;
using Seedwork.Web.Extensions;
using Seedwork.Web.HealthChecks;
using Seedwork.Web.Middleware;
using StructuringNotifications.Application;
using StructuringNotifications.Application.Api;
using StructuringNotifications.WebApp.Configuration;
using StructuringNotifications.WebApp.Infrastructure;

namespace StructuringNotifications.WebApp
{
    public class Startup : SeedworkStartup<StructuringNotificationsConfiguration>
    {
        private const string AllowAllCorsPolicy = "AllowAll";

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void DoConfigureServices(IServiceCollection services)
        {
            services.RemoveRoutingNotForProduction(ConfigurationDto.ConfigType == ConfigType.Prod);

            services.AddScoped<SecurityContextProvider>();
            services.AddScoped<IUserContext>(p => p.GetRequiredService<SecurityContextProvider>().Context);

            ConfigureHttpClient(services);

            services.AddHttpContextAccessor();

            services.AddControllers(options =>
                {
                    options.Filters.Add<PerformanceLoggerFilter>();
                    options.Filters.Add<TransactionControlFilter>();
                    options.Filters.Add(new AuthorizeFilter());
                })
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                           ForwardedHeaders.XForwardedProto;
                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit 
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllCorsPolicy,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddApiVersion();

            services
                .AddApplicationServices();
        }

        private void ConfigureHttpClient(IServiceCollection services)
        {
            services.AddScoped<WebRequestFillerHandler>();
            services.AddScoped<HttpLoggingHandler>();

            var delay = Backoff.DecorrelatedJitterBackoffV2(
                medianFirstRetryDelay: TimeSpan.FromSeconds(5),
                retryCount: 3);

            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(delay);

            services.AddPolicyRegistry()
                .Add(ConfigurationDto!.HttpRetryPolicyName, retryPolicy);
        }

        protected override void AddHealthChecks(IHealthChecksBuilder builder)
        {
        }

        protected override void DoConfigure(IApplicationBuilder builder)
        {
            builder.UseForwardedHeaders();
            builder.UseRouting();
            builder.UseCors(AllowAllCorsPolicy);
            if (ConfigurationDto.ConfigType == ConfigType.Dev)
                builder.UseHsts();
            builder.UseAuthentication();
            builder.UseMiddleware<WebRequestOperationContextFillerMiddleware>();
            builder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        protected override (bool Enabled, IReadOnlyCollection<string> Excludes) RequestResponseLogging => (true, new List<string>()
        {
            //swagger
            @"\/index\.html.*",
            @"\/swagger.*",
            //api
            @"\/\b",
            @"\/favicon.*"
        });

    }
}