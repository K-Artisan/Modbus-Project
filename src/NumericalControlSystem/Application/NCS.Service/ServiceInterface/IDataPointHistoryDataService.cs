using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.Messaging.DataPointHistoryDataService;
using NCS.Service.Messaging.DataPointService;

namespace NCS.Service.ServiceInterface
{
    public interface IDataPointHistoryDataService
    {
        /// <summary>
        /// 添加历史数据
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        AddDataPointHistoryDataResponse AddDataPointHistoryData(AddDataPointHistoryDataRequst requst);

        /// <summary>
        /// 删除历史数据
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        DeleteDataPointHistoryDataResponse DeleteDataPointHistoryData(DeleteDataPointHistoryDataRequst requst);

        /// <summary>
        /// 获取指定的DataPiont的历史数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetDataPiontHistoryDataResponse GetDataPiontHistoryData(GetDataPointHistoryDataRequest request);

        /// <summary>
        /// 获取所有DataPoint的历史数据
        /// </summary>
        /// <returns></returns>
        GetAllDataPointsHistoryDataResponse GetAllDataPointsHistoryData();

    }
}
