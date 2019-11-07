using FailoverScheduler.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthorization;

namespace FailoverScheduler.Services
{
    public interface IHandlingDBService
    {
        Task CreateFailOver(MessageModel model);
    }
    public class HandlingDBService : IHandlingDBService
    
    {
        private readonly FailoverDbContext _context;
        private readonly ILogger _logger;
        public HandlingDBService(FailoverDbContext context,ILogger<HandlingDBService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateFailOver(MessageModel model)
        {
            try
            {
                FailoverModel request = new FailoverModel()
                {
                    UUID = Guid.NewGuid(),
                    Stamp = DateTime.Now,
                    retries = "5m",
                    topicName = model.topic,
                    attempt = 0,
                    UserId = model.userId,
                    metadata = model.metadata,
                    totalRetry = 0,
                };
                await _context.FailoverMonitoring.AddAsync(request);
                 _context.SaveChanges();
            }
            catch(Exception e)
            {
                _logger.LogInformation(e, e.Message);
            }

        }
    }
}
