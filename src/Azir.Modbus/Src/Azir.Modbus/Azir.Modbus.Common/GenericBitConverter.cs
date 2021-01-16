using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Azir.Modbus.Common
{
    public static class GenericBitConverter
    {
        /// <summary>
        /// 只是对BitConverter类的部分函数进行封装。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes<T>(object value)
        {
            //装箱，性能可能有些问题。不管了，目前只能这样
            object temp = value;

            if (temp is double)
            {
                double valueResult = (double)temp;
                return BitConverter.GetBytes(valueResult);
            }

            if (temp is float)
            {
                float valueResult = (float)temp;
                return BitConverter.GetBytes(valueResult);
            }

            if (temp is int)
            {
                int valueResult = (int)temp;
                return BitConverter.GetBytes(valueResult);
            }

            if (temp is long)
            {
                long valueResult = (long)temp;
                return BitConverter.GetBytes(valueResult);
            }

            if (temp is short)
            {
                short valueResult = (short)temp;
                return BitConverter.GetBytes(valueResult);
            }

            if (temp is uint)
            {
                uint valueResult = (uint)temp;
                return BitConverter.GetBytes(valueResult);
            }

            if (temp is ulong)
            {
                ulong valueResult = (ulong)temp;
                return BitConverter.GetBytes(valueResult);
            }

            if (temp is ushort)
            {
                ushort valueResult = (ushort)temp;
                return BitConverter.GetBytes(valueResult);
            }

            return null;
        }

        /// <summary>
        /// 获取目标类型的所占字节数
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>目标类型的所占字节数</returns>
        public static int GetByteCountOfT<T>()
        {
            int byteCountOfT = Marshal.SizeOf(typeof(T));
            return byteCountOfT;
        }
    }
}
