using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ZaloPay.Response
{
    public class CreateZaloPayPayResponse
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = string.Empty;
        public string OrderUrl { get; set; } = string.Empty;
    }
}
