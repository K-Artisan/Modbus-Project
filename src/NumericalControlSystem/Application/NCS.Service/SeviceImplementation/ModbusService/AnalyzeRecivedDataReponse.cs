using NCS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NCS.Service.SeviceImplementation.ModbusService
{
    public class AnalyzeRecivedDataReponse
    {
        private bool modbusReponseSuccess = true;    //Modbus返回错误的数据、或者数据不对应等；
        private bool analyzeRecivedDataSuccess = true;
        private List<Register> registers = new List<Register>();
        private DataPointType dataPointType = DataPointType.ReadByFunNum03;

        public bool ModbusReponseSuccess
        {
            get { return modbusReponseSuccess; }
            set { modbusReponseSuccess = value; }
        }
        public List<Register> Registers
        {
            get { return registers; }
            set { registers = value; }
        }
        public bool AnalyzeRecivedDataSuccess
        {
            get { return analyzeRecivedDataSuccess; }
            set { analyzeRecivedDataSuccess = value; }
        }
        public DataPointType DataPointType
        {
            get { return dataPointType; }
            set { dataPointType = value; }
        }
    }
}
