using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace automation.Models
{
   
    public class Sensor
    {
        public UInt64 Id { get; set; }

        public string FriendlyName { get; set; }

        public string Description { get; set; }
    
        public byte[] CustomData { get; set; }

        public ICollection<TelemetryData> TelemetryData { get; set; } = new List<TelemetryData>();
    }
}
