using System;
using System.Threading;
using System.Threading.Tasks;
using Seedwork.UnitOfWork;

namespace StructuringNotifications.Tests
{
    public class MockTaskExecutor : ITaskExecutor
    {
        public async Task<bool> Execute(Func<Task<bool>> task, CancellationToken cancellationToken = new())
        {
            return await task.Invoke();
        }
    }
}