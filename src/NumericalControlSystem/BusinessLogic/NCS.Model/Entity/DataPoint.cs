using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;

namespace NCS.Model.Entity
{
    /// <summary>
    /// 数据点
    /// </summary>
    public partial class DataPoint : EntityBase<int>, IAggregateRoot
    {
        /// <summary>               
        /// 数据点编号（必须唯一）
        /// </summary>
        public virtual int Number
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 设备（主机）地址
        /// 取值范围是一个字节：0-127
        /// </summary>
        public int DeviceAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 数据点对应的寄存器的起始寄存器
        /// </summary>
        public virtual int StartRegisterAddress
        {
            get;
            set;
        }
        /// <summary>
        /// 数据点的数据类型
        /// </summary>
        public virtual DataType DataType
        {
            get;
            set;
        }


        /// <summary>
        /// 数据点的类型
        /// </summary>
        public virtual DataPointType DataPointType
        {
            get;
            set;

        }

        /// <summary>
        /// 数据点的相关描述
        /// </summary>
        public virtual string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 所属于的模块
        /// </summary>
        public Module ModuleBelongTo
        {
            get;
            set;
        }

        ///// <summary>
        ///// 数据点的历史数据
        ///// </summary>
        //public virtual IList<NCS.Model.DataPointHistoryData.DataPointHistoryData> DataPointHistoryDatas
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 数据点的实时数据
        /// </summary>
        public double RealTimeValue
        {
            get;
            set;
        }

        public double ValueToSet
        {
            get;
            set;
        }

        #region EntityBase members

        protected override void Validate()
        {
        }

        #endregion

        
    }
}
