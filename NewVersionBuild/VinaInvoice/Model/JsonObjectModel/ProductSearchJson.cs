using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
	class ProductSearchJson
	{
	}

	public class ProductSearchResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public DataProDuctSearch data { get; set; }
	}

	public class DataProDuctSearch
	{
		public int current_page { get; set; }
		public int max_item_one_page { get; set; }
		public List<Product> invoice_product_list { get; set; }
	}

	
	public class ProductSearchRequest
	{
        public int type_search { get; set; }
        public int type_sort { get; set; }
        public int sort_method { get; set; }
        public string key { get; set; }
        public int page { get; set; }
    }
}
