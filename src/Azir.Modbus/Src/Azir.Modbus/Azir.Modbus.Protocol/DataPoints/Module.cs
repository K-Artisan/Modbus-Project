using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.DataPoints
{
    public class Module
    {
        public  string Number { get; set; }
        public  string Name { get; set; }
        public  string Description { get; set; }

        private List<DataPoint> _dataPoints = new List<DataPoint>();
        public virtual List<DataPoint> DataPoints
        {
            get { return _dataPoints; }
        }
    }
}
