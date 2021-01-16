using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ModbusServer
{
    /*--------------------------------------------------------------------
     * CMEquipment类--Communications Management Equipment(通讯管理机,简写：CME)
     *-------------------------------------------------------------------*/
    class CMEquipment
    {
        int _equipId;        //通讯管理机Id
        public int EquipId
        {
            get { return _equipId; }
            set { _equipId = value; }
        }

        string _equipIP;     //通讯管理机IP
        public string EquipIP
        {
            get { return _equipIP; }
            set { _equipIP = value; }
        }

        int _equipPort;      //通讯管理机端口
        public int EquipPort
        {
            get { return _equipPort; }
            set { _equipPort = value; }
        }


        //List<DataPoint> _dataPointList;    //数据点列表
        //internal List<DataPoint> DataPointList
        //{
        //    get { return _dataPointList; }
        //    set { _dataPointList = value; }
        //}


        //List<DataPoint> _aiDataPointList;
        //internal List<DataPoint> AiDataPointList
        //{
        //    get { return _aiDataPointList; }
        //    set { _aiDataPointList = value; }
        //}

        //List<DataPoint> _diDataPointList;
        //internal List<DataPoint> DiDataPointList
        //{
        //    get { return _diDataPointList; }
        //    set { _diDataPointList = value; }
        //}

        //List<DataPoint> _aoDataPointList;
        //internal List<DataPoint> AoDataPointList
        //{
        //    get { return _aoDataPointList; }
        //    set { _aoDataPointList = value; }
        //}

        //List<DataPoint> _doDataPointList;
        //internal List<DataPoint> DoDataPointList1
        //{
        //    get { return _doDataPointList; }
        //    set { _doDataPointList = value; }
        //}

        //List<DataPoint> _accDataPointList;
        //internal List<DataPoint> AccDataPointList
        //{
        //    get { return _accDataPointList; }
        //    set { _accDataPointList = value; }
        //}



        //AI点的xml配置文件中的最后一个寄存器id
        int _xmlEndResAi = 0;

        public int XmlEndResAi
        {
            get { return _xmlEndResAi; }
            set { _xmlEndResAi = value; }
        }

        //DI点的xml配置文件中的最后一个寄存器id
        int _xmlEndResDi = 0;

        public int XmlEndResDi
        {
            get { return _xmlEndResDi; }
            set { _xmlEndResDi = value; }
        }

        //ACC点的xml配置文件中的最后一个寄存器id
        int _xmlEndResAcc = 0;

        public int XmlEndResAcc
        {
            get { return _xmlEndResAcc; }
            set { _xmlEndResAcc = value; }
        }

        //AO点的xml配置文件中的最后一个寄存器id
        int _xmlEndResAo = 0;

        public int XmlEndResAo
        {
            get { return _xmlEndResAo; }
            set { _xmlEndResAo = value; }
        }

        //DO点的xml配置文件中的最后一个寄存器id
        int _xmlEndResDo = 0;

        public int XmlEndResDo
        {
            get { return _xmlEndResDo; }
            set { _xmlEndResDo = value; }
        }


        DataPoint _aiMaxResIdDataPoint = new DataPoint();

        public DataPoint AiMaxResIdDataPoint
        {
            get { return _aiMaxResIdDataPoint; }
            set { _aiMaxResIdDataPoint = value; }
        }
        DataPoint _diMaxResIdDataPoint = new DataPoint();

        public DataPoint DiMaxResIdDataPoint
        {
            get { return _diMaxResIdDataPoint; }
            set { _diMaxResIdDataPoint = value; }
        }
        DataPoint _accMaxResIdDataPoint = new DataPoint();

        public DataPoint AccMaxResIdDataPoint
        {
            get { return _accMaxResIdDataPoint; }
            set { _accMaxResIdDataPoint = value; }
        }
        DataPoint _aoMaxResIdDataPoint = new DataPoint();

        public DataPoint AoMaxResIdDataPoint
        {
            get { return _aoMaxResIdDataPoint; }
            set { _aoMaxResIdDataPoint = value; }
        }
        DataPoint _doMaxResIdDataPoint = new DataPoint();
        public DataPoint DoMaxResIdDataPoint
        {
            get { return _doMaxResIdDataPoint; }
            set { _doMaxResIdDataPoint = value; }
        }
        




        /*
         * _dicResIdToDataPoint 中存在的寄存器id都是永远都是每个DataPoint的起始寄存器id，
         * 而永远不存在任何DataPoint的结束寄存器id，
         * 要检查每个DataPoint的结束寄存器id还要结合每个DataPoint的Type字段（U16?S16?U32?S32?F32?BOOL）
         */
        Dictionary<int, DataPoint> _dicResIdToDataPoint;   // <寄存器id，DataPiont>
        internal Dictionary<int, DataPoint> DicResIdToDataPoint
        {
            get { return _dicResIdToDataPoint; }
            set { _dicResIdToDataPoint = value; }
        }


        private static int _FIRSTACCREGISTERID = 8000;  //第一个表示ACC值的寄存器号
        public static int FIRSTACCREGISTERID
        {
            get { return CMEquipment._FIRSTACCREGISTERID; }
            set { CMEquipment._FIRSTACCREGISTERID = value; }
        }


        public static int _XMLRESIGERIDAI = 40001;

        public static int _XMLRESIGERIDACC = 40001;

        public static int _XMLRESIGERIDDI = 10001;

        public static int _XMLRESIGERIDAO = 80001;

        public static int _XMLRESIGERIDDO = 90001;



        public CMEquipment(int equipId, string xmlPath)
        {
            
            _dicResIdToDataPoint = new Dictionary<int, DataPoint>();

            //_dataPointList = new List<DataPoint>();
            //_aiDataPointList = new List<DataPoint>();
            //_diDataPointList = new List<DataPoint>();
            //_aoDataPointList = new List<DataPoint>();
            //_doDataPointList = new List<DataPoint>();
            //_accDataPointList = new List<DataPoint>();
            
            InitCMEquipmentWithXml(equipId, xmlPath);
        }


        public void InitCMEquipmentWithXml(int equipId, string xmlPath)
        {


            List<DataPoint>  dataPointList = new List<DataPoint>();
            List<DataPoint>  aiDataPointList = new List<DataPoint>();
            List<DataPoint>  diDataPointList = new List<DataPoint>();
            List<DataPoint>  aoDataPointList = new List<DataPoint>();
            List<DataPoint>  doDataPointList = new List<DataPoint>();
            List<DataPoint>  accDataPointList = new List<DataPoint>();

            System.Xml.XmlDataDocument xmlDoc = new System.Xml.XmlDataDocument();
            xmlDoc.Load(xmlPath);

            foreach (XmlNode node1 in xmlDoc.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    //设置通讯管理机id
                    _equipId = equipId;

                    //通讯管理机IP和端口
                    if (node2.Name.Trim() == "Equip" && node2.Attributes["ID"].Value.Trim() == Convert.ToString(equipId))
                    {
                        _equipIP = node2.Attributes["IP"].Value.Trim();
                        _equipPort = Convert.ToInt32(node2.Attributes["Port"].Value.Trim());

                        int resId;
                        string strDataType;
                        int devId;
                        int pointId;
                        string strType;
                        DataPoint tmpDataPoint;

                        foreach (XmlNode node3 in node2.ChildNodes)
                        {
                            //设置所有其数据点
                            if (node3.Name.Trim() == "Point" && node3.Attributes["MachineID"].Value.Trim() == Convert.ToString(equipId))
                            {
                                resId = Convert.ToInt32(node3.Attributes["RegID"].Value.Trim());
                                strDataType = node3.Attributes["DataType"].Value.Trim();
                                devId = Convert.ToInt32(node3.Attributes["DevID"].Value.Trim());
                                pointId = Convert.ToInt32(node3.Attributes["PointID"].Value.Trim());
                                strType = node3.Attributes["Type"].Value.Trim();

                                tmpDataPoint = new DataPoint(equipId, resId, strDataType, devId, strType, pointId);
                               
                                //_dataPointList.Add(tmpDataPoint);
                                _dicResIdToDataPoint.Add(resId, tmpDataPoint);

                                //封装不同类型数据的DataPoint
                                switch (strType)
                                {
                                    case "AI":
                                        {
                                            aiDataPointList.Add(tmpDataPoint);
                                            break;
                                        }

                                    case "DI":
                                        {
                                            diDataPointList.Add(tmpDataPoint);
                                            break;
                                        }
                                    case "AO":
                                        {
                                            aoDataPointList.Add(tmpDataPoint);
                                            break;
                                        }
                                    case "DO":
                                        {
                                            doDataPointList.Add(tmpDataPoint);
                                            break;
                                        }
                                    case "ACC":
                                        {
                                            accDataPointList.Add(tmpDataPoint);
                                            break;
                                        }
                                } //switch
                            } //if
                        } //node3
                    } //if
                }//node2
            }  //node1

            //提取各种类型在xml配置文件中最后一个寄存器对应的点
            _aiMaxResIdDataPoint = aiDataPointList.OrderByDescending(i => i.ResId).First();
            _diMaxResIdDataPoint = diDataPointList.OrderByDescending(i => i.ResId).First();
            _accMaxResIdDataPoint = accDataPointList.OrderByDescending(i => i.ResId).First();
            _aoMaxResIdDataPoint = aoDataPointList.OrderByDescending(i => i.ResId).First();
            _doMaxResIdDataPoint = doDataPointList.OrderByDescending(i => i.ResId).First();

            _xmlEndResAi = SetLastReadXmlResId(_aiMaxResIdDataPoint);
            _xmlEndResDi = SetLastReadXmlResId(_diMaxResIdDataPoint);
            _xmlEndResAcc = SetLastReadXmlResId(_accMaxResIdDataPoint);
            _xmlEndResAo = SetLastReadXmlResId(_aoMaxResIdDataPoint);
            _xmlEndResDo = SetLastReadXmlResId(_doMaxResIdDataPoint);
       
        }


        private int SetLastReadXmlResId(DataPoint dataPoint)
        {
            int lastReadXmlResId = 0;
            Type dataType = dataPoint.DataType;

            if (typeof(UInt16) == dataType || typeof(Int16) == dataType || typeof(Boolean) == dataType)
            {
                lastReadXmlResId = dataPoint.ResId;
            }
            else if (typeof(UInt32) == dataType || typeof(Int32) == dataType || typeof(Single) == dataType)
            {
                lastReadXmlResId = dataPoint.ResId +1;
            }
            else
            {
                lastReadXmlResId = 0;
            }

            return lastReadXmlResId;
        }


        internal DataPoint DataPoint
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        } //InitCMEquipmentWithXml

    }//地方
}
