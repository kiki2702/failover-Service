using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FailoverScheduler.Interfaces
{
    public interface ISchedulerService
    {
        void Init();
        void Start();
        void Stop();
        void DeleteJob();
    }
}
