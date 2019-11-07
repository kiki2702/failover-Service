using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FailoverScheduler.Models
{
    public class MessageModel
    {
        public string metadata { get; set; }
        public string topic { get; set; }
        public Guid userId { get; set; }
    }
}
