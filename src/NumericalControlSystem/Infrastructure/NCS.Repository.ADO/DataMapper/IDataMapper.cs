using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NCS.Model.Entity;

namespace NCS.Repository.ADO.DataMapper
{
    /// <summary>
    /// 数据库表和实体的映射类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataMapper<T>
    {
        //TODO:希望以后能将PropertyMapToTableColumn配置到xml文件中
        /// <summary>
        /// key:属性名
        /// value:数据库表中的字段名
        /// </summary>
        Dictionary<string, string> PropertyMapToTableColumn { get; set; }

        /// <summary>
        /// 将数据库表中的数据转化为T类型实体
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        T ConverFrom(DataRowCollection rows, int i);
    }
}
