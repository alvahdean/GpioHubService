using GpioMonitor.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GpioMonitor
{
    public class MockPublisher : GpioStatePublisher
    {
        static protected DateTime startTime = DateTime.Now;

        private Random rnd;

        public MockPublisher()
            : this(null, null) { }

        public MockPublisher(string hubUrl, ILogger log) 
            : this(0,hubUrl, log) { }

        public MockPublisher(int seed,string hubUrl, ILogger log) 
            : base(hubUrl, log)
        {
            rnd = seed != 0 ? new Random(seed) : new Random();
        }

        /// <summary>
        /// Noise level in simulated signal
        /// </summary>
        public double NoiseLevel { get; set; } = 0;
        override protected async Task<GpioState> readPinAsync(int pinId)
        {
            GpioState result = new GpioState(pinId, 0);
            double dt = (DateTime.Now - startTime).TotalMilliseconds;
            double period = 5000;
            double phase = (dt % period)/period;
            double signal = (double)GpioState.MaxValue * ( 0.5d + Math.Sin(phase* 2 * Math.PI)/2d);
            double noise = (rnd.NextDouble() - .5) * NoiseLevel;
            result.Value = (int)(signal * (1+noise));
            await Task.Delay(5);
            return result;
        }
    }
}
