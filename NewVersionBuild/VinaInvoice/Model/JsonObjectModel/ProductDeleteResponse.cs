using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
    public class ProductDeleteResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public ProductDeleteResponseData data { get; set; }
    }

    public class ProductDeleteResponseData
    {
        public string[] SuccessDelete { get; set; }
        public string[] FailDelete { get; set; }
    }
}


