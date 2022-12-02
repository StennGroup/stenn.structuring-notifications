using System.Threading.Tasks;
using StructuringNotifications.WebApp.Infrastructure;

#pragma warning disable CS1591

namespace StructuringNotifications.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await new StructuringNotificationsServiceHost().RunAsync(args);
        }
    }
}