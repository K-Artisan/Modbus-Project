using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract
{
    public class OperatingRegisteMaxNumOneTime
    {
        /// <summary>
        /// 各个功能码对应的每次能操作的寄存器个数的最大值，
        /// -1表示无限制或不用分包。
        /// </summary>
        private static Dictionary<FunctionNumType, int> canOperatingRegisteMaxNumOneTimeDic = new Dictionary<FunctionNumType, int>()
        {
            {FunctionNumType.FunctionNum01, 30},
            {FunctionNumType.FunctionNum02, -1},
            {FunctionNumType.FunctionNum03, 30},
            {FunctionNumType.FunctionNum04, -1},
            {FunctionNumType.FunctionNum05, -1},
            {FunctionNumType.FunctionNum06, 30},
            {FunctionNumType.FunctionNum07, -1},
            {FunctionNumType.FunctionNum08, -1},
            {FunctionNumType.FunctionNum09, -1},
            {FunctionNumType.FunctionNum10, -1},
            {FunctionNumType.FunctionNum11, -1},
            {FunctionNumType.FunctionNum12, -1},
            {FunctionNumType.FunctionNum13, -1},
            {FunctionNumType.FunctionNum14, -1},
            {FunctionNumType.FunctionNum15, 30},
            {FunctionNumType.FunctionNum16, 30},
            {FunctionNumType.FunctionNum17, -1},
            {FunctionNumType.FunctionNum18, -1},
            {FunctionNumType.FunctionNum19, -1},
            {FunctionNumType.FunctionNum20, -1},
        };

        public static int GetCanOperatingRegisterMaxNumOneTime(FunctionNumType functionNumType)
        {
            int canOperatingRegistMaxNumOneTime = -1;

            if (canOperatingRegisteMaxNumOneTimeDic.ContainsKey(functionNumType))
            {
                canOperatingRegistMaxNumOneTime = canOperatingRegisteMaxNumOneTimeDic[functionNumType];
            }

            return canOperatingRegistMaxNumOneTime;
        }
    }
}
