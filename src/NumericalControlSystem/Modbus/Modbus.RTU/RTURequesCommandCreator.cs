using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Modbus.Contract;
using Modbus.Contract.RequestDataBase;
using Modbus.RTU.RTURequestData;

namespace Modbus.RTU
{
    public static class RTURequesCommandCreator
    {
        /// <summary>
        /// 将ICustomerRequestData（界面输入的请求信息）转换成RTU请求帧（若干个）
        /// 说明：
        /// 一个ICustomerRequestData对应若干个RTU请求帧，故返回List<List<byte>>
        /// </summary>
        /// <typeparam name="T">
        /// 1.T类型只能是如下类型:
        ///  double\ float\ int \long \short\ uint\ ulong\ ushort
        /// 2.T类型只是针对如下功能码:
        /// 06\16
        /// 其他功能码可以设置为任何类型,因为没用到类型T，
        /// 只是为了统一接口，而使用了泛型函数。
        /// </typeparam>
        /// <param name="customerRequestData">若干个RTU请求帧</param>
        /// <returns></returns>
        public static List<List<byte>> CreateRequestCommandByteStream<T>(ICustomerRequestData customerRequestData)
        {
            List<List<byte>> requesCommandByteStreams = new List<List<byte>>();

            switch (customerRequestData.FunctionNum)
            {
                case FunctionNumType.FunctionNum01:
                    requesCommandByteStreams =
                        CreateRequestCommandByteStreamsForFunNum01(customerRequestData);
                    break;
                case FunctionNumType.FunctionNum02:
                    break;

                case FunctionNumType.FunctionNum03:
                    requesCommandByteStreams = 
                        CreateRequestCommandByteStreamsForFunNum03(customerRequestData);
                    break;

                case FunctionNumType.FunctionNum04:
                    break;

                case FunctionNumType.FunctionNum05:
                    requesCommandByteStreams =
                        CreateRequestCommandByteStreamsForFunNum05(customerRequestData);
                    break;

                case FunctionNumType.FunctionNum06:
                    requesCommandByteStreams =
                        CreateRequestCommandByteStreamsForFunNum06<T>(customerRequestData);  
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
                    requesCommandByteStreams =
                        CreateRequestCommandByteStreamsForFunNum15(customerRequestData);
                    break;

                case FunctionNumType.FunctionNum16:
                    requesCommandByteStreams =
                        CreateRequestCommandByteStreamsForFunNum16<T>(customerRequestData);  
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

            return requesCommandByteStreams;            
        }

        #region CreateRequestCommandByteStream具体的方法
        /****************************************************************************
         * 这些列算法的核心：
         * 第一步：将ICustomerRequestData分解成若干个IFunNumRequestDataBase；
         * 第二步：将第一步得到的若干个IFunNumRequestDataBase转换成对应数量的
         *         IFunNumRequestDataRTU（RTU请求帧）
         * 第三步：将若干个IFunNumRequestDataRTU转换成对应数量的请求帧字节流
         ****************************************************************************/

        private static List<List<byte>> CreateRequestCommandByteStreamsForFunNum01(ICustomerRequestData customerRequestData)
        {
            List<List<byte>> requesCommandByteStreams = new List<List<byte>>();

            FunNum01CustomerRequestData funNum01CustomerRequestData = (FunNum01CustomerRequestData)customerRequestData;
            //第一步：将ICustomerRequestData分解成若干个IFunNumRequestDataBase；
            List<FunNum01RequestDataBase> funNum01RequestDataBases = funNum01CustomerRequestData.CovertToFunNum01RequestDataBases();

            foreach (FunNum01RequestDataBase item in funNum01RequestDataBases)
            {
                FunNum01RequestDataRTU funNum01RequestDataRTU = new FunNum01RequestDataRTU();
                //第二步：将第一步得到的若干个IFunNumRequestDataBase转换成对应数量的IFunNumRequestDataRTU（RTU请求帧）
                funNum01RequestDataRTU.FunNum01RequestDataBase = item;

                //第三步：将若干个IFunNumRequestDataRTU转换成对应数量的请求帧字节流
                requesCommandByteStreams.Add(funNum01RequestDataRTU.ToByteList());
            }

            return requesCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamsForFunNum03(ICustomerRequestData customerRequestData)
        {
            List<List<byte>> requesCommandByteStreams = new List<List<byte>>();

            FunNum03CustomerRequestData funNum03CustomerRequestData = (FunNum03CustomerRequestData)customerRequestData;
            List<FunNum03RequestDataBase> funNum03RequestDataBases = funNum03CustomerRequestData.CovertToFunNum03RequestDataBases();
           
            foreach (FunNum03RequestDataBase item in funNum03RequestDataBases)
            {
                FunNum03RequestDataRTU funNum03RequestDataRTU = new FunNum03RequestDataRTU();
                funNum03RequestDataRTU.FunNum03RequestDataBase = item;

                requesCommandByteStreams.Add(funNum03RequestDataRTU.ToByteList());
            }

            return requesCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamsForFunNum05(ICustomerRequestData customerRequestData)
        {
            List<List<byte>> requesCommandByteStreams = new List<List<byte>>();

            FunNum05CustomerRequestData funNum05CustomerRequestData = (FunNum05CustomerRequestData)customerRequestData;
            List<FunNum05RequestDataBase> funNum05RequestDataBases = funNum05CustomerRequestData.CovertToFunNum05RequestDataBases();

            foreach (FunNum05RequestDataBase item in funNum05RequestDataBases)
            {
                FunNum05RequestDataRTU funNum05RequestDataRTU = new FunNum05RequestDataRTU();
                funNum05RequestDataRTU.FunNum05RequestDataBase = item;

                requesCommandByteStreams.Add(funNum05RequestDataRTU.ToByteList());
            }

            return requesCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamsForFunNum06<T>(ICustomerRequestData customerRequestData)
        {
            List<List<byte>> requesCommandByteStreams = new List<List<byte>>();

            FunNum06CustomerRequestData<T> funNum06CustomerRequestData = (FunNum06CustomerRequestData<T>)customerRequestData;
            List<FunNum06RequestDataBase> funNum06RequestDataBases = funNum06CustomerRequestData.CovertToFunNum06RequestDataBases();

            foreach (FunNum06RequestDataBase item in funNum06RequestDataBases)
            {
                FunNum06RequestDataRTU funNum06RequestDataRTU = new FunNum06RequestDataRTU();
                funNum06RequestDataRTU.FunNum06RequestDataBase = item;

                requesCommandByteStreams.Add(funNum06RequestDataRTU.ToByteList());
            }

            return requesCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamsForFunNum15(ICustomerRequestData customerRequestData)
        {
            List<List<byte>> requesCommandByteStreams = new List<List<byte>>();

            FunNum15CustomerRequestData funNum15CustomerRequestData = (FunNum15CustomerRequestData)customerRequestData;
            List<FunNum15RequestDataBase> funNum15RequestDataBases = funNum15CustomerRequestData.CovertToFunNum15RequestDataBases();

            foreach (FunNum15RequestDataBase item in funNum15RequestDataBases)
            {
                FunNum15RequestDataRTU funNum15RequestDataRTU = new FunNum15RequestDataRTU();
                funNum15RequestDataRTU.FunNum15RequestDataBase = item;

                requesCommandByteStreams.Add(funNum15RequestDataRTU.ToByteList());
            }

            return requesCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamsForFunNum16<T>(ICustomerRequestData customerRequestData)
        {
            List<List<byte>> requesCommandByteStreams = new List<List<byte>>();

            FunNum16CustomerRequestData<T> funNum16CustomerRequestData = (FunNum16CustomerRequestData<T>)customerRequestData;
            List<FunNum16RequestDataBase> funNum16RequestDataBases = funNum16CustomerRequestData.CovertToFunNum16RequestDataBases();

            foreach (FunNum16RequestDataBase item in funNum16RequestDataBases)
            {
                FunNum16RequestDataRTU funNum16RequestDataRTU = new FunNum16RequestDataRTU();
                funNum16RequestDataRTU.FunNum16RequestDataBase = item;

                requesCommandByteStreams.Add(funNum16RequestDataRTU.ToByteList());
            }

            return requesCommandByteStreams;
        }

        #endregion
    }
}
