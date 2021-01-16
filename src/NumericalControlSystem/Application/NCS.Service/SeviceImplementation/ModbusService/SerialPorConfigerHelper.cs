using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Xml;
using NCS.Infrastructure.Configuration;

namespace NCS.Service.SeviceImplementation.ModbusService
{
    public static class SerialPorConfigerHelper
    {
        public static SerialPort GetSerialPortFormConfigFile()
        {
            SerialPort serialPort = new SerialPort();
            string serialPortRelativePath = ApplicationSettingsFactory.GetApplicationSettings().SerialPortConfigFilePath;
            string serialPortConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                                             serialPortRelativePath);
            try
            {
                if (System.IO.File.Exists(serialPortConfigFile))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(serialPortConfigFile);

                    XmlElement portNameElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/PortName");
                    XmlElement baudRateElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/BaudRate");
                    XmlElement parityElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/Parity");
                    XmlElement dataBitsElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/DataBits");
                    XmlElement stopBitsElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/StopBits");
                    XmlElement readTimeoutElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/ReadTimeout");
                    XmlElement writeTimeoutElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/WriteTimeout");

                    string portName = portNameElement.GetAttribute("Value") != null ? portNameElement.GetAttribute("Value") : "COM1";
                    int baudRate = baudRateElement.GetAttribute("Value") != null ? Convert.ToInt32(baudRateElement.GetAttribute("Value")) : 9600;
                    Parity parity = parityElement.GetAttribute("Value") != null ? (Parity)Enum.Parse(typeof(Parity), parityElement.GetAttribute("Value"), true) : Parity.Odd;
                    int dataBits = dataBitsElement.GetAttribute("Value") != null ? Convert.ToInt32(dataBitsElement.GetAttribute("Value")) : 8;
                    StopBits topBits = stopBitsElement.GetAttribute("Value") != null ? (StopBits)Enum.Parse(typeof(StopBits), stopBitsElement.GetAttribute("Value"), true) : StopBits.One;
                    int readTimeout = readTimeoutElement.GetAttribute("Value") != null ? Convert.ToInt32(readTimeoutElement.GetAttribute("Value")) : -1;
                    int writeTimeout = writeTimeoutElement.GetAttribute("Value") != null ? Convert.ToInt32(writeTimeoutElement.GetAttribute("Value")) : -1;

                    serialPort.PortName = portName;
                    serialPort.BaudRate = baudRate;
                    serialPort.Parity = parity;
                    serialPort.DataBits = dataBits;
                    serialPort.StopBits = topBits;
                    serialPort.ReadTimeout = readTimeout;
                    serialPort.WriteTimeout = writeTimeout;
                }
            }
            catch (Exception)
            {
                serialPort.PortName = "COM1";
                serialPort.BaudRate = 9600;
                serialPort.Parity = Parity.Odd;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.ReadTimeout = -1;
                serialPort.WriteTimeout = -1;
            }

            return serialPort;
        }

        public static void SaveSerialPortToConfigFile(SerialPort serialPort)
        {
            string serialPortRelativePath = ApplicationSettingsFactory.GetApplicationSettings().SerialPortConfigFilePath;
            string serialPortConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                                             serialPortRelativePath);

            try
            {
                if (System.IO.File.Exists(serialPortConfigFile))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(serialPortConfigFile);

                    XmlElement portNameElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/PortName");
                    XmlElement baudRateElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/BaudRate");
                    XmlElement parityElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/Parity");
                    XmlElement dataBitsElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/DataBits");
                    XmlElement stopBitsElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/StopBits");
                    XmlElement readTimeoutElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/ReadTimeout");
                    XmlElement writeTimeoutElement = (XmlElement)xmlDoc.SelectSingleNode("/SerialPort/WriteTimeout");

                    portNameElement.SetAttribute("Value", serialPort.PortName);
                    baudRateElement.SetAttribute("Value", Convert.ToString(serialPort.BaudRate));
                    parityElement.SetAttribute("Value", Convert.ToString(serialPort.Parity));
                    dataBitsElement.SetAttribute("Value", Convert.ToString(serialPort.DataBits));
                    stopBitsElement.SetAttribute("Value", Convert.ToString(serialPort.StopBits));
                    readTimeoutElement.SetAttribute("Value", Convert.ToString(serialPort.ReadTimeout));
                    writeTimeoutElement.SetAttribute("Value", Convert.ToString(serialPort.WriteTimeout));

                    xmlDoc.Save(serialPortConfigFile);

                }
            }
            catch (Exception)
            {
                throw new Exception("保存串口配置失败");
            }
        }
    }
}
