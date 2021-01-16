using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Logging;
using NCS.Infrastructure.Querying;
using NCS.Infrastructure.UnitOfWork;



using NCS.Service.Helper;
using NCS.Service.Mapping;
using NCS.Service.Messaging.DataPointHistoryDataService;
using NCS.Service.Messaging.DataPointService;
using NCS.Service.ServiceInterface;
using NCS.Model.Repository;
using NCS.Model.Entity;

namespace NCS.Service.SeviceImplementation
{
    public class DataPointHistoryDataService : IDataPointHistoryDataService
    {
        private readonly IDataPointHistoryDataRepository dataPointHistoryDataRepository;
        //private readonly IDataPointRepository _dataPointRepository;
        //private readonly IModuleRepository _moduleRepository;
        private readonly IUnitOfWork unitOfWork;

        private System.Timers.Timer delectDataPointHistoryValueTimer = null; //定时删除为历史数据
        private readonly object delectDataPointHistoryValueLock = new object();

        public DataPointHistoryDataService(IDataPointHistoryDataRepository dataPointHistoryDataRepository,
            //IDataPointRepository dataPointRepository,
            //IModuleRepository moduleRepository,
            IUnitOfWork unitOfWork)
        {
            this.dataPointHistoryDataRepository = dataPointHistoryDataRepository;
            //_dataPointRepository = dataPointRepository;
            //_moduleRepository = moduleRepository;
            this.unitOfWork = unitOfWork;

            InitializeTimer();
        }


        #region 处理历史数据：定时删除

        private void InitializeTimer()
        {
            this.delectDataPointHistoryValueTimer = new System.Timers.Timer();
            this.delectDataPointHistoryValueTimer.Interval = 1000 * 60;
            this.delectDataPointHistoryValueTimer.Enabled = true;
            this.delectDataPointHistoryValueTimer.AutoReset = true;
            this.delectDataPointHistoryValueTimer.Elapsed += DelecteDataPointHistoryValue;
            this.delectDataPointHistoryValueTimer.Start();
        }

        private void DelecteDataPointHistoryValue(object obj, EventArgs e)
        {
            lock (this.delectDataPointHistoryValueLock)
            {
                this.delectDataPointHistoryValueTimer.Interval = DateTimeHelper.GetCurrentTimeUntilNextHourInterval();
                DelecteDataPointHistoryValueBefore(7);
            }
        }

        public void DelecteDataPointHistoryValueBefore(int dayAgo)
        {
            if (null != this.dataPointHistoryDataRepository)
            {
                try
                {
                    Query query = new Query();
                    query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, DateTime.Now.AddDays(-dayAgo), CriteriaOperator.LessThan));
                    query.OrderByProperty = OrderByClause.Create<DataPointHistoryData>(p => p.DateTime, false);

                    this.dataPointHistoryDataRepository.Remove(query);

                    var uwRepository = this.dataPointHistoryDataRepository as IUnitOfWorkRepository;
                    if (uwRepository != null)
                    {
                        uwRepository.UnitOfWork.Commit();
                    }

                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        #endregion

        #region IDataPointHistoryDataService members

        public AddDataPointHistoryDataResponse AddDataPointHistoryData(AddDataPointHistoryDataRequst requst)
        {
            AddDataPointHistoryDataResponse response = new AddDataPointHistoryDataResponse();

            try
            {
                foreach (DataPointHistoryData dataPointHistoryData in requst.DataPointHistoryDatasToAdd)
                {
                    dataPointHistoryDataRepository.Add(dataPointHistoryData);
                }

                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                string message = "添加失败!错误信息:/n" + ex.Message;
                response = new AddDataPointHistoryDataResponse()
                {
                    ResponseSucceed = false,
                    Message = "添加失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

        public DeleteDataPointHistoryDataResponse DeleteDataPointHistoryData(DeleteDataPointHistoryDataRequst requst)
        {
            DeleteDataPointHistoryDataResponse response = new DeleteDataPointHistoryDataResponse();

            try
            {
                Query query = new Query();
                query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, requst.BeginTime, CriteriaOperator.GreaterThanOrEqual));
                query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, requst.EndTime, CriteriaOperator.LesserThanOrEqual));
                query.QueryOperator = QueryOperator.And;

                dataPointHistoryDataRepository.Remove(query);

                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                string message = "删除失败!错误信息:/n" + ex.Message;
                response = new DeleteDataPointHistoryDataResponse()
                {
                    ResponseSucceed = false,
                    Message = "删除失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

        public GetDataPiontHistoryDataResponse GetDataPiontHistoryData(GetDataPointHistoryDataRequest request)
        {
            GetDataPiontHistoryDataResponse response = new GetDataPiontHistoryDataResponse();

            try
            {
                Query query = new Query();
                query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DataPoint.Id, request.DataPointId, CriteriaOperator.Equal));
                query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, request.BeginTime, CriteriaOperator.GreaterThanOrEqual));
                query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, request.EndTime, CriteriaOperator.LesserThanOrEqual));
                query.QueryOperator = QueryOperator.And;
                query.OrderByProperty = OrderByClause.Create<DataPointHistoryData>(p => p.DateTime, false);

                IEnumerable<DataPointHistoryData> dataPointHistoryDatas =
                    dataPointHistoryDataRepository.FindBy(query);

                response.DataPointHistoryDataView =
                    dataPointHistoryDatas.ConvetToDataPointHistoryDataView();
            }
            catch (Exception ex)
            {
                string message = "查询失败!错误信息:/n" + ex.Message;
                response = new GetDataPiontHistoryDataResponse()
                {
                    ResponseSucceed = false,
                    Message = "查询失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

        public GetAllDataPointsHistoryDataResponse GetAllDataPointsHistoryData()
        {
            GetAllDataPointsHistoryDataResponse response = new GetAllDataPointsHistoryDataResponse();

            try
            {
                IEnumerable<DataPointHistoryData> dataPointHistoryDatas =
                    dataPointHistoryDataRepository.FindAll();

                response.DataPointHistoryDataView =
                    dataPointHistoryDatas.ConvetToDataPointHistoryDataView();
            }
            catch (Exception ex)
            {
                string message = "查询失败!错误信息:/n" + ex.Message;
                response = new GetAllDataPointsHistoryDataResponse()
                {
                    ResponseSucceed = false,
                    Message = "查询失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

        #endregion


    }
}
