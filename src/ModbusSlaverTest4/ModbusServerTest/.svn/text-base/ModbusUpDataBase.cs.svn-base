using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;


namespace ModbusServer
{   
    /// <summary>
    /// 上传ModBus数据
    /// </summary>
    class ModbusUpDataBase : UpDataBase.DriverUpdataBase
    {
        IList<TCPModBusServer> modbusList = new List<TCPModBusServer>(); //需要上传数据的ModBus列表
        internal IList<TCPModBusServer> ModbusList
        {
            get { return modbusList; }
            set { modbusList = value; }
        }

        /// <summary>
        /// 用xml文件初始化modbusList字段
        /// </summary>
        /// <param name="xmlPath">xml文件地址</param>
        public void SetModbusListWithXml(string xmlPath)
        {
            System.Xml.XmlDataDocument xmlDoc = new System.Xml.XmlDataDocument();
            xmlDoc.Load(xmlPath);

            foreach (XmlNode node1 in xmlDoc.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    if (node2.Name == "Equip")
                    {
                        int modId = int.Parse(node2.Attributes["ID"].Value);
                        modbusList.Add(new TCPModBusServer(modId, xmlPath));
                    }
                }
            }
        }//end of SetModbusListWithXml

        //建立两个字典IDEquipRes[,]---IDModel[, , , ],便于索引
        private Dictionary<ulong, IDModel> dictionEToM = new Dictionary<ulong, IDModel>();
        private Dictionary<ulong, IDEquipRes> dictionMToE = new Dictionary<ulong, IDEquipRes>();
        /// <summary>
        /// 由ModBus列表创建IDEquipRes[,]---IDModel[, , , ]相互索引
        /// </summary>
        /// <param name="modbusList">ModBus列表</param>
        public void SetDictionaryWithXml(string xmlPath)
        {
            System.Xml.XmlDataDocument xmlDoc = new System.Xml.XmlDataDocument();
            xmlDoc.Load(xmlPath);

            IDEquipRes tmpIDEqu;
            IDModel    tmpIDMod;

            foreach (XmlNode node1 in xmlDoc.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    if (node2.Name == "Point")
                    {
                        int equId = Convert.ToInt32(node2.Attributes["MachineID"].Value);
                        int resId = Convert.ToInt32(node2.Attributes["RegID"].Value);
                        int devId = Convert.ToInt32(node2.Attributes["DevID"].Value);
                        int pointId = Convert.ToInt32(node2.Attributes["PointID"].Value);
                        string strType = node2.Attributes["Type"].Value.Trim();

                        tmpIDEqu = new IDEquipRes(equId, resId);
                        tmpIDMod = new IDModel(devId, strType, pointId);

                        //IDEquipRes --> IDModel
                        dictionEToM.Add(tmpIDEqu.ToULongForIndex(), tmpIDMod);

                        //IDModel --> IDEquipRes
                        dictionMToE.Add(tmpIDMod.ToULongForIndex(), tmpIDEqu);
                    }
                }
            } 
        } //end of SetIndexWithXml

        
        /// <summary>
        /// 从ModBus列表中提取数据
        /// </summary>
        /// <param name="modbusList">ModBus列表</param>
        /// <returns>数据</returns>
        public List<UpDataBase.RTWriteProxy.PointRTModel> GetDataWithModbusList(IList<TCPModBusServer> modbusList)
        {
            List<UpDataBase.RTWriteProxy.PointRTModel> data = new List<UpDataBase.RTWriteProxy.PointRTModel>();

            // 从ModBus列表中提取数据
            for (int indexMod = 0; indexMod < modbusList.Count; ++indexMod)  //Modbus个数
            {
                for (int indexRes = 0; indexRes < modbusList[indexMod].RegisterList.Count; ++indexRes) //遍历每个Modbus中寄存器
                {
                    data.Add(new UpDataBase.RTWriteProxy.PointRTModel()
                             {
                                 ID = new UpDataBase.RTWriteProxy.IDModel()
                                          {
                                              DevID = modbusList[indexMod].RegisterList[indexRes].DevId,
                                              Type = modbusList[indexMod].RegisterList[indexRes].Type,
                                              PointID = modbusList[indexMod].RegisterList[indexRes].PointId,
                                          },
                                 Value = modbusList[indexMod].RegisterList[indexRes].ResValue,
                                 Time = DateTime.Now,
                             }
                        );
                } //end inside for 
            } //end outside for

            return data;
        } // end of GetDataWithModbusList

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="obj">事件源</param>
        /// <param name="newData">事件源上传的数据</param>
        public void UpdataOnRegisterValueChangeEvent(object obj, UpDataEventArgs newData)
        {
            Debug.WriteLine("UpdataOnRegisterValueChangeEvent方法");

            const int maxUpdata = 2;
            //数据拆分成若干包
            List<List<UpDataBase.RTWriteProxy.PointRTModel>> dataPacketList = SplitDataPacket(newData.Updata, maxUpdata);

            for (int i = 0; i < dataPacketList.Count; ++i)
            {
                //分包上传数据
                UsrWrite(dataPacketList[i]);
            }
            //Debug.WriteLine("count={0},object={1}:", count, obj);
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
        }







        //---------------------------UpDataBase.DriverUpdataBase-----------------------
        public void UsrWrite(List<UpDataBase.RTWriteProxy.PointRTModel> data)
        {
            base.Write(data);
        }

        public void UsrInit(List<UpDataBase.RTWriteProxy.PointRTModel> data)
        {
            //foreach (PointRTModel p in points)
            //{
            //    data.Add();
            //}
            base.Init(data);
        }

        public override bool SendControlToBus(List<UpDataBase.RTWriteProxy.PointRTModel> pointList)
        {
            return false;
        }
        //---------------------------UpDataBase.DriverUpdataBase-----------------------

        public override Guid GetGuid()
        {
            //throw new NotImplementedException();
            return new Guid(2011,10,21,17,37,00,36,25,72,41,55);
        }
    }
}
