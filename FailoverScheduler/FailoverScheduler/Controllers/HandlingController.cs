using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FailoverScheduler.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FailoverScheduler.Services;
using Microsoft.Extensions.Logging;
using FailoverScheduler.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FailoverScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class HandlingController : Controller
    {
        private readonly IHandlingDBService _handlingService;
        private readonly ILogger _logger;
        public HandlingController(IHandlingDBService handlingService,ILogger<HandlingController> logger)
        {
            _handlingService = handlingService;
            _logger = logger;
        }

        public IActionResult PostMessage([FromBody]MessageModel model)
        {
            try
            {
                _handlingService.CreateFailOver(model);
                return Created("", "Success");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest();
            }
            
        }
    }
}
