using System;
using Seedwork.Web.ServiceBus.Configuration;
using StructuringNotifications.WebApp.Configuration;

namespace StructuringNotifications.WebApp.Infrastructure;

#pragma warning disable CS1591

public class OperationsNotificationsProcessingEndpointConfigurator : ProcessingEndpointConfigurator<
    StructuringNotificationsConfiguration>
{
    public override string EndpointName => StructuringNotificationsQueue.Name;
    protected override int ImmediateRetriesCount => 2;
    protected override int DelayedRetriesCount => 2;
    protected override TimeSpan DelayedRetryTimeIncrease => TimeSpan.FromSeconds(2);
    protected override int MessageProcessingConcurrency => 10;
    protected override TimeSpan TimeToKeepOutboxDeduplicationData => TimeSpan.FromHours(1);
    public override SubscriptionStrategy SubscriptionStrategy => SubscriptionStrategy.All();
}