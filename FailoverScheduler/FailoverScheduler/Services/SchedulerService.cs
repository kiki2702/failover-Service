using FailoverScheduler.Factories;
using FailoverScheduler.Interfaces;
using FailoverScheduler.Jobs;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz.Impl.Matchers;

namespace FailoverScheduler.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly IScheduler _scheduler;
        private readonly IConfiguration _configuration;
        public SchedulerService(JobFactory quartzJobFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            var schedFactory = new StdSchedulerFactory();
            _scheduler = schedFactory.GetScheduler()
                         .GetAwaiter()
                         .GetResult();
            _scheduler.JobFactory = quartzJobFactory;
        }

        public void Init()
        {
            var scheduleProducer = _configuration.GetValue<string>("Failover:Producer");
            var scheduleConsumer = _configuration.GetValue<string>("Failover:Consumer");
            AddJob<FailoverProducerJob>(scheduleProducer);
            AddJob<FailoverConsumerJob>(scheduleConsumer);
        }

        public void Start()
        {
            if (_scheduler.InStandbyMode)
            {
                _scheduler.Start();
                Console.WriteLine("Starting scheduler service");
            }

        }

        public void Stop()
        {
            if (_scheduler != null)
                _scheduler?.Shutdown(true);
        }

        public void DeleteJob()
        {
            DeleteJob<FailoverProducerJob>();
            
        }

        private void AddJob<T>(string cronExpression) where T : IJob
        {
            var jobName = typeof(T).Name;
            var groupName = jobName + "Group";
            var triggerName = groupName + "Trigger";

            var jobDetail = JobBuilder.Create<T>()
                            .WithIdentity(jobName, groupName)
                            .Build();

            var Trigger = TriggerBuilder.Create()
                                .WithIdentity(triggerName, groupName)
                                .StartNow()
                                .WithCronSchedule(cronExpression)
                                .Build();
            _scheduler.ScheduleJob(jobDetail, Trigger);
        }

        private void DeleteJob<T>() where T : IJob
        {
            var jobName = typeof(T).Name;
            var jobKeys = _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                        .GetAwaiter()
                        .GetResult();

            foreach(var jobKey in jobKeys)
            {
                if (jobKey.Name.Equals(jobName))
                    _scheduler.DeleteJob(jobKey);
            }
            //var jobDetail = JobBuilder.Create<T>()
            //                .WithIdentity(jobkey)
            //                .Build();
        }


    }
}
