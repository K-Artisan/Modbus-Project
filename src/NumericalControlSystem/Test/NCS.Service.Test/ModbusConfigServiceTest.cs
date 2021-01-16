using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Configuration;
using NCS.Infrastructure.UnitOfWork;



using NCS.Repository.ADO;
using NCS.Repository.ADO.Repositories;
using NCS.Service.SeviceImplementation;
using NUnit.Framework;
using NCS.Model.Repository;

namespace NCS.Service.Test
{
    [TestFixture]
    public class ModbusConfigServiceTest
    {
        [Test()]
        public void TestReadConfigFormModbusConfigFile()
        {
            ApplicationSettingsFactory.InitializeApplicationSettingsFactory(new AppConfigApplicationSettings());
            IApplicationSettings applicationSettings =
                ApplicationSettingsFactory.GetApplicationSettings();

            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);
            IModuleRepository moduleRepository = new ModuleRepository(unitOfWork);

            ModbusConfigService modbusConfigService = new ModbusConfigService(dataPointRepository,
                moduleRepository);

            modbusConfigService.ReadConfigFormModbusConfigFile();

        }

        [Test()]
        public void TestWriteConfigToDataBase()
        {
            ApplicationSettingsFactory.InitializeApplicationSettingsFactory(new AppConfigApplicationSettings());
            IApplicationSettings applicationSettings =
                ApplicationSettingsFactory.GetApplicationSettings();

            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);
            IModuleRepository moduleRepository = new ModuleRepository(unitOfWork);

            ModbusConfigService modbusConfigService = new ModbusConfigService(dataPointRepository,
                moduleRepository);

            modbusConfigService.ReadConfigFormModbusConfigFile();
           
            modbusConfigService.WriteConfigToDataBase();
        }

        [Test()]
        public void TestBitConverter()
        {
            ushort var1 = 2;
            ushort var2 = 9;

            byte[] byteValuesLow = BitConverter.GetBytes(var1);
            byte[] byteValuesHigh = BitConverter.GetBytes(var2);
            byte[] byteValues = new byte[]
                                    {
                                        byteValuesLow[0],
                                        byteValuesLow[1],
                                        byteValuesHigh[0],
                                        byteValuesHigh[1],

                                    };


            double realTimeValue1 = BitConverter.ToInt32(byteValues, 0);
            //int int32 = (int)realTimeValue1;
            int int32 = Convert.ToInt32(realTimeValue1);
            byte[] byteValuesInt32 = BitConverter.GetBytes(int32);

            double realTimeValue1_1 = BitConverter.ToInt32(byteValuesInt32, 0);
            double realTimeValue1_2 = BitConverter.ToDouble(byteValuesInt32, 0);



            double realTimeValue2 = BitConverter.ToUInt32(byteValues, 0);
            byte[] byteValuesUInt32 = BitConverter.GetBytes(realTimeValue1);
            double realTimeValue2_1 = BitConverter.ToUInt32(byteValuesUInt32, 0);
            double realTimeValue2_2 = BitConverter.ToDouble(byteValuesUInt32, 0);


            //double realTimeValue3 = BitConverter.ToDouble(byteValues, 0);



        }
    }
}
