using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.Messaging.DataPointService;

namespace NCS.Service.ServiceInterface
{
    public interface IDataPointService
    {
        /// <summary>
        /// 添加数据点
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        AddDataPointResponse AddDataPoint(AddDataPointRequst requst);

        /// <summary>
        /// 获取特定的DataPointInfo（数据点信息）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetDataPointInfoResponse GetDataPointInfo(GetDataPointInfoRequest request);

        /// <summary>
        /// 获取所有的DataPointInfo（数据点信息）
        /// </summary>
        /// <returns></returns>
        GetAllDataPointsInfoResponse GetAllDataPointInfo();

        /// <summary>
        /// 获取隶属指定Module的DataPointInfo（数据点信息）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetDataPointByModuleResponse GetDataPointInfoByModule(GetDataPointByModuleRequest request);
    }
}
