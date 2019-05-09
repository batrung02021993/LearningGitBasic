using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model;

namespace VinaInvoice.Model.JsonObjectModel
{
    class InvoiceListRequest
	{
        public int page { get; set; }
        public int[] status { get; set; }
		public int[] state_of_bill { get; set; }
	}

    public class InvoiceListResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public InvoiceList data { get; set; }
    }

    public class InvoiceList
    {
        public int current_page { get; set; }
        public int max_item_one_page { get; set; }
        public Invoice[] invoice_invoice_list { get; set; }
    }

}

