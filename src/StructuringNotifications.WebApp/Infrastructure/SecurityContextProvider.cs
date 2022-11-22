using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using StructuringNotifications.Application.Api;

namespace StructuringNotifications.WebApp.Infrastructure
{
    public sealed class SecurityContextProvider
    {
        private readonly ClaimsPrincipal _principal;

        public SecurityContextProvider(
            IHttpContextAccessor httpContextAccessor)
        {
            _principal = httpContextAccessor.HttpContext?.User;
        }

        public IUserContext Context
        {
            get { return new SecurityContext(_principal.Identities.First(x => x.IsAuthenticated)); }
        }
    }
}