using Seedwork.Auth.Configuration;
using Seedwork.Configuration;
using Seedwork.HttpClientHelpers;
using Seedwork.Logging;
using Seedwork.Web.Configuration;

namespace StructuringNotifications.WebApp.Configuration
{
    public class StructuringNotificationsConfiguration :
        ConfigurationBaseDto,
        ILoggingConfiguration,
        IRequestResponseLoggerMiddlewareConfiguration,
        ISwaggerConfiguration
    {
        public string StructuringNotificationsDbContext { get; set; }

        public LoggingConfiguration LoggingConfiguration { get; set; }

        public RequestResponseLoggerMiddlewareConfig RequestResponseLoggerMiddlewareConfig { get; set; }

        public Auth0Configuration Auth0Configuration { get; set; }

        public SwaggerConfiguration Swagger { get; set; }

        public string HttpRetryPolicyName => "DefaultRetryPolicy";
    }
}