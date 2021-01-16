using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.UnitOfWork;

using NCS.Repository.ADO;
using NCS.Repository.ADO.Repositories;
using NCS.Service.Helper;
using NCS.Service.SeviceImplementation.ModbusService;
using NUnit.Framework;
using NCS.Model.Repository;
using NCS.Model.Entity;

namespace NCS.Service.Test
{
    [TestFixture]
    public class ModbusServiceTest
    {
        [Test()]
        public void TestGroupingDataPointsForReadRegister()
        {
            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);

            List<DataPoint> allDataPoints = dataPointRepository.FindAll().ToList();

            int dataPointCount = 0;
            List<List<DataPoint>> dataPointsGroupedForRead = 
                DataPointGrouper.GroupingDataPointsForReadRegister(allDataPoints);

            foreach (var groud in dataPointsGroupedForRead)
            {
                dataPointCount += groud.Count;
            }
            Assert.IsTrue(allDataPoints.Count == dataPointCount);
        }

        [Test()]
        public void TestGroupingDataPointsForWriteRegister()
        {
            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);
    
            List<DataPoint> allDataPoints = dataPointRepository.FindAll().ToList();

            int dataPointCount = 0;
            List<List<DataPoint>> dataPointsGroupedForWrite =
                DataPointGrouper.GroupingDataPointsForWriteRegister(allDataPoints);

            foreach (var groud in dataPointsGroupedForWrite)
            {
                dataPointCount += groud.Count;
            }
            Assert.IsTrue(allDataPoints.Count == dataPointCount);
        }

        [Test()]
        public void TestGetCurrentTimeUntilNextHourInterval()
        {
            double inr = DateTimeHelper.GetCurrentTimeUntilNextHourInterval();

            List<byte> textBytes1 = new List<byte>();

            textBytes1.Add((byte)0x01);
            textBytes1.Add((byte)0x02);
            byte[] textBytes1ABytes = textBytes1.ToArray();
            UInt16 ushort16 = BitConverter.ToUInt16(textBytes1ABytes, 0); //513
            byte[] textBytes1ABytes2 = BitConverter.GetBytes(ushort16);

            List<byte> textBytes2 = new List<byte>();

            textBytes2.Add((byte)0x01);
            textBytes2.Add((byte)0x02);
            textBytes2.Add((byte)0x03);
            textBytes2.Add((byte)0x04);
            byte[] textBytes2ABytes = textBytes2.ToArray();

            uint uInt32 = BitConverter.ToUInt32(textBytes2ABytes, 0); //67305985
            byte[] textBytes2ABytes2 = BitConverter.GetBytes(uInt32);


        }

    }
}
