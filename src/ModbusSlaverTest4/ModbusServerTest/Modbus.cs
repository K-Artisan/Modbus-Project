using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using ModbusServer;
using UpDataBase.RTWriteProxy;
using System.Diagnostics;

namespace ModbusServer
{
    /*--------------------------------------------------------------------
     *Modbus  --接收整理CmeManager数据，
     *-------------------------------------------------------------------*/
    public class Modbus : UpDataBase.DriverUpdataBase, IDisposable
    {
        private List<CmeManager> _cmeManagerList;
        internal List<CmeManager> CmeManagerList
        {
            get { return _cmeManagerList; }
            set { _cmeManagerList = value; }
        }

        //建立两个字典IDEquipRes[,]---IDModel[, , , ],便于索引
        private Dictionary<ulong, IDModel> _dictionEToM;
        private Dictionary<ulong, IDEquipRes> _dictionMToE;
        /// <summary>
        /// 由ModBus列表创建IDEquipRes[,]---IDModel[, , , ]相互索引
        /// </summary>
        /// <param name="modbusList">ModBus列表</param>
        public void SetDictionaryWithXml(string xmlPath)
        {
            System.Xml.XmlDataDocument xmlDoc = new System.Xml.XmlDataDocument();
            xmlDoc.Load(xmlPath);

            IDEquipRes tmpIDEqu;
            IDModel tmpIDMod;
            int equId;
            int resId;
            int devId;
            int pointId;
            string strType;


            foreach (XmlNode node1 in xmlDoc.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    foreach (XmlNode node3 in node2.ChildNodes)
                    {
                        if (node3.Name.Trim() == "Point")
                        {
                            equId = Convert.ToInt32(node3.Attributes["MachineID"].Value.Trim());
                            resId = Convert.ToInt32(node3.Attributes["RegID"].Value.Trim());
                            devId = Convert.ToInt32(node3.Attributes["DevID"].Value.Trim());
                            pointId = Convert.ToInt32(node3.Attributes["PointID"].Value.Trim());
                            strType = node3.Attributes["Type"].Value.Trim().ToUpper();

                            tmpIDEqu = new IDEquipRes(equId, resId);
                            tmpIDMod = new IDModel(devId, strType, pointId);

                            //IDEquipRes --> IDModel
                            _dictionEToM.Add(tmpIDEqu.ToULongForIndex(), tmpIDMod);

                            //IDModel --> IDEquipRes
                            _dictionMToE.Add(tmpIDMod.ToULongForIndex(), tmpIDEqu);
                        }
                    }
                } //node2
            }//node1
        } //end of SetIndexWithXml

        private Dictionary<int, CmeManager> _dicEquipIdToCmeManager;
        internal Dictionary<int, CmeManager> DicEquipIdToCmeManager
        {
            get { return _dicEquipIdToCmeManager; }
            set { _dicEquipIdToCmeManager = value; }
        }

        const int _MAXUPIDMODELNUMONCE = 1000;   //IDModel每次的上传最大数

        System.Timers.Timer _timerPulse = new System.Timers.Timer(); //定时心率
        System.Timers.Timer _timerInitAllData = new System.Timers.Timer(); //定时初始化清空所有数据

        public Modbus()
        {
            _cmeManagerList = new List<CmeManager>();

            _dictionEToM = new Dictionary<ulong, IDModel>();
            _dictionMToE = new Dictionary<ulong, IDEquipRes>();
            _dicEquipIdToCmeManager = new Dictionary<int, CmeManager>();

            //定时心率
            _timerPulse = new System.Timers.Timer();
            _timerPulse.Interval = 100;
            _timerPulse.Enabled = true;
            _timerPulse.Elapsed += StartPulse;  //(object obj, EventArgs e)
        }

        /// <summary>
        /// 该方法实现了IDisposable的Dispose
        /// </summary>
        public new void Dispose()
        {
            //调用实际执行的清理的、带参数的Dispose方法
            Dispose(true);
        }

        /// <summary>
        /// 调用该共用方法来代替Dispose；
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose();

                _timerPulse.Stop();
                _timerPulse.Close();

                _timerInitAllData.Stop();
                _timerInitAllData.Close();

                foreach (CmeManager cmeM in _cmeManagerList)
                {
                    cmeM.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }


        public void StartModbus(string xmlPath)
        {
            //初始化
            InitWithXml(xmlPath);

            //上传数据
            for (int i = 0; i < _cmeManagerList.Count; ++i)
            {
                //注册OnReadCMEquipmentEvent事件处理程序：上传数据
                _cmeManagerList[i]._OnReadCMEquipmentEvent +=
                     new EventHandler<ModbusServer.CmeManager.ReadCMEEventArgs>(UpdataOnReadCMEquipmentEvent);

                //启动CmeManager
                _cmeManagerList[i].Start(true);
            }

            //定时初始化清空所有数据，
            _timerInitAllData.Interval = 900000;   //15分钟
            _timerInitAllData.Enabled = true;
            _timerInitAllData.Elapsed += InitAllDataToNull;  //(object obj, EventArgs e)
        }

        public void InitWithXml(string xmlPath)
        {
            //从xml中提取所有CmeManager,填充_cmeManagerList；
            SetCmeManagerListWithXml(xmlPath);

            //建立两个字典IDEquipRes[,]---IDModel[, , , ],便于索引
            SetDictionaryWithXml(xmlPath);
        }

        private void InitAllDataToNull(object obj, EventArgs e)
        {
            InitAllData();
        }

        private void StartPulse(object obj, EventArgs e)
        {
            Pulse();
        }

        public void SetCmeManagerListWithXml(string xmlPath)
        {
            System.Xml.XmlDataDocument xmlDoc = new System.Xml.XmlDataDocument();
            xmlDoc.Load(xmlPath);
            int equipId;

            foreach (XmlNode node1 in xmlDoc.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    if (node2.Name == "Equip")
                    {
                        equipId = int.Parse(node2.Attributes["ID"].Value.Trim());

                        CmeManager tmpCmeManager = new CmeManager(equipId, xmlPath);
                        _cmeManagerList.Add(tmpCmeManager);
                        _dicEquipIdToCmeManager.Add(equipId, tmpCmeManager);
                    }
                }
            }
        }

        public void UpdataOnReadCMEquipmentEvent(object obj, ModbusServer.CmeManager.ReadCMEEventArgs newData)
        {
            Debug.WriteLine("Modbus.UpdataOnReadCMEquipmentEvent方法");

            int maxUpdataNum = _MAXUPIDMODELNUMONCE;

            //数据拆分成若干包
            List<List<UpDataBase.RTWriteProxy.PointRTModel>> dataPacketList = SplitDataPacket(newData.Updata, maxUpdataNum);

            for (int i = 0; i < dataPacketList.Count; ++i)
            {
                //分包上传数据
                UsrWrite(dataPacketList[i]);
            }
        }


        /// <summary>
        /// 将数据包拆分成每包countMax个
        /// </summary>
        /// <param name="sourData">原始数据包</param>
        /// <returns>每包countMax个的最大值</returns>
        public List<List<UpDataBase.RTWriteProxy.PointRTModel>> SplitDataPacket(List<UpDataBase.RTWriteProxy.PointRTModel> sourData, int countMax)
        {
            List<List<UpDataBase.RTWriteProxy.PointRTModel>> dataPacketList = new List<List<UpDataBase.RTWriteProxy.PointRTModel>>();
            int coutSourData = sourData.Count;

            if (coutSourData > countMax)//源数据个数大于countMax
            {
                int remainder = coutSourData % countMax;  //余数
                int countSplit = coutSourData / countMax; //拆分的次数

                if (remainder > 0)  //不被countMax的整除
                {
                    countSplit += 1; //不被countMax的整除，多分解一次

                    for (int i = 0; i < countSplit; ++i)
                    {
                        if (i < countSplit - 1)
                        {
                            dataPacketList.Add(sourData.GetRange(i * countMax, countMax));
                        }
                        else if ((countSplit - 1) == i) //最后一趟（不满countMax个）
                        {
                            dataPacketList.Add(sourData.GetRange(i * countMax, remainder));
                        }
                    }
                }
                else if (0 == remainder)  //countMax的整数倍
                {
                    for (int i = 0; i < countSplit; ++i)
                    {
                        dataPacketList.Add(sourData.GetRange(i * countMax, countMax));
                    }
                }

            }
            else //源数据个数小于或等于countMax
            {
                dataPacketList.Add(sourData);
            }

            return dataPacketList;
        }  //SplitDataPacket

        /// <summary>
        /// 向通讯管理机写入的数据（调用WriteDataToCmeAndGetReturnValInCell），并且取得通讯管理机的值
        /// </summary>
        /// <param name="pointRTModel">要向通讯管理机写入的数据</param>
        /// <returns>通讯管理机的值：null 表示没有用取到通许管理机的值</returns>
        private double? GetControlReturnValue(PointRTModel pointRTModel)
        {
            double? returnVal = null;  //null 表示没有用取到通许管理机的值

            int devID = pointRTModel.ID.DevID;
            UpDataBase.RTWriteProxy.MType type = pointRTModel.ID.Type;
            int pointID = pointRTModel.ID.PointID;
            double pointVal = pointRTModel.Value;

            IDModel idModel = new IDModel(devID, type, pointID);
            IDEquipRes idEquipRes = null;  //idModel对应的IDEquipRes

            if (_dictionMToE.ContainsKey(idModel.ToULongForIndex()))   //判断_dictionMToE字典中是否存在idModel
            {
                idEquipRes = _dictionMToE[idModel.ToULongForIndex()];
            }
            else
            {
                return null;  //没有用取到通许管理机的值
            }

            CmeManager cmeMagr = _dicEquipIdToCmeManager[idEquipRes.EquipID]; //根据IDEquipRes找到对用的CmeManager

            DataPoint dataPiont = cmeMagr.CmEquipment.DicResIdToDataPoint[idEquipRes.RegID];  //根据idEquipRes.RegID找到对用的CmeManager上DataPoint
            //新创建的dataPiont的PointValue 默认值为null，不要漏了赋新值pointVal
            dataPiont.PointValue = pointVal;

            returnVal = cmeMagr.WriteDataToCmeAndGetReturnValInCell(dataPiont);

            return returnVal;
        }

        //internal CmeManager CmeManager
        //{
        //    get
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //    set
        //    {
        //    }
        //}


        //---------------------------UpDataBase.DriverUpdataBase-----------------------
        public void UsrWrite(List<UpDataBase.RTWriteProxy.PointRTModel> data)
        {
            base.Write(data);
        }

        //public void UsrInit(List<UpDataBase.RTWriteProxy.PointRTModel> data)
        //{
        //    base.Init(data);
        //}
        public bool SendControlToBusForTest(List<PointRTModel> pointList)
        {
            return SendControlToBus(pointList);
        }

        /// <summary>
        /// 向通许管理机写控制命令
        /// </summary>
        /// <param name="pointList">要发送到控制命令（数据），注意</param>
        /// <returns>true表示写（控制）成功</returns>
        protected override bool SendControlToBus(List<PointRTModel> pointList)
        {
            bool ctrSuccessFlag = false;  //控制成功为true，（PointRTModel.Value == 返回的PointRTModel.Value）

            if (null != pointList && 1 == pointList.Count)    // 来到时候虽然是pointList，但只能写一个
            {
                if (pointList[0].ID.Type == MType.DO)
                {
                    //因为寄存器DO值要么为0，要么为1
                    //05功能码（写DO）时用户输入不合法；将用户输入值转换为：
                    //要么是1--表示开,用户输入值为非零值一律转化为1,
                    //要么是0--表示关

                    if (pointList[0].Value != 0)
                    {
                        pointList[0].Value = 1;
                    }
                }

                double usrInputVal = pointList[0].Value;
                double? returnVal = GetControlReturnValue(pointList[0]);

                if (null == returnVal)  //没有收到通讯管理机的数据
                {
                    return false;
                }
                else  //收到了数据
                {
                    //将写入的数据与写后通讯管理机返回的数据进行对比
                    //相等表示写成功，否则写失败
                    if (usrInputVal == returnVal)
                    {
                        ctrSuccessFlag = true;  //写成功
                    }
                    else
                    {
                        ctrSuccessFlag = false;  //写失败
                    }
                }
            }
            else  //非(1 == pointList.Count)
            {
                ctrSuccessFlag = false;
            }

            return ctrSuccessFlag;
        }

        protected override Guid GetGuid()
        {
            //throw new NotImplementedException();
            return new Guid(2011, 10, 21, 17, 37, 00, 36, 25, 72, 41, 55);
        }



        //---------------------------UpDataBase.DriverUpdataBase-----------------------

        //#region IDisposable 成员

        //public new void Dispose()
        //{
        //    base.Dispose();



        //    System.GC.SuppressFinalize(this);
        //    //throw new NotImplementedException();
        //}

        //#endregion

        protected override void InitAllData()
        {
            if (null != _cmeManagerList)
            {
                for (int i = 0; i < _cmeManagerList.Count; ++i)
                {
                    //for (int k = 0; k < _cmeManagerList[i].CmEquipment.DicResIdToDataPoint.Count; ++k)
                    //{
                    //    _cmeManagerList[i].CmEquipment.DicResIdToDataPoint[k].PointValue = null;
                    //}
                    foreach (int indexRes in _cmeManagerList[i].CmEquipment.DicResIdToDataPoint.Keys)
                    {
                        _cmeManagerList[i].CmEquipment.DicResIdToDataPoint[indexRes].PointValue = null;
                    }
                }
            }

            Debug.WriteLine("\n\n-----------------清空数据！-------------------------\n");
            //throw new NotImplementedException();  
        }
    }
}//Modbus
