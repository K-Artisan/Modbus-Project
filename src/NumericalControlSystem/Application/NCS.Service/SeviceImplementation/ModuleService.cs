using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Logging;
using NCS.Infrastructure.UnitOfWork;



using NCS.Service.Mapping;
using NCS.Service.Messaging.ModuleService;
using NCS.Service.ServiceInterface;
using NCS.Model.Repository;
using NCS.Model.Entity;

namespace NCS.Service.SeviceImplementation
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ModuleService(IModuleRepository moduleRepository,
            IUnitOfWork unitOfWork)
        {
            _moduleRepository = moduleRepository;
            _unitOfWork = unitOfWork;
        }


        public AddModuleResponse AddModule(AddModuleRequst requst)
        {
            AddModuleResponse response = new AddModuleResponse();

            try
            {
                foreach (Module module in requst.ModulesToAdd)
                {
                    _moduleRepository.Add(module);
                }

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                string message = "添加失败!错误信息:/n" + ex.Message;
                response = new AddModuleResponse()
                {
                    ResponseSucceed = false,
                    Message = "添加失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

        public GetModuleResponse GetModule(GetModuleRequest request)
        {
            GetModuleResponse response = new GetModuleResponse();

            try
            {
                Module module = _moduleRepository.FindBy(request.ModuleId);
                response.ModuleView = module.ConverToModuleView();
            }
            catch (Exception ex)
            {
                string message = "查询失败!错误信息:/n" + ex.Message;
                response = new GetModuleResponse()
                {
                    ResponseSucceed = false,
                    Message = "查询失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

        public GetAllModuleResponse GetAllModules(GetAllModuleRequest request)
        {
            GetAllModuleResponse response = new GetAllModuleResponse();

            try
            {
                IEnumerable<Module> modules = _moduleRepository.FindAll();

                response.ModuleViews = modules.ConverToModuleViews();
            }
            catch (Exception ex)
            {
                string message = "查询失败!错误信息:/n" + ex.Message;
                response = new GetAllModuleResponse()
                {
                    ResponseSucceed = false,
                    Message = "查询失败"
                };
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return response;
            }

            return response;
        }

    }
}
