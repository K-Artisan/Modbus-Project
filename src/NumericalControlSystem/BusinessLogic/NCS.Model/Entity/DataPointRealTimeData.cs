using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Model.Entity
{
    public partial class DataPointRealTimeData
    {
        public DataPoint DataPoint { get; set; }
        public double RealTimeValue { get; set; }
    }
}
