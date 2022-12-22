using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seedwork.UnitOfWork;
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
    private readonly ITaskExecutor _taskExecutor;

    public EssarNotificationsController(EssarOverdueNotificationService overdueNotificationService, ITaskExecutor taskExecutor)
    {
        _overdueNotificationService = overdueNotificationService;
        _taskExecutor = taskExecutor;
    }
    
    /// <summary>
    /// Send overdue notifications by essar invoices
    /// </summary>
    [HttpGet("send-overdue")]
    public Task GetTest(CancellationToken cancellationToken) 
        => _taskExecutor.Execute(async () =>
        {
            await _overdueNotificationService.SendOverdueNotificationEventsAsync(cancellationToken);
            return true;
        }, cancellationToken);
}