using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FailoverScheduler.Interfaces;
using FailoverScheduler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace FailoverScheduler.Jobs
{
    public class FailoverProducerJob : IJob
    {
        private readonly ILogger<FailoverProducerJob> _logger;
        private readonly IKafkaSendMessageService _producer;
        public IConfiguration _configuration { get; private set; }
        private readonly FailoverDbContext _context;
        public FailoverProducerJob(ILogger<FailoverProducerJob> logger, IKafkaSendMessageService producer, IConfiguration configuration, FailoverDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _producer = producer;
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataOrder = await _context.FailoverMonitoring.ToListAsync();
            
            foreach (var data in dataOrder)
            {
                try
                {
                    var topic = $"{data.topicName}";
                    var msg = data.metadata;
                    _producer.Send(topic, msg);
                    //_context.Failover_handling.Remove(data);
                    //_context.SaveChanges();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
            await Task.CompletedTask;

        }
    }
}
