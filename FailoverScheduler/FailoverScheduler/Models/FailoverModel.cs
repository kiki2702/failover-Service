using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FailoverScheduler.Models{
    //public class Message
    //{
    //  public string metadata{get;set;}
    //}
    public class FailoverModel
    {
        [Key]
        public Guid UUID { get; set; }
        public DateTime Stamp { get; set; }
        public string retries { get; set; }
        public string topicName { get; set; }
        public int attempt { get; set; }
        public Guid UserId { get; set; }
        public string metadata { get; set; }
        public int totalRetry { get; set; }
        

    }

    
}