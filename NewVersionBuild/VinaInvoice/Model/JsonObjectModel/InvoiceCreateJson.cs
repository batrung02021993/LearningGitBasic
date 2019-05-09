using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
	// Request is availble in Model

	public class InvoiceCreateResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public InvoiceCreateData data { get; set; }
	}

	public class InvoiceCreateData
	{
		public int invoice_number { get; set; }
		public string search_invoice_id { get; set; }
		public string invoice_id { get; set; }

	}


}
