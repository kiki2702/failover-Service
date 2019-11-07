using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FailoverScheduler.Interfaces
{
    public interface IKafkaNotificationService
    {
        void Send(string topic, string message);
        Task SendAsync(string topic, string message);
    }
}
