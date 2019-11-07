using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FailoverScheduler.Interfaces
{
    public interface IReceiveMethodService
    {
        void Consume(string topic, Action<string> processingMethod, Action<Exception> errorHandlingMethod);
    }
}
