using Seedwork.Auth.Configuration;
using Seedwork.Configuration;
using Seedwork.HttpClientHelpers;
using Seedwork.Logging;
using Seedwork.ServiceBus;
using Seedwork.Web.Configuration;
using StructuringNotifications.Application;
using StructuringNotifications.Interop;

namespace StructuringNotifications.WebApp.Configuration
{
    public class StructuringNotificationsConfiguration :
        ConfigurationBaseDto,
        ILoggingConfiguration,
        IRequestResponseLoggerMiddlewareConfiguration,
        ISwaggerConfiguration,
        IOperationsApiConfiguration,
        IServiceBusConfiguration,
        IOverdueNotificationsConfiguration
    {
        public string StructuringNotificationsDbContext { get; set; }

        public LoggingConfiguration LoggingConfiguration { get; set; }

        public RequestResponseLoggerMiddlewareConfig RequestResponseLoggerMiddlewareConfig { get; set; }

        public LoginPassword BasicAuthentication { get; set; }

        public SwaggerConfiguration Swagger { get; set; }

        public string HttpRetryPolicyName => "DefaultRetryPolicy";
        public string RootUrlOperationsApi { get; set; }
        public string OperationsApiAdClientId { get; set; }
        public string OperationsApiAdClientSecret { get; set; }
        public string OperationsApiAdInstance { get; set; }
        public string OperationsApiAdTenantId { get; set; }
        public string[] CompanyDunsesToNotify { get; set; }
        public ServiceBusConfiguration ServiceBusConfiguration { get; set; }
    }
}