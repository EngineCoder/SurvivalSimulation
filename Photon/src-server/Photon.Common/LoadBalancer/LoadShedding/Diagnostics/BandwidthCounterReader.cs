using ExitGames.Diagnostics.Counter;

using System;
using System.Linq;

namespace Photon.Common.LoadBalancer.LoadShedding.Diagnostics
{
    /// <summary>
    /// Provides information about network throughput
    /// </summary>
    internal class BandwidthCounterReader 
    {
        private readonly ValueHistory values;

        /// <summary>
        ///   The windows performance counter field.
        /// </summary>
        private readonly ICounter bytesIn;

        /// <summary>
        ///   The windows performance counter field.
        /// </summary>
        private readonly ICounter bytesOut;

        public BandwidthCounterReader(int averageValueHistory)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    {
                        this.bytesIn = new PerformanceCounterReader("Photon Socket Server", "bytes in/sec", "_Total");
                        this.bytesOut = new PerformanceCounterReader("Photon Socket Server", "bytes out/sec", "_Total");
                        break;
                    }
                case PlatformID.Unix:
                    {
                        this.bytesIn = new Linux.LinuxBytesInCounterReader();
                        this.bytesOut = new Linux.LinuxBytesOutCounterReader();
                        break;
                    }
                default:
                    {
                        throw new PlatformNotSupportedException($"There is not counters to read CPU on {Environment.OSVersion.Platform}");
                    }
            }

            this.values = new ValueHistory(averageValueHistory);
        }

        public bool IsValid => this.bytesIn.IsValid && this.bytesOut.IsValid;

        public double GetNextAverage()
        {
            var value = (int)(this.bytesIn.GetNextValue() + this.bytesOut.GetNextValue());

            this.values.Add(value);
            return this.values.Average();
        }
    }
}
