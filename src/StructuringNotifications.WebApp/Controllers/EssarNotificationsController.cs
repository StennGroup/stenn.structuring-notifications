using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StructuringNotifications.Application;

namespace StructuringNotifications.WebApp.Controllers;


/// <summary>
/// Essar notifications
/// </summary>
[ApiController]
[Route("essar")]
public class EssarNotificationsController: ControllerBase
{
    private readonly EssarOverdueNotificationService _overdueNotificationService;

    public EssarNotificationsController(EssarOverdueNotificationService overdueNotificationService)
    {
        _overdueNotificationService = overdueNotificationService;
    }
    
    /// <summary>
    /// Send overdue notifications by essar invoices
    /// </summary>
    [HttpGet("send-overdue")]
    public Task GetTest(CancellationToken cancellationToken) 
        => _overdueNotificationService.SendOverdueNotificationEventsAsync(cancellationToken);
}