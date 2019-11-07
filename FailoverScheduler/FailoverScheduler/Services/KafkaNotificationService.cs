using Confluent.Kafka;
using FailoverScheduler.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FailoverScheduler.Services
{
    public class KafkaNotificationService : IKafkaNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ProducerConfig _producerConfig;
        private readonly int _timeoutMs = 60000;

        public KafkaNotificationService(IConfiguration configuration, ILogger<KafkaNotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _producerConfig = new ProducerConfig
            {
                Acks = Acks.Leader,
                BootstrapServers = _configuration.GetValue<string>("Kafka:ProducerNotif:BootstrapServers"),
                MessageTimeoutMs = _timeoutMs,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 6000,
                LingerMs = 6000,
                MaxInFlight = 1,
            };
        }

        public void Send(string topic, string message)
        {
            SendAsync(topic, message).GetAwaiter().GetResult();
        }

        public async Task SendAsync(string topic, string message)
        {

            _logger.LogInformation($"Sending. Topic: {topic} ");
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Sending. Topic: {topic}");
            }
            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {

                var msg = new Message<Null, string>
                {

                    Value = message
                };
                DeliveryResult<Null, string> deliveryResult;
                try
                {

                    deliveryResult = await producer.ProduceAsync(topic, msg);


                }
                catch (ProduceException<Null, string> e)
                {
                    if (e.Error.Code == ErrorCode.Local_MsgTimedOut)
                    {
                        throw new TimeoutException($"Error code \"{e.Error.Code}\". Failed to send within {_timeoutMs} milliseconds.", e);
                    }
                    else
                    {
                        throw new Exception($"Error code \"{e.Error.Code}\". Failed to send message.", e);
                    }
                }
            }
        }
    }
}
