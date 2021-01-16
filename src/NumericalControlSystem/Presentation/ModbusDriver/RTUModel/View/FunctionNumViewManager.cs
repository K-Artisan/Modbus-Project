using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Modbus.Contract;

namespace ModbusDriver.RTUModel.View
{
    public class FunctionNumViewManager
    {
        public  static IFunctionNumView GetFunctionNumView(FunctionNumType functionNumType)
        {
            IFunctionNumView funNumView = null;

            switch (functionNumType)
            {
                case FunctionNumType.FunctionNum01:
                    break;
                case FunctionNumType.FunctionNum02:
                    break;
                case FunctionNumType.FunctionNum03:
                    funNumView = new FunctionNum03View();
                    break;
                case FunctionNumType.FunctionNum04:
                    break;
                case FunctionNumType.FunctionNum05:
                    break;
                case FunctionNumType.FunctionNum06:
                    funNumView = new FunctionNum06View();
                    break;
                case FunctionNumType.FunctionNum07:
                    break;
                case FunctionNumType.FunctionNum08:
                    break;
                case FunctionNumType.FunctionNum09:
                    break;
                case FunctionNumType.FunctionNum10:
                    break;
                case FunctionNumType.FunctionNum11:
                    break;
                case FunctionNumType.FunctionNum12:
                    break;
                case FunctionNumType.FunctionNum13:
                    break;
                case FunctionNumType.FunctionNum14:
                    break;
                case FunctionNumType.FunctionNum15:
                    break;
                case FunctionNumType.FunctionNum16:
                    break;
                case FunctionNumType.FunctionNum17:
                    break;
                case FunctionNumType.FunctionNum18:
                    break;
                case FunctionNumType.FunctionNum19:
                    break;
                case FunctionNumType.FunctionNum20:
                    break;
                default:
                    break;
            }

            return funNumView;
        }
    }
}
