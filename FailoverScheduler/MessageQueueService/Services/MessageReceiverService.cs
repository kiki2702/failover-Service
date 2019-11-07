using System;
using Confluent.Kafka;

namespace MessageQueueService
{
    public class MessageReceiverService : IMessageReceiverService
    {
        private readonly ConsumerConfig _config;

        public MessageReceiverService(string kafkaBootstrapServers, string groupId)
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = kafkaBootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public void Consume(string topic, Action<string> processingMethod, Action<Exception> errorHandlingMethod)
        {
            using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
            {
                consumer.Subscribe(topic);

                var consuming = true;

                while (consuming)
                {
                    try
                    {
                        var result = consumer.Consume();
                        var message = result.Message.Value;

                        processingMethod(message);
                    }
                    catch (ConsumeException e)
                    {
                        errorHandlingMethod(e);
                    }
                }

                consumer.Close();
            }
        }
    }
}
