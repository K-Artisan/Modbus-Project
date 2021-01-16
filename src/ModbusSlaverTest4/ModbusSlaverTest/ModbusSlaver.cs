using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModbusSlaverTest
{
    public partial class ModbusSlaver : Form
    {
        TCPSlaver slaver;
        public ModbusSlaver()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            slaver = new TCPSlaver();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            slaver.Close();
        }

        private void ModbusSlaver_Load(object sender, EventArgs e)
        {
        }
    }
}
