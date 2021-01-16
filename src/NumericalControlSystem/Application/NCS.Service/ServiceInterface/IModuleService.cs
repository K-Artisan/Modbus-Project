using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.Messaging.ModuleService;

namespace NCS.Service.ServiceInterface
{
    public interface IModuleService
    {
        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        AddModuleResponse AddModule(AddModuleRequst requst);

        /// <summary>
        /// 获取特定的Module（模块）信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetModuleResponse GetModule(GetModuleRequest request);

        /// <summary>
        /// 获取所有的Module（模块）信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAllModuleResponse GetAllModules(GetAllModuleRequest request);
    }
}
