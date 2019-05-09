using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
    class CustomerDeleteResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public CustomerDeleteResponseData data { get; set; }
    }
    public class CustomerDeleteResponseData
    {
        public string[] SuccessDelete { get; set; }
        public string[] FailDelete { get; set; }
    }
}
