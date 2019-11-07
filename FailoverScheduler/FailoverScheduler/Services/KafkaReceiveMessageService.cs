using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FailoverScheduler.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using FailoverScheduler.Interfaces;

namespace FailoverScheduler.Services
{
    public class KafkaReceiveMessageService : IKafkaReceiveMessageService
    {
        
        private readonly IConfiguration _configuration;
        private IReceiveMethodService _messageReceiver;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        public KafkaReceiveMessageService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }


        public void StartConsumeData()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _messageReceiver = scope.ServiceProvider.GetRequiredService<IReceiveMethodService>();
                var _DBcontext = scope.ServiceProvider.GetService<FailoverDbContext>();
                var topics = _DBcontext.FailoverMonitoring.Select(x=>x.topicName).ToList();
                //var topics = (from x in _DBcontext.FailoverMonitoring select x.topicName).ToList();

                foreach (var topic in topics)
                {
                    
                    Task.Run(() => _messageReceiver.Consume(
                                topic,
                                HandleConsumer,
                                HandleException));
                }
                
            }

        }

        public void HandleException(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        private void HandleConsumer(string data)
        {
            Console.WriteLine($"Data: {data}");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetService<FailoverDbContext>();

                var consume = _context.FailoverMonitoring.Where(x => x.metadata.Equals(data)).FirstOrDefault();
                _context.FailoverMonitoring.Remove(consume);
                _context.SaveChanges();
            }
        }
    }

 
}
