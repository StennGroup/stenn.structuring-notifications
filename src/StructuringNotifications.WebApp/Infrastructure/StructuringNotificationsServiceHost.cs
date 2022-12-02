using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Seedwork.ServiceBus;
using Seedwork.Web.ServiceBus;
using Seedwork.Web.ServiceBus.Configuration;
using StructuringNotifications.WebApp.Configuration;

namespace StructuringNotifications.WebApp.Infrastructure
{
    public class StructuringNotificationsServiceHost :
        SeedworkHostWithServiceBus<Startup, StructuringNotificationsConfiguration, ServiceBusOperationContext>
    {
        protected override IEnumerable<Func<IServiceProvider, Task>> GetBeforeStartupActions()
        {
            yield break;
        }

        protected override IEnumerable<Func<IServiceProvider, Task>> GetAfterStartupActions()
        {
            yield break;
        }

        protected override IConfigureServiceBusEndpoint<StructuringNotificationsConfiguration>
            EndpointInWebContainerConfigurator
            => new OperationsNotificationsSendOnlyEndpointConfigurator();

        protected override IEnumerable<IConfigureProcessingServiceBusEndpoint<StructuringNotificationsConfiguration>>
            EndpointsInSeparateContainersConfigurators
            => new[]
            {
                (IConfigureProcessingServiceBusEndpoint<StructuringNotificationsConfiguration>)
                new OperationsNotificationsProcessingEndpointConfigurator()
            };
    }
}