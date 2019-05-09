using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
    class InvoiceDeleteRequest
    {
        public string[] id { get; set; }
    }


	public class InvoiceDeleteResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public DeleteResponse data { get; set; }
	}

	public class DeleteResponse
	{
		public object SuccessDelete { get; set; }
		public string[] FailDelete { get; set; }
	}

}
