namespace StructuringNotifications.Application;

public interface IOverdueNotificationsConfiguration
{
    string[] CompanyDunsesToNotify { get; set; }
}