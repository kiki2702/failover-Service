using System;
using Confluent.Kafka;

namespace MessageQueueService
{
    public interface IMessageReceiverService
    {
        void Consume(string topic, Action<string> processingMethod, Action<Exception> errorHandlingMethod);
    }
}