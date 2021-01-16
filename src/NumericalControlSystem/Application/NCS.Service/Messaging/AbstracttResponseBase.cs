using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.Messaging
{
    public abstract class AbstracttResponseBase
    {
        private bool _responseSucceed = true;
        private string _message = "";

        public bool ResponseSucceed
        {
            get { return _responseSucceed; }
            set { _responseSucceed = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
