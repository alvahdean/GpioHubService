using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GpioMonitor;
using GpioMonitor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Starbender.GpioService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpioController : ControllerBase
    {
        private static Dictionary<int, GpioState> _states = new Dictionary<int, GpioState>();

        private static IGpioStateSubscriber _subscription=GpioSubscriberFactory.Create((string)"/gpioState",(ILogger)null);

        public GpioController(IGpioStateSubscriber subscription)
        {
            _subscription = subscription;
            subscription.
        }

        // GET: api/Gpio
        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult(_states.Values);
        }

        // GET: api/Gpio/5
        [HttpGet("{id}", Name = "Get")]
        public JsonResult Get(int id)
        {
            return _states.ContainsKey(id) 
                ? new JsonResult(_states[id])
                : new JsonResult(null) { StatusCode = 404 };
        }

        // POST: api/Gpio (Set pin value)
        [HttpPost]
        public JsonResult Post([FromBody] int id, [FromBody] int value)
        {
            if (_states.ContainsKey(id))
            {
                _states[id].Value = value;
                return new JsonResult(_states[id]);
            }
            else
                return new JsonResult(null) { StatusCode = 500 };
        }

        // PUT: api/Gpio/5 (Add or modify)
        [HttpPut("{id}")]
        public JsonResult Put(int id, [FromBody] bool isAnalog=false)
        {
            if (_states.ContainsKey(id))
                _states[id].IsAnalog = isAnalog;
            else
                _states.Add(id, new GpioState(id, 0, null, isAnalog));
            return new JsonResult(true);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            if(_states.ContainsKey(id))
                _states.Remove(id);
            return new JsonResult(true);
        }
    }
}
