using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Confluent.Kafka;
using FailoverScheduler.Interfaces;
using FailoverScheduler.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FailoverScheduler.Services
{
    public class KafkaSendMessageService : IKafkaSendMessageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ProducerConfig _producerConfig;
        private readonly int _timeoutMs = 30000;
        private readonly FailoverDbContext _context;
        private readonly ISchedulerService _scheduler;
        private readonly IEmailNotificationService _sendNotif;
        

        public KafkaSendMessageService(IConfiguration configuration, ILogger<KafkaSendMessageService> logger, FailoverDbContext context,IEmailNotificationService sendNotif,ISchedulerService scheduler)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _producerConfig = new ProducerConfig
            {
                Acks = Acks.Leader,
                BootstrapServers = _configuration.GetValue<string>("Kafka:Producer:BootstrapServers"),
                MessageTimeoutMs = _timeoutMs,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 6000,
                LingerMs = 6000,
                MaxInFlight = 1,
            };
            _scheduler = scheduler;
            _sendNotif = sendNotif;
           
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
                        //List<OrderRequest> orderReq = JsonConvert.DeserializeObject<List<OrderRequest>>(message);
                        var orderReq = _context.FailoverMonitoring.ToList();
                        var attemptRetry = _context.FailoverMonitoring.Where(x => x.attempt == 3).FirstOrDefault();

                        if (attemptRetry!=null)
                        {
                            //NotificationModel notif = new NotificationModel
                            //{
                            //    Title = "test",
                            //    RequestTime = DateTime.UtcNow,
                            //    Message = "testing",
                            //    EmailFrom = "noreply@unitedtractors.com",
                            //    EmailFromName = "Failover Service Notification",
                            //    EmailTo = new string[] { "cst_dev101@unitedtractors.com" },
                            //    Priority = 1

                            //};

                            //var MessageJson = JsonConvert.SerializeObject(notif);
                            //_sendNotif.Send("utportal_emailnotification.add", MessageJson);
                            _sendNotif.SendEmail();

                            foreach (var order in orderReq)
                            {
                                order.attempt = 0;
                                _context.Attach(order);
                                _context.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _context.SaveChanges();
                            }

                        }
                        foreach (var item in orderReq)
                        {
                           var order = _context.FailoverMonitoring.Where(x => x.UUID.Equals(item.UUID)).FirstOrDefault();
                           {
                                order.totalRetry = order.totalRetry + 1;
                                order.attempt += 1;
                                //order.topicName = topic;
                                _context.Attach(order);
                                _context.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                           }
                            _context.SaveChanges();
                        }
                       
                        // Running failover

                        throw new TimeoutException($"Error code \"{e.Error.Code}\". Failed to send within {_timeoutMs} milliseconds.", e);
                    }
                    else
                    {
                        throw new Exception($"Error code \"{e.Error.Code}\". Failed to send message.", e);
                    }
                }
                _logger.LogInformation($"Sent. Topic: {deliveryResult.Topic} Partition: {deliveryResult.Partition.Value} Offset: {deliveryResult.Offset.Value} Message: {deliveryResult.Message.Value}");
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"Sent. Topic: {deliveryResult.Topic} Partition: {deliveryResult.Partition.Value} Offset: {deliveryResult.Offset.Value} Message: {deliveryResult.Message.Value}");
                }
            }

        }
    }
}
