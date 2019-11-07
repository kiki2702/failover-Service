using System;
using Quartz;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;

namespace FailoverScheduler.Factories
{
    public class JobFactory : IJobFactory, IDisposable
    {
        private readonly IServiceScope _serviceScope;
        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceScope= serviceProvider.CreateScope();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceScope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
