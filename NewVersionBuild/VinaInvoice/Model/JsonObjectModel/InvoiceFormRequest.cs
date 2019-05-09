using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{	
	public class InvoiceFormRequest
	{
		public string enterprise_id { get; set; }
		public int page { get; set; }
		public string type_search { get; set; }
	}

}
