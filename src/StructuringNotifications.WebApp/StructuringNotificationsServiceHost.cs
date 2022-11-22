using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seedwork.UnitOfWork;
using Serilog;
using StructuringNotifications.WebApp.Configuration;
using Seedwork.Web;

namespace StructuringNotifications.WebApp
{
    public class StructuringNotificationsServiceHost :
        SeedworkHost<Startup, StructuringNotificationsConfiguration>
    {
        protected override IEnumerable<Func<IServiceProvider, Task>> GetBeforeStartupActions()
        {
            yield break;
        }

        protected override IEnumerable<Func<IServiceProvider, Task>> GetAfterStartupActions()
        {
            yield break;
        }

        }
    }