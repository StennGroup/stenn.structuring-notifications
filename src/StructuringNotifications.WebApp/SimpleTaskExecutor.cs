using System;
using System.Threading;
using System.Threading.Tasks;
using Seedwork.UnitOfWork;

namespace StructuringNotifications.WebApp;

#pragma warning disable CS1591

public class SimpleTaskExecutor : ITaskExecutor
{

    public Task<bool> Execute(Func<Task<bool>> task, CancellationToken cancellationToken = default)
    {
        return task();
    }
}