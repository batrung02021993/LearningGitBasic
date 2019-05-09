using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
	

		public class InvoiceDocumentRequest
	{
		public string document_id { get; set; }
		
	}

	public class InvoiceDocumentResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public DocumentRespone data { get; set; }
	}

	public class DocumentRespone
	{
		public string document_content { get; set; }
	}

	public class InvoiceChangeRequest
	{
		public string id { get; set; }
		public int state_of_bill { get; set; }
		public string document_no { get; set; }
		public string document_content { get; set; }
		public string reason_content { get; set; }
		
	}

	public class InvoiceChangeResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public ChangeRespone data { get; set; }
	}

	public class ChangeRespone
	{
		public int invoice_number { get; set; }
		public string search_invoice_id { get; set; }
		public string invoice_id { get; set; }
		
	}


	public class InvoiceConvertRequest
	{
		public string id { get; set; }
		public string xml_content { get; set; }
	}


	public class InvoiceConvertResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public ConvertResponse data { get; set; }
	}

	public class ConvertResponse
	{
		public int invoice_number { get; set; }
		public string search_invoice_id { get; set; }
	}


}
