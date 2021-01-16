using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.DataReponse
{
    public class AnalyzeRecivedDataReponse
    {
        /// <summary>
        /// false：Modbus返回错误的数据、或者数据不对应等；
        /// </summary>
        private bool modbusReponseSuccess = true;
        /// <summary>
        /// 解析Modbus响应数据是否成功
        /// </summary>
        private bool analyzeRecivedDataSuccess = true;
        private List<Register> registers = new List<Register>();

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Messager { get; set; }

        /// <summary>
        /// 解析Modbus响应数据是否成功,true:成功；否则,失败
        /// </summary>
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

        /// <summary>
        /// false：Modbus返回错误的数据、或者数据不对应等；
        /// </summary>
        public bool AnalyzeRecivedDataSuccess
        {
            get { return analyzeRecivedDataSuccess; }
            set { analyzeRecivedDataSuccess = value; }
        }

    }
}
