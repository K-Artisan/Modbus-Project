using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.FuncitonNum.CustomerRequest
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerRequstDataAuxiliary
    {
        /// <summary>
        /// 将每个请求帧的客户端格式分成更小的请求帧：
        /// 原因：每次可以操作的寄存器个数有限。
        /// </summary>
        /// <returns></returns>
        public static List<T> Splite<T>(FunctionNumType functionNum, int numOfOperatingRegister)
            where T : new()
        {
            List<T> customerRequestDatas = new List<T>();

            //每次可以操作的最大寄存器个数
            int canOperatingRegisterMaxNumOneTime = ModbusProtocolRule.GetCanOperatingRegisterMaxNumOneTime(functionNum);

            if (canOperatingRegisterMaxNumOneTime == -1)  //每次可以操作寄存器最大个数无限制，不用分包
            {
                customerRequestDatas.Add(new T());
                return customerRequestDatas;
            }
            else
            {
                customerRequestDatas = Splite<T>(canOperatingRegisterMaxNumOneTime, numOfOperatingRegister);
            }

            return customerRequestDatas;
        }

        private static List<T> Splite<T>(int canOperatingRegisterMaxNumOneTime, int numOfOperatingRegister)
            where T : new()
        {
            List<T> customerRequestDatas = new List<T>();
            int requestCount = (int)Math.Ceiling((double)numOfOperatingRegister / (double)canOperatingRegisterMaxNumOneTime);

            for (int i = 0; i < requestCount; i++)
            {
                customerRequestDatas.Add(new T());
            }

            return customerRequestDatas;
        }


    }
}
