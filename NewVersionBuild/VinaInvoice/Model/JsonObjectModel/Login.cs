using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.ViewModel;

namespace VinaInvoice.Model.JsonObjectModel
{
	public class LoginInfo : BaseViewModel
	{
		private string _tax_code;

		public string email { get; set; }
		public string password { get; set; }
		public string tax_code { get => _tax_code; set { _tax_code = value;OnPropertyChanged(); } }
		public string model { get; set; }
		public string role { get; set; }
	}
	public class Profile
	{
		public int code { get; set; }
		public string message { get; set; }
		public ProfileData data { get; set; }	
	}
}
