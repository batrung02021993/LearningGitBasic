using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
	
	public class ProductListResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public Data data { get; set; }
	}

	public class Data
	{
		public int current_page { get; set; }
		public int max_item_one_page { get; set; }
		public Product[] invoice_product_list { get; set; }
	}

	

}
