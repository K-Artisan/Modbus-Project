using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCS.Service.Messaging.DataPointService;
using NCS.Model.Entity;

namespace NCS.Service.ViewModel.DataPoints
{
    /// <summary>
    /// DataPoint的基本信息
    /// </summary>
    public class DataPointInfoView
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public int DeviceAddress { get; set; }
        public int StartRegisterAddress { get; set; }
        public DataType DataType { get; set; }
        public DataPointType DataPointType { get; set; }
        public string Description { get; set; }

        public double RealTimeValue { get; set; }
        public double ValueToSet { get; set; }

        public int ModuleId { get; set; }
    }
}
