using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FailoverScheduler.Interfaces;
using Microsoft.Extensions.Logging;

namespace FailoverScheduler.Jobs
{
    public class FailoverConsumerJob : IJob
    {
        private readonly IKafkaReceiveMessageService _receiveMessage;
        private readonly ILogger _logger;
        public FailoverConsumerJob(IKafkaReceiveMessageService receiveMessage,ILogger<FailoverConsumerJob> logger)
        {
            _logger = logger;
            _receiveMessage = receiveMessage;
            
        }

        public async Task Execute(IJobExecutionContext context)
        {
                try
                {
                     _receiveMessage.StartConsumeData();
                     _logger.LogInformation("Success Consume Data and Delete temporary data on Database ");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            
            await Task.CompletedTask;
        }
    }
}
