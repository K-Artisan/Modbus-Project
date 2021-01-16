using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusServer
{
    /*-----------------------------------------------------------------
     *DataPoint --上传数据的数据单元
     *----------------------------------------------------------------*/
    public class DataPoint
    {
        int _equipId;   //数据点所属的通讯机
        public int EquipId
        {
            get { return _equipId; }
            set { _equipId = value; }
        }

        int _resId;     //数据点对应的起始寄存器的id（U32、S32、F32类型的Point占两个寄存器）
        public int ResId
        {
            get { return _resId; }
            set { _resId = value; }
        }
        Type _dataType; //数据点的类型（U16、S16、U32、S32、F32），U16、S16占有一个寄存器值，U32、S32、F32占有两个寄存器的值
        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        int _devId;     //数据库中的通讯机标识,与_equipId不一定值一致（无任何联系）
        public int DevId
        {
            get { return _devId; }
            set { _devId = value; }
        }

        UpDataBase.RTWriteProxy.MType _type; //类型（AI，DI，AO，DO，ACC）
        public UpDataBase.RTWriteProxy.MType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        int _pointId;           //数据点Id
        public int PointId
        {
            get { return _pointId; }
            set { _pointId = value; }
        }

        double? _pointValue;     //数据点的值
        public double? PointValue
        {
            get { return _pointValue; }
            set { _pointValue = value; }
        }

        DateTime _dateTime;     //时间(上传数据的时间)
        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        public DataPoint()
        {
            _equipId = 0;
            _resId = 0;
            _dataType = typeof(UInt16);
            _devId = 0;
            _type = UpDataBase.RTWriteProxy.MType.AI;
            _pointId = 0;
            _pointValue = null;
            _dateTime = DateTime.Now;
        }

        public DataPoint(int equipId, int resId, string  strDataType, int devId, string strType, int pointId)
        {
            _equipId = equipId;
            _resId = resId;
            _devId = devId;
            _pointId = pointId;
            _pointValue = null;
            _dateTime = DateTime.Now;

            switch (strDataType.Trim())
            {
                case "U16":
                    {
                        _dataType = typeof(UInt16);
                        break;
                    }

                case "S16":
                    {
                        _dataType = typeof(Int16);
                        break;
                    }
                case "U32":
                    {
                        _dataType = typeof(UInt32);
                        break;
                    }
                case "S32":
                    {
                        _dataType = typeof(Int32);
                        break;
                    }
                case "F32":
                    {
                        _dataType = typeof(Single);
                        break;
                    }

                case "Bit":
                    {
                        _dataType = typeof(Boolean);
                        break;
                    }
            }

            switch (strType)
            {
                case "AI":
                    {
                        _type = UpDataBase.RTWriteProxy.MType.AI;
                        break;
                    }

                case "DI":
                    {
                        _type = UpDataBase.RTWriteProxy.MType.DI;
                        break;
                    }
                case "AO":
                    {
                        _type = UpDataBase.RTWriteProxy.MType.AO;
                        break;
                    }
                case "DO":
                    {
                        _type = UpDataBase.RTWriteProxy.MType.DO;
                        break;
                    }
                case "ACC":
                    {
                        _type = UpDataBase.RTWriteProxy.MType.ACC;
                        break;
                    }
            }

        }


    }//DataPoint
}
