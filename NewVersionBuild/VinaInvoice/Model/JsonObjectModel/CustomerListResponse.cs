using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{


	public class CustomerListResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public DataCustomerList data { get; set; }
	}

	public class DataCustomerList
	{
		public int current_page { get; set; }
		public int max_item_one_page { get; set; }
		public Invoice_Customer[] invoice_customer_list { get; set; }
	}

	public class Invoice_Customer
	{
		public string id { get; set; }
		public string email { get; set; }
		public string dislay_name { get; set; }
		public string tax_code { get; set; }
		public string address { get; set; }
		public string phone_number { get; set; }
		public string website { get; set; }
		public string fax_number { get; set; }
		public string bank_name { get; set; }
		public string bank_number { get; set; }
		public string director { get; set; }
		public string description { get; set; }
		public bool company { get; set; }
		public bool friendly { get; set; }
		public string company_tax_code { get; set; }
		public string company_name { get; set; }
		public string create_by { get; set; }
		public int create_time { get; set; }
		public string update_by { get; set; }
		public int update_time { get; set; }
	}
}