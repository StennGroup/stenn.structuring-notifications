using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using idunno.Authentication.Basic;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Seedwork.HttpClientHelpers;
using Seedwork.Web.HttpClientHandlers;
using Seedwork.Configuration.Contracts;
using Seedwork.UnitOfWork;
using Seedwork.Web;
using Seedwork.Web.Extensions;
using Seedwork.Web.Middleware;
using StructuringNotifications.Application;
using StructuringNotifications.Application.Api;
using StructuringNotifications.Interop;
using StructuringNotifications.WebApp.Configuration;
using StructuringNotifications.WebApp.Infrastructure;

#pragma warning disable CS1591

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
            ConfigureAuthentication(services);

            services.AddHttpContextAccessor();
            services.AddScoped<ITaskExecutor, SimpleTaskExecutor>();
            services.AddControllers(options =>
                {
                    options.Filters.Add<PerformanceLoggerFilter>();
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
                .AddOperationsClient(EnvironmentConfig.EnvironmentName)
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
        /// <summary>
        /// </summary>
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
        /// <summary>
        /// </summary>
        protected override (bool Enabled, IReadOnlyCollection<string> Excludes) RequestResponseLogging => (true, new List<string>()
        {
            //swagger
            @"\/index\.html.*",
            @"\/swagger.*",
            //api
            @"\/\b",
            @"\/favicon.*"
        });

        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasic(options =>
                {
                    options.Realm = "Structuring Notifier API";
                    options.AllowInsecureProtocol = false;
                    options.Events = new BasicAuthenticationEvents
                    {
                        OnValidateCredentials = ctx =>
                        {
                            if (ctx.Username == ConfigurationDto.BasicAuthentication.Login &&
                                ctx.Password == ConfigurationDto.BasicAuthentication.Password)
                            {
                                var claims = new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, ctx.Username, ClaimValueTypes.String,
                                        ctx.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, ctx.Username, ClaimValueTypes.String,
                                        ctx.Options.ClaimsIssuer)
                                };
                                ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ctx.Scheme.Name));
                                ctx.Success();
                            }
                            else
                            {
                                ctx.Fail("Invalid login or password");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}