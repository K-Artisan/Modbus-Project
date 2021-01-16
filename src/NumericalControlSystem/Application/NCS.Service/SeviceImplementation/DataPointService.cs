using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;
using NCS.Infrastructure.UnitOfWork;



using NCS.Service.Mapping;
using NCS.Service.Messaging.DataPointService;
using NCS.Service.ServiceInterface;
using NCS.Infrastructure.Logging;
using NCS.Model.Repository;
using NCS.Model.Entity;

namespace NCS.Service.SeviceImplementation
{
    public class DataPointService : IDataPointService
    {
        private readonly IDataPointRepository _dataPointRepository;
        //private readonly IDataPointHistoryDataRepository _dataPointHistoryDataRepository;
        //private readonly IModuleRepository _moduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        
        public DataPointService(IDataPointRepository dataPointRepository,
            //IDataPointHistoryDataRepository dataPointHistoryDataRepository,
            //IModuleRepository moduleRepository,
            IUnitOfWork unitOfWork)
        {
            _dataPointRepository = dataPointRepository;
            //_dataPointHistoryDataRepository = dataPointHistoryDataRepository;
            //_moduleRepository = moduleRepository;
            _unitOfWork = unitOfWork;
        }

        #region IDataPointService Members


        public AddDataPointResponse AddDataPoint(AddDataPointRequst requst)
        {
            AddDataPointResponse response = new AddDataPointResponse();

            try
            {
                foreach (DataPoint dataPoint in requst.DataPointsToAdd)
                {
                    _dataPointRepository.Add(dataPoint);
                }

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                string message = "添加失败!错误信息:/n" + ex.Message;
                response = new AddDataPointResponse()
                {
                    ResponseSucceed = false,
                    Message = "添加失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

        public GetDataPointInfoResponse GetDataPointInfo(GetDataPointInfoRequest request)
        {
            GetDataPointInfoResponse response = new GetDataPointInfoResponse();

            try
            {
                DataPoint dataPoint = _dataPointRepository.FindBy(request.DataPointId);
                response.DataPointInfoView = dataPoint.ConvertToDataPoinntInfoView();
            }
            catch (Exception ex)
            {
                string message = "查询失败!错误信息:/n" + ex.Message;
                response = new GetDataPointInfoResponse()
                {
                    ResponseSucceed = false,
                    Message = "查询失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

        public GetAllDataPointsInfoResponse GetAllDataPointInfo()
        {
            GetAllDataPointsInfoResponse response = new GetAllDataPointsInfoResponse();

            try
            {
                IEnumerable<DataPoint> allDataPoints = _dataPointRepository.FindAll();
                response.DataPointInfoViews = allDataPoints.ConverToDataPointInfoViews();
            }
            catch (Exception ex)
            {
                string message = "查询失败!\n错误信息:\n" + ex.Message;
                response = new GetAllDataPointsInfoResponse()
                {
                        ResponseSucceed = false,
                        Message = "查询失败"
                 };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }


            return response;
        }

        public GetDataPointByModuleResponse GetDataPointInfoByModule(GetDataPointByModuleRequest request)
        {
            GetDataPointByModuleResponse response = new GetDataPointByModuleResponse();

            try
            {
                Query query = new Query();
                query.AddCriterion(Criterion.Create<DataPoint>(p => p.ModuleBelongTo.Id, request.ModuleId, CriteriaOperator.Equal));
                query.OrderByProperty = OrderByClause.Create<DataPoint>(p => p.Number, false);

                IEnumerable<DataPoint> dataPoints = _dataPointRepository.FindBy(query);
                response.DataPointInfoViews = dataPoints.ConverToDataPointInfoViews();
            }
            catch (Exception ex)
            {
                string message = "查询失败!错误信息:/n" + ex.Message;
                response = new GetDataPointByModuleResponse()
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
