using System.Collections.Generic;
using NUnit.Framework;
using Modbus.Contract.RequestDataBase;
using Modbus.RTU;
using Modbus.Contract;

namespace Modbus.Test
{
    [TestFixture]
    public class ModbusContractTest
    {
        [Test()]
        public void CreateRequestCommandByteStreamForFunNum15()
        {

            FunNum15CustomerRequestData customerRequestData = new FunNum15CustomerRequestData();
            customerRequestData.DeviceAddress = 1;
            customerRequestData.FunctionNum = FunctionNumType.FunctionNum15;
            customerRequestData.StartingCoilAddress = 0;
            //customerRequestData.NumOfCoilToForce = 20;
            customerRequestData.ForceData = new List<bool>()
            { 
                true, false, true, false, true, false, true, false, true, false, 
                true, false, true, false, true, false, true, false, true, false, 
            };

            List<List<byte>> rtuRequesCmdByteStreams = RTURequesCommandCreator.CreateRequestCommandByteStream<bool>(customerRequestData);
        }

        [Test()]
        public void CreateRequestCommandByteStreamForFunNum06()
        {
            FunNum06CustomerRequestData<int> customerRequestData = new FunNum06CustomerRequestData<int>();
            customerRequestData.DeviceAddress = 1;
            customerRequestData.FunctionNum =FunctionNumType.FunctionNum06;
            customerRequestData.RegisterAddress = 1;
            customerRequestData.PresetData = 65535+1;

            List<List<byte>> rtuRequesCmdByteStreams = RTURequesCommandCreator.CreateRequestCommandByteStream<int>(customerRequestData);
            int countSplitOfcustomerRequestData = 2;
            Assert.True(countSplitOfcustomerRequestData == rtuRequesCmdByteStreams.Count);
        }

        [Test()]
        public void CreateRequestCommandByteStreamForFunNum16()
        {

            FunNum16CustomerRequestData<int> customerRequestData = new FunNum16CustomerRequestData<int>();
            customerRequestData.DeviceAddress = 1;
            customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
            customerRequestData.StartingRegisterAddress = 0;
            customerRequestData.PresetData = new List<int>()
            { 
                1, 2, 3, 4 , 5 , 6 , 7, 8, 9 , 10 , 11, 12,
                13, 14, 15, 16 ,17 ,18, 19 , 20, 21
            };

            List<List<byte>> rtuRequesCmdByteStreams = RTURequesCommandCreator.CreateRequestCommandByteStream<int>(customerRequestData);

            int countSplitOfcustomerRequestData = 1;
            Assert.True(countSplitOfcustomerRequestData == rtuRequesCmdByteStreams.Count);
        }

        [Test()]
        public void CreateRequestCommandByteStreamForFunNum16ForDouble()
        {

            FunNum16CustomerRequestData<double> customerRequestData = new FunNum16CustomerRequestData<double>();
            customerRequestData.DeviceAddress = 1;
            customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
            customerRequestData.StartingRegisterAddress = 0;
            customerRequestData.PresetData = new List<double>()
            { 
                1, 2, 3, 4 , 5 , 6 , 7, 8, 9 , 10 , 11, 12,
                13, 14, 15, 16 ,17 ,18, 19 , 20, 21
            };

            List<List<byte>> rtuRequesCmdByteStreams = RTURequesCommandCreator.CreateRequestCommandByteStream<double>(customerRequestData);

            int countSplitOfcustomerRequestData = 1;
            Assert.True(countSplitOfcustomerRequestData == rtuRequesCmdByteStreams.Count);
        }
    }
}
