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
using System.IO.Ports;
using Modbus.Contract;
using System.Text.RegularExpressions;
using Modbus.Common;

namespace ModbusDriver.RTUModel.View
{
    /// <summary>
    /// FunctionNum03View.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionNum03View : UserControl, IFunctionNumView
    {
        public FunctionNum03View()
        {
            InitializeComponent();

            this.functionNum = FunctionNumType.FunctionNum03;
            InitializeTitle();
        }

        public FunctionNum03View(FunctionNumType functionNum)
        {
            InitializeComponent();

            this.functionNum = functionNum;
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

        //public IRequestInfo GetRequestInfo()
        //{
        //    RequestInfo requestInfo = new RequestInfo();

        //    if (VerifyData())
        //    {
        //        try
        //        {
        //            requestInfo.DeviceAddress = Convert.ToByte(this.tbDeviceAddress.Text);
        //            requestInfo.FunctionNum = this.functionNum;
        //            requestInfo.TheByte3and4 = Convert.ToUInt16(this.tbBeginingResAddress.Text);
        //            requestInfo.TheByte5and6 = Convert.ToUInt16(this.tbNumOfResToRead.Text); 
        //        }
        //        catch (Exception)
        //        {
        //            MessageBox.Show("设备地址[0-255] \n寄存器地址[0-65535] \n数量[0-65535]");
        //            return requestInfo = null;
        //        }
        //    }

        //    return requestInfo;
        //}

        #endregion

        private bool VerifyData()
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(this.tbDeviceAddress.Text)
                || string.IsNullOrWhiteSpace(tbBeginingResAddress.Text)
                || string.IsNullOrWhiteSpace(tbNumOfResToRead.Text)
                )
            {
                MessageBox.Show("请填完信息");
                return false;
            }

            if (!NumericString.IsNumeric(this.tbDeviceAddress.Text.Trim())
                || !NumericString.IsNumeric(this.tbBeginingResAddress.Text.Trim())
                || !NumericString.IsNumeric(this.tbNumOfResToRead.Text.Trim()))
            {
                MessageBox.Show("只能输入数字");
                return false;
            }

            return valid;
        }




        public IRequestInfo GetRequestInfo()
        {
            throw new NotImplementedException();
        }
    }
}
