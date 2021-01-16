using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Modbus.Contract;
using Modbus.Common;

namespace ModbusDriver.RTUModel.View
{
    /// <summary>
    /// FunctionNumView06.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionNum06View : UserControl, IFunctionNumView
    {
        public FunctionNum06View()
        {
            InitializeComponent();

            this.functionNum = FunctionNumType.FunctionNum06;
            InitializeTitle();
        }

        private void InitializeTitle()
        {
            this.txTitle.Text = Convert.ToString(functionNum);
        }

        #region IFunctionNumView member

        public FunctionNumType functionNum
        {
            get;
            set;
        }

        public IRequestInfo GetRequestInfo()
        {
            RequestInfo requestInfo = new RequestInfo();

            //if (VerifyData())
            //{
            //    string deviceAddress = this.tbDeviceAddress.Text.Trim();
            //    string registAddress = this.tbRegistAddress.Text.Trim();
            //    string valueOfResToSet = this.tbValueOfResToSet.Text.Trim();

            //    try
            //    {
            //        byte ushortdeviceAddress = Convert.ToByte(deviceAddress);
            //        ushort ushortregistAddress = Convert.ToUInt16(registAddress);
            //        ushort ushortvalueOfResToSet = Convert.ToUInt16(valueOfResToSet);
                    
            //        //requestInfo.FunctionNum = this.functionNum;
            //        //requestInfo.DeviceAddress = ushortdeviceAddress;
            //        //requestInfo.TheByte3and4 = ushortregistAddress;
            //        //requestInfo.TheByte5and6 = ushortvalueOfResToSet;
            //    }
            //    catch (Exception)
            //    {
            //        MessageBox.Show("设备地址[0-255] \n寄存器地址[0-65535] \n设置值[0-65535]");
            //        return requestInfo = null;
            //    }
            //}

            return requestInfo;
        }

        #endregion 

        private bool VerifyData()
        {
            bool valid = true;

            string deviceAddress = this.tbDeviceAddress.Text.Trim();
            string registAddress = this.tbRegistAddress.Text.Trim();
            string valueOfResToSet = this.tbValueOfResToSet.Text.Trim();

            if (string.IsNullOrWhiteSpace(deviceAddress)
                || string.IsNullOrWhiteSpace(registAddress)
                || string.IsNullOrWhiteSpace(valueOfResToSet)
                )
            {
                MessageBox.Show("请填完信息");
                return false;
            }

            if (!NumericString.IsNumeric(deviceAddress)
                || !NumericString.IsNumeric(registAddress)
                || !NumericString.IsNumeric(valueOfResToSet))
            {
                MessageBox.Show("只能输入数字");
                return false;
            }

            return valid;
        }
    }
}
