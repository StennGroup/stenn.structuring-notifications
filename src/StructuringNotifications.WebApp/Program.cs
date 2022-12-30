using System.Threading.Tasks;
using StructuringNotifications.WebApp.Infrastructure;

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