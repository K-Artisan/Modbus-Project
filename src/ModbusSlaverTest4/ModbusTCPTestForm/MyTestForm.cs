using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ModbusServer;
using UpDataBase.RTWriteProxy;

namespace ModbusTCPTestForm
{
    public partial class MyTestForm : Form
    {
        Modbus modbus;
        public MyTestForm()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            modbus = new Modbus();
            //modbus.StartModbus("..\\modbusConfig.xml");
            modbus.StartModbus(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ModbusTCPCfg.xml"));
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (radioButtonAO.Checked == true)
            {
                List<PointRTModel> list = GetSendPoint(MType.AO);
                if (list != null)
                {
                    try
                    {
                        if (modbus.SendControlToBusForTest(list))
                        {
                            MessageBox.Show("控制成功");
                        }
                        else
                        {
                            MessageBox.Show("控制失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if(radioButtonDO.Checked == true)
            {
                List<PointRTModel> list = GetSendPoint(MType.DO);
                if (list != null)
                {
                    try
                    {
                        if (modbus.SendControlToBusForTest(list))
                        {
                            MessageBox.Show("控制成功");
                        }
                        else
                        {
                            MessageBox.Show("控制失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private List<PointRTModel> GetSendPoint(MType type)
        {
            try
            {
                PointRTModel point = new PointRTModel();
                point.ID = new IDModel()
                {
                    DevID = Convert.ToInt32(textBoxDevID.Text),
                    Type = type,
                    PointID = Convert.ToInt32(textBoxPointID.Text)
                };
                point.Value = Convert.ToSingle(textBoxValue.Text);
                List<PointRTModel> list = new List<PointRTModel>();
                list.Add(point);
                return list;
            }
            catch
            {
                return null;
            }
        }

        private void MyTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (modbus is IDisposable)
            {
                (modbus as IDisposable).Dispose();
            }
        }
    }
}
