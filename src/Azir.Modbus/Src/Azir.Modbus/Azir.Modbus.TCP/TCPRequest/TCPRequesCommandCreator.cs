using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum.CustomerRequest;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.TCP.TCPRequest
{
    public static class TCPRequesCommandCreator
    {
        /// <summary>
        /// 将ICustomerRequestData（界面输入的请求信息）转换成TCP请求帧<see cref="FunNumRequestDataTCP"/>（若干个）
        /// 说明：
        /// 一个ICustomerRequestData对应若干个TCP请求帧<see cref="FunNumRequestDataTCP"/>，
        /// 然后返回请求对应的请求字节流List<List<byte>>
        /// </summary>
        /// <typeparam name="T">
        /// T类型只能是如下类型:IFunNumRequestDataBase 接口的实现类
        /// </typeparam>
        /// <param name="customerRequestData">客户端请求信息</param>
        /// <returns></returns>
        public static List<List<byte>> CreateRequestCommandByteStream<T>(ICustomerRequestData<T> customerRequestData)
             where T : IFunNumRequestDataBase
        {
            return CreateRequestCommandByteStreamsFromCustomerRequestData(customerRequestData);;
        }

        #region 从ICustomerRequestData 获取 请求字节流
        /****************************************************************************
         * 这些列算法的核心：
         * 第一步：将ICustomerRequestData分解成若干个IFunNumRequestDataBase；
         * 第二步：将第一步得到的若干个IFunNumRequestDataBase转换成对应数量的
         *         FunNumRequestDataTCP（TCP请求帧）
         * 第三步：将若干个IFunNumRequestDataTCP 转换成对应数量的请求帧字节流
         ****************************************************************************/

        /// <summary>
        /// 从ICustomerRequestData 获取 若干个请求字节流
        /// </summary>
        /// <typeparam name="T">
        /// IFunNumRequestDataBase的实现类
        /// </typeparam>
        /// <param name="customerRequestData">客户端请求信息</param>
        /// <returns>请求字节流List</returns>
        private static List<List<byte>> CreateRequestCommandByteStreamsFromCustomerRequestData<T>(ICustomerRequestData<T> customerRequestData)
            where T : IFunNumRequestDataBase
        {
            List<List<byte>> requesCommandByteStreams = new List<List<byte>>();

            //第一步：将ICustomerRequestData分解成若干个IFunNumRequestDataBase；
            List<T> funNumRequestDataBases = customerRequestData.CovertToFunNumRequestDataBases();

            foreach (var questDataBase in funNumRequestDataBases)
            {
                //第二步：将第一步得到的若干个IFunNumRequestDataBase转换成对应数量的FunNumRequestDataTCP（TCP 请求帧）
                FunNumRequestDataTCP funNumRequestDataRtu = new FunNumRequestDataTCP(questDataBase);

                //第三步：将若干个IFunNumRequestDataTCP 转换成对应数量的请求帧字节流
                requesCommandByteStreams.Add(funNumRequestDataRtu.ToByteList());
            }

            return requesCommandByteStreams;
        }

        #endregion
    }
}
