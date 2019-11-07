using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FailoverScheduler.Models
{
    public class NotificationModel
    {
        public Guid? UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Attachment { get; set; }
        public DateTime? RequestTime { get; set; }
        public string EmailFrom { get; set; }
        public string EmailFromName { get; set; }
        public IList<string> EmailTo { get; set; }
        public IList<string> EmailCc { get; set; }
        public int? Priority { get; set; }
    }
}
