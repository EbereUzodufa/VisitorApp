using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace VisitorApp.Common
{
    public class ResponseMessage
    {
        public int ResponseCode { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string userName { get; set; }
        public string companyName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string photstring { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
    }
}
