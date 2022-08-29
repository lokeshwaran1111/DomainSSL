using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Domainssl
{
    public class HandledException : Exception
    {
        public int Code { get; private set; }

        public HandledException(Exception ex, HttpStatusCode code) : base(ex.Message, ex)
        {
            this.Code = (int)code;
        }

        public HandledException(string message, HttpStatusCode code) : base(message)
        {
            this.Code = (int)code;
        }

        public HandledException(string message, int code) : base(message)
        {
            this.Code = (int)code;
        }
    }
}
