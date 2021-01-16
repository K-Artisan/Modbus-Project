using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.Messaging.DataBaseConfigService;

namespace NCS.Service.ServiceInterface
{
    public interface IDataBaseConfigService
    {
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CreateDataBaseResponse CreateDataBase(CreateDataBaseRequest request);

        /// <summary>
        /// 执行数据库脚本
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ExecuteSqlScriptResponse ExecuteSqlScript(ExecuteSqlScriptRequest request);

        /// <summary>
        /// 获取当前数据库登录信息
        /// </summary>
        /// <returns></returns>
        GetCurrentDataBaseLoginInfoResponse GetCurrentDataBaseLoginInfoAndConnetStatus();

        /// <summary>
        /// 测试是否能链接数据库
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        TestConnetDataBaseResponse TestConnetDataBase(TestConnetDataBaseRequest  request);
    }
}
