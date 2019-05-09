using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
	public class CustomerSearchResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public DataSearchCustomer data { get; set; }
	}

	public class DataSearchCustomer
	{
		public int current_page { get; set; }
		public int max_item_one_page { get; set; }
		public List<Invoice_Customer_Search> invoice_customer_list { get; set; }
	}

	public class Invoice_Customer_Search
	{
		public string id { get; set; }
		public string dislay_name { get; set; }
        public string company_tax_code { get; set; } 
		public string address { get; set; }
		public string phone_number { get; set; }
		public string fax_number { get; set; }
		public string website { get; set; }
		public string bank_name { get; set; }
		public string bank_number { get; set; }
		public string company_name { get; set; }
		public bool company { get; set; }
		public bool friendly { get; set; }
		public string create_by { get; set; }
		public int create_time { get; set; }
		
	}

	//request

	public class CustomerSearchRequest
	{
        public int type_search { get; set; }
        public int type_sort { get; set; }
        public int sort_method { get; set; }
        public string key { get; set; }
        public int page { get; set; }
    }

}
