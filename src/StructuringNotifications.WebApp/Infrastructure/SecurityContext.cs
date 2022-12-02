using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using StructuringNotifications.Application.Api;

namespace StructuringNotifications.WebApp.Infrastructure
{
    public sealed class SecurityContext : IUserContext
    {
        public SecurityContext([NotNull] IIdentity identity)
        {
            UserName = identity.Name;
        }

        public string UserName { get; private set; }
    }
}