using GpioMonitor.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

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

        /// <summary>
        /// Period in milliseconds
        /// </summary>
        public int Period { get; set; } = 5000;

        override protected async Task<GpioState> readPinAsync(WiringPiPin pin)
        {
            return await Task.Run<GpioState>(() => {
                GpioState result = new GpioState(pin, 0);
                double dt = (DateTime.Now - startTime).TotalMilliseconds;
                double phase = (dt % Period) / Period;
                double signal = 0.5d + Math.Sin(phase * 2 * Math.PI) / 2d;
                double noise = (rnd.NextDouble() - .5) * NoiseLevel;
                result.Value = (signal * (1 + noise)) > .5d ? GpioPinValue.High : GpioPinValue.Low;
                return result;
            });
        }
    }
}
