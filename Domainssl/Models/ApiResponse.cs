using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domainssl.Models
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Result { get; set; }
        public bool HasError { get; set; }
        public string Message { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
