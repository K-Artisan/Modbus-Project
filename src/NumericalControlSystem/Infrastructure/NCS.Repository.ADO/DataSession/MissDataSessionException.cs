using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Repository.ADO.DataSession
{
    public class MissDataSessionException : Exception
    {
        public MissDataSessionException(string message)
            : base(message)
        {
        }
    }
}
