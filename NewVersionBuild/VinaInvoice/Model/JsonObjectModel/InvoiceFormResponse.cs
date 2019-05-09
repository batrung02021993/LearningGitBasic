using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{


	public class InvoiceFormResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public DataInvoice data { get; set; }
	}

	public class DataInvoice
	{
		public int current_page { get; set; }
		public int max_item_one_page { get; set; }
		public Form[] invoice_form_list { get; set; }
	}

	

}
