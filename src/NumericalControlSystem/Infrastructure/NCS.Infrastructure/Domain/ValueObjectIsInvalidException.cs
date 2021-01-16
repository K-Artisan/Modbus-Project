using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Domain
{
    /// <summary>
    /// 值对象异常类
    /// </summary>
    public class ValueObjectIsInvalidException : Exception
    {
        public ValueObjectIsInvalidException(string message) 
            : base(message)
        {
        }
    }
}
