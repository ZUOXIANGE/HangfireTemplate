using System;
using System.Threading.Tasks;
using Hangfire;
using HangfireTemplate.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireTemplate.Tasks.Implements
{
    public class TestTask : IMyTask
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TestTask(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public string GetTaskName()
        {
            return nameof(TestTask);
        }

        public void Start(string cron)
        {
            RecurringJob.AddOrUpdate(GetTaskName(), () => WorkAsync(), cron, TimeZoneInfo.Local);
        }

        public async Task WorkAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetService<TestService>();

            if (service != null)
            {
                await service.HelloAsync();
            }
        }
    }
}