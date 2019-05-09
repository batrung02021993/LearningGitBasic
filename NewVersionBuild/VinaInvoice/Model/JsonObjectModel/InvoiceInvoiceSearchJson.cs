using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model;

namespace VinaInvoice.Model.JsonObjectModel
{
    public class InvoiceInvoiceSearchRequest
    {
        public int type_search { get; set; }
        public int type_sort { get; set; }
        public int sort_method { get; set; }
        public string key { get; set; }
        public int page { get; set; }
        public int[] status { get; set; }
        public int[] state_of_bill { get; set; }
        public string start_date { get; set; }
        public string stop_date { get; set; }
        public bool get_detail { get; set; }
    }

    public class InvoiceInvoiceSearchResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public InvoiceInvoiceSearchResponseData data { get; set; }
    }

    public class InvoiceInvoiceSearchResponseData
    {
        public int current_page { get; set; }
        public int max_item_one_page { get; set; }
        public Invoice[] invoice_invoice_list { get; set; }
    }
}



