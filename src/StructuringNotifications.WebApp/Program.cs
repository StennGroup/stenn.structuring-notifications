using System.Threading.Tasks;

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