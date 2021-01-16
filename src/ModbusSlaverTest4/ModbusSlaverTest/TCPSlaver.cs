using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace ModbusSlaverTest
{
    class TCPSlaver
    {
        private Thread listenThread;
        private bool m_bListening = false;
        private System.Net.IPAddress MasterIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]; 
        private int masterPort = 502;
        private TcpListener listener;
        private TcpClient client;
        private NetworkStream IOStream;
        private Random radData = new Random();

        public TCPSlaver()
        {
            slaverInit();
            System.IO.File.Delete("123.txt");
        }

        public void Close()
        {
            try
            {
                endResive();
                listener.Stop();
                listenThread.Abort();
            }
            catch 
            {
            }
        }

        private void slaverInit()
        {
            listenThread = new Thread(startListen);
            listener = new TcpListener(masterPort);
            startResive();
        }

        private void startResive()
        {
            if (!m_bListening)
            {
                m_bListening = true;
                if (!listenThread.IsAlive)
                {
                    listenThread.Start();
                }
            }
        }

        private void endResive()
        {
            if (m_bListening)
            {
                m_bListening = false;
                if (listenThread.IsAlive)
                {
                    listenThread.Abort();
                }
            }
        }

        private void startListen()
        {
            listener.Start();
            client = listener.AcceptTcpClient();
            IOStream = client.GetStream();
            IOStream.ReadTimeout = 500;
            IOStream.WriteTimeout = 500;
            //接收数据
            while (m_bListening)
            {
                try
                {
                    //字组处理
                    byte[] bytes = new byte[1026];
                    int bytesread = IOStream.Read(bytes, 0, bytes.Length);
       
                    if (bytesread != 0)
                    {
                        bytesread = modbusDoIt(bytes);
                        IOStream.Write(bytes, 0, bytesread);
                    }
                    else
                    {
                        IOStream.Close();
                        client.Close();
                        client = listener.AcceptTcpClient();
                        IOStream = client.GetStream();
                    }
                }
                catch
                {
                    try
                    {
                        IOStream.Close();
                        client.Close();
                        client = listener.AcceptTcpClient();
                        IOStream = client.GetStream();
                    }
                    catch
                    { 
                    }
                }
            }
            listener.Stop();
        }
        private int modbusDoIt(byte[] bytes)
        {
            int lenth = 0;
            if (bytes[7] == 3)
            { 
                lenth = ((int)bytes[10] * 256 + (int)bytes[11]) * 2;
                //int start = ((int)bytes[8] * 256) + (int)bytes[9];
                bytes[8] = (byte)(lenth); //内容的长度


                for (int i = 0; i < lenth/2; i++)
                {
                    ushort temp;
                    if (i < 2)
                        temp = 500;
                    else
                        temp = (ushort)radData.Next(65536);
                    //ushort temp = Convert.ToUInt16(i);
                    //temp = (UInt16)(i + (int)(start + 40001));
                    bytes[9 + 2 * i] = (byte)(temp / 256);
                    bytes[10 + 2 * i] = (byte)(temp % 256);
                }
            }
            if (bytes[7] == 1)
            {
                lenth = (int)Math.Ceiling((double)(((int)bytes[10] * 256 + (int)bytes[11])) / 8); //要返回的字节数=Ceiling（寄存器的个数/8）


                bytes[8] = (byte)lenth; //wk:根据协议；bytes[8]存储的是响应的字节数，而不是寄存器个数//bytes[8] = (byte)(((int)bytes[10] * 256 + (int)bytes[11]));  //wk：寄存器的个数

                for (int i = 0; i < lenth; i++)
                {
                    byte temp = (byte)radData.Next(256);//0xff;// (byte)radData.Next(256);
                    if (i < 2)
                    {
                        temp = 0x8;
                    }
                    bytes[9 + i] = (byte)(temp);
                }
            }
            return lenth + 9;
        }
    }
}
