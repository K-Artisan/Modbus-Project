using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.Messaging.DataBaseConfigService
{
    public class TestConnetDataBaseRequest
    {
        /// <summary>
        /// true保存保存当前测试的参数，，否则不保存
        /// </summary>
        public bool SaveConfig { get; set; }   

        /// <summary>
        /// 数据库IP
        /// </summary>
        public string Ip { get; set; } 

        /// <summary>
        /// 数据库账号
        /// </summary>
        public string Account { get; set; }  

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } 
    }
}
