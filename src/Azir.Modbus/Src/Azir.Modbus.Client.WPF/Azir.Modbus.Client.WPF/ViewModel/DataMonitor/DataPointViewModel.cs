using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.DataPoints;
using Microsoft.Practices.Prism.ViewModel;

namespace Azir.Modbus.Client.WPF.ViewModel.DataMonitor
{
    public class DataPointViewModel : NotificationObject
    {
        #region 绑定界面的数据

        private int id;
        private string number;
        private string name;
        private int deviceAddress;
        private int startRegisterAddress;
        private DataPointDataType dataPointDataType;
        private DataPointType dataPointType;
        private string description;

        private double realTimeValue;
        private double valueToSet;

        private string moduleId;
        private string moduleNumber;
        private string moduleName;
        private string moduleDescription;

        private string modbusUnitNumber;
        private string modbusUnitName;
        
        #region 访问器

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                this.RaisePropertyChanged("Id");
            }
        }

        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                this.RaisePropertyChanged("Number");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        public int DeviceAddress
        {
            get { return deviceAddress; }
            set
            {
                deviceAddress = value;
                this.RaisePropertyChanged("DeviceAddress");
            }
        }

        public int StartRegisterAddress
        {
            get { return startRegisterAddress; }
            set
            {
                startRegisterAddress = value;
                this.RaisePropertyChanged("StartRegisterAddress");
            }
        }

        public DataPointDataType DataPointDataType
        {
            get { return dataPointDataType; }
            set
            {
                dataPointDataType = value;
                this.RaisePropertyChanged("DataPointDataType");
            }
        }

        public DataPointType DataPointType
        {
            get { return dataPointType; }
            set
            {
                dataPointType = value;
                this.RaisePropertyChanged("DataPointType");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                this.RaisePropertyChanged("Description");
            }
        }

        public double RealTimeValue
        {
            get { return realTimeValue; }
            set
            {
                realTimeValue = value;
                this.RaisePropertyChanged("RealTimeValue");
            }
        }

        public double ValueToSet
        {
            get { return valueToSet; }
            set
            {
                valueToSet = value;
                this.RaisePropertyChanged("ValueToSet");
            }
        }

        public string ModuleId
        {
            get { return moduleId; }
            set
            {
                moduleId = value;
                this.RaisePropertyChanged("ModuleId");
            }
        }

        public string ModuleNumber
        {
            get { return moduleNumber; }
            set
            {
                moduleNumber = value;
                this.RaisePropertyChanged("ModuleNumber");
            }
        }

        public string ModuleName
        {
            get { return moduleName; }
            set
            {
                moduleName = value;
                this.RaisePropertyChanged("ModuleName");
            }
        }


        public string ModuleDescription
        {
            get { return moduleDescription; }
            set
            {
                moduleDescription = value;
                this.RaisePropertyChanged("ModuleDescription");
            }
        }

        public string ModbusUnitNumber
        {
            get { return modbusUnitNumber; }
            set
            {
                modbusUnitNumber = value;
                this.RaisePropertyChanged("ModbusUnitNumber");
            }
        }


        public string ModbusUnitName
        {
            get { return modbusUnitName; }
            set
            {
                modbusUnitName = value;
                this.RaisePropertyChanged("ModbusUnitName");
            }
        }

        #endregion
        #endregion
    }
}
