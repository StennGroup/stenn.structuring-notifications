using Seedwork.Web.ServiceBus.Configuration;
using StructuringNotifications.WebApp.Configuration;

namespace StructuringNotifications.WebApp.Infrastructure;

#pragma warning disable CS1591
public class OperationsNotificationsSendOnlyEndpointConfigurator :
    SendOnlyEndpointConfigurator<StructuringNotificationsConfiguration>
{
    public override string EndpointName => StructuringNotificationsQueue.Name;
}