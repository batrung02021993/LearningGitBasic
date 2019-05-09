using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model
{

	public class FormDetailRequest
	{ 
		public string enterprise_id { get; set; }
		public string id { get; set; }
	}


	public class FormDetailResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public Form data { get; set; }
	}



	public class Form
	{
		public string id { get; set; } = "";
		public string form_name { get; set; } = "";
		public string xslt_content { get; set; } = "";
		public string xml_content { get; set; } = "";
		public bool _using { get; set; } = false;
		public string create_by { get; set; } = "";
		public int create_time { get; set; }
		public string update_by { get; set; } = "";
		public int update_time { get; set; }
		public string image { get; set; } = "";
		public bool Is_more_tax { get; set; }
	}

}
