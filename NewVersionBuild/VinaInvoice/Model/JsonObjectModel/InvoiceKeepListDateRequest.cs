using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
    class InvoiceKeepDateRequest
    {
        public string invoice_id { get; set; }
        public string enterprise_id { get; set; }

    }


    public class InvoiceKeepDateResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public InvoiceKeepDate data { get; set; }
    }

    public class InvoiceKeepDate
    {
        public int start_date { get; set; }
        public int stop_date { get; set; }
    }
}