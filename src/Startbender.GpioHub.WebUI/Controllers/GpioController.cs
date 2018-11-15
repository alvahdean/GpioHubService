using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GpioMonitor;
using GpioMonitor.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Starbender.GpioService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpioController : ControllerBase
    {
        private static object _syncRoot = new object();
        private IGpioStateSubscriber _subscription;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ILogger _logger;

        public GpioController(
            IHostingEnvironment hostingEnvironment
            ,IGpioStateSubscriber subscription
            ,ILogger logger)
        {
            _subscription = subscription;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;

            if(!_subscription.IsConnected)
            {
                lock (_syncRoot)
                {
                    if (_subscription.HubUri == null)
                    {
                        Uri uri = new Uri("http://localhost:5000");
                        string hubUrl = $"{uri.Scheme}://{uri.Host}:{uri.Port}/gpioState";
                        _logger.LogInformation($"Setting Url to [{hubUrl}]");
                        _subscription.HubUri = new Uri(hubUrl);
                    }
                    if (!_subscription.IsConnected)
                    {
                        _logger.LogInformation($"Connecting to [{_subscription.HubUrl}]");
                        _subscription.Connect();
                    }
                }
            }
        }

        // GET: api/Gpio
        [HttpGet]
        public JsonResult Get()
        {
            IEnumerable<GpioState> states =
                _subscription.Subscriptions
                .Select(t => _subscription.GetLast(t))
                .Where(t => t != null);
            return new JsonResult(states);
        }

        // GET: api/Gpio/5
        [HttpGet("{id}", Name = "Get")]
        public JsonResult Get(int id)
        {
            return _subscription.IsSubscribed(id) 
                ? new JsonResult(_subscription.GetLast(id))
                : new JsonResult(null) { StatusCode = 404 };
        }

        // POST: api/Gpio/5 (Add new subscription)
        [HttpPost("{id}")]
        public JsonResult Post(int id)
        {
            bool result = true;
            if (!_subscription.IsSubscribed(id))
            {
                result=_subscription.SubscribePin(id);
            }
            return new JsonResult(result);
        }

        // DELETE: api/Gpio/5 (Unsubscribe from pin)
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            bool result = true;
            if (_subscription.IsSubscribed(id))
            {
                _subscription.UnsubscribePin(id);
                result = !_subscription.IsSubscribed(id);
            }
            return new JsonResult(result);
        }
    }
}
