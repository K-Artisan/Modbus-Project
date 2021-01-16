using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Azir.ModbusServer.TCP.Socket
{
    /// <summary>
    /// ModbusTCP 服务器
    /// </summary>
    public class SockeHelper : IDisposable
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public TcpClient TcpClient { get; set; }

        #region ctor

        public SockeHelper(string ip, int port)
        {
            IpAddress = IPAddress.Parse(ip);
            Port = port;
            TcpClient = new TcpClient();
        }

        #endregion
        
        public bool Connect()
        {
            bool success = true;
            try
            {
                TcpClient.Connect(IpAddress, Port);
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }

        public bool IsConnect()
        {
            if (TcpClient != null)
            {
                return TcpClient.Connected;
            }
            else
            {
                return false;
            }
        }

        public bool TryReConnect()
        {
            if (!IsConnect())
            {
                TcpClient = new TcpClient();
                return Connect();
            }
            else
            {
                return true;
            }
        }

        public List<byte> Send(List<byte> sendBytes)
        {
            List<byte> receviceBytes = null;
            NetworkStream netIOStream = null;
            try
            {
                if (!IsConnect())
                {
                    //恢复中途某时间段链接失败
                    TryReConnect();
                }

                if (!IsConnect())
                {
                    return null;
                }

                //网络传输流
                netIOStream = TcpClient.GetStream();
                netIOStream.ReadTimeout = 1000;
                netIOStream.WriteTimeout = 1000;

                if (netIOStream.CanWrite)
                {
                    netIOStream.Write(sendBytes.ToArray(), 0, sendBytes.Count);
                }

                if (netIOStream.CanRead)
                {
                    byte[] recvdata = new byte[1026];
                    int recvLen = netIOStream.Read(recvdata, 0, recvdata.Length);

                    receviceBytes = recvdata.ToList();
                }
            }
            catch (Exception ex)
            {
                receviceBytes = null;
                CloseNetIOStream(netIOStream);
            }
            finally
            {
            }
            return receviceBytes;
        }

        public void Stop()
        {
            CloseTcpClient(TcpClient);
        }

        #region Dispose
        public void Dispose()
        {
            CloseTcpClient(TcpClient);
        }

        private void CloseTcpClient(TcpClient tcpClient)
        {
            if (null != tcpClient)
            {
                try
                {
                    tcpClient.Close();
                }
                catch (Exception e)
                {
                }
            }
        }
        private void CloseNetIOStream(NetworkStream netIOStream)
        {
            if (null != netIOStream)
            {
                try
                {
                    netIOStream.Close();
                    netIOStream.Dispose();

                }
                catch (Exception e)
                {
                }
            }
        }
        #endregion

    }
}
