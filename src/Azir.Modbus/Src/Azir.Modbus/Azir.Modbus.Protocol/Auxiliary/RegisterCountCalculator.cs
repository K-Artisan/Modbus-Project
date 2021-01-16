using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.DataPoints;

namespace Azir.Modbus.Protocol.Auxiliary
{
    public class RegisterCountCalculator
    {
        /// <summary>
        /// 根据数据点的类型获取其占用的寄存器个数,
        /// </summary>
        /// <param name="dataPointType">数据点的类型</param>
        /// <returns></returns>
        public static int GetRegisterCount(DataPointDataType dataPointType)
        {
            int result = 0;

            switch (dataPointType)
            {
                case DataPointDataType.S16:
                    result = 1;
                    break;
                case DataPointDataType.U16:
                    result = 1;
                    break;
                case DataPointDataType.S32:
                    result = 2;
                    break;
                case DataPointDataType.U32:
                    result = 2;
                    break;
                case DataPointDataType.S64:
                    result = 4;
                    break;
                case DataPointDataType.U64:
                    break;
                case DataPointDataType.F32:
                    result = 2;
                    break;
                case DataPointDataType.D64:
                    result = 4;
                    break;
                case DataPointDataType.Bit:
                    result = 1;
                    break;
                default:
                    break;

            }

            return result;
        }
    }
}
