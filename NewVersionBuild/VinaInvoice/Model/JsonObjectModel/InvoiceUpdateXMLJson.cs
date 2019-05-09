using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{

	public class InvoiceUpdateXMLRequest
	{
		public List<Xml_Update> xml_update_list { get; set; }
	}

	public class Xml_Update
	{
		public string invoice_id { get; set; }
		public string xml_content { get; set; }
	}


	public class InvoiceUpdateXMLResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public UdateXMLData data { get; set; }
	}

	public class UdateXMLData
	{
		public string[] SuccessList { get; set; }
		public string[] FailList { get; set; }
	}




}
