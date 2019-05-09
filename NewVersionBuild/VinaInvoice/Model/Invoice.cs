using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Common;
using VinaInvoice.ViewModel;

namespace VinaInvoice.Model
{
	/// <summary>
	/// Present for Invoice
	/// In this case also present for Json repuest Object
	/// </summary>
	public class Invoice : BaseViewModel, System.ComponentModel.INotifyDataErrorInfo
	{
		#region Only using for UI
		public bool IsSelected { get; set; } = false;
		public int Stt { get; set; } = 1;
		public string state_of_bill_string
		{
			get
			{
				if (status == 0)
					return "DRAFT";
				else if (status == 1)
					return "SIGNED";
				else if (status == 2)
					return "KEEP";
				else
					return "DELETE";

			}
			set
			{
				_state_of_bill_string = value;

			}
		}

		#endregion


		public string id { get => _id; set => _id = value; }
		public string search_invoice_id { get; set; }

		public string b_name {
            get => _b_name;
            set
            {
                _b_name = value;
                OnPropertyChanged();
            }
        }
		public string b_company {
            get => _b_company;
            set
            {
                _b_company = value;
                OnPropertyChanged();

            }
        }
		public string b_tax_code
		{
			get { return _b_tax_code; }
			set
			{
				
		        if (Isb_tax_codeValid(value) || _b_tax_code != value)
                {
                    _b_tax_code = value;
                    OnPropertyChanged();
                }
			}
		}
		public string b_address
        {
            get => _b_address;
            set
            {
                _b_address = value;
                OnPropertyChanged();

            }
        }
		public string b_phone_number { get => _b_phone_number; set => _b_phone_number = value; }
		public string b_fax_number { get => _b_fax_number; set => _b_fax_number = value; }
		public string b_email {
            get => _b_email;
            set
            {
                _b_email = value;
                OnPropertyChanged();
            }
        }
		public string b_website { get => _b_website; set => _b_website = value; }
		public string b_bank_number {
            get => _b_bank_number;
            set
            {
                _b_bank_number = value;
                OnPropertyChanged();
            }
        }
		public string b_bank_name {
            get => _b_bank_name;
            set
            {
                _b_bank_name = value;
                OnPropertyChanged();

            }
        }
		public bool send_draft { get => _send_draft; set => _send_draft = value; }
		public string invoice_form_name
		{
			get
			{

				return _invoice_form_name;
			}
			set
			{
				if (value.Trim() == "" || value == "...") value = null;
				_invoice_form_name = value;
			}
		}
		public string invoice_form_id { get => _invoice_form_id; set { if (value.Trim() == "") value = null; _invoice_form_id = value; } }
		public string secret_key { get => _secret_key; set => _secret_key = value; }
		public string invoice_file_name { get => _invoice_file_name; set => _invoice_file_name = value; }
		public string invoice_serial_name
		{
			get => _invoice_serial_name;
			set
			{
				if (value.Trim() == "" || value == "...") value = null;
				_invoice_serial_name = value;
			}
		}
		public string invoice_serial_id { get => _invoice_serial_id; set => _invoice_serial_id = value; }
		public string invoice_ref { get => _invoice_ref; set => _invoice_ref = value; }
		public int invoice_number { get => _invoice_number; set => _invoice_number = value; }
		public string invoice_sign_date
		{
			get => _invoice_sign_date;
			set
			{
				if (value.Trim() == "" || value == "...") value = null;
				_invoice_sign_date = value;
			}
		}
		public int vat_rate { get => _vat_rate; set => _vat_rate = value; }
		public int method_of_payment { get => _method_of_payment; set => _method_of_payment = value; }
		public string currency_code { get => _currency_code; set => _currency_code = value; }
		public double exchange_rate
		{
			get => _exchange_rate;
			set
			{
				if (IsPositivedoubleValid("exchange_rate", value, "Tỷ giá") || _exchange_rate != value)
					_exchange_rate = value;
			}
		}
		public double sub_total { get => _sub_total; set => _sub_total = value; }
		public double vat_amount { get => _vat_amount; set => _vat_amount = value; }
		public double total_amount { get => _total_amount; set => _total_amount = value; }
		public int discount_percent {
			get => _discount_percent;
			set
			{
				_discount_percent = value;			
			}
		}
		public double total_discount { get => _total_discount; set { _total_discount = value; } }
		public int service_charge_percent { get => _service_charge_percent; set => _service_charge_percent = value; }
		public double total_service_charge
		{
			get => _total_service_charge;
			set
			{
				if (IsPositivedoubleValid("total_service_charge", value, "Phí dịch vụ") || _total_service_charge != value)
					_total_service_charge = value;

			}
		}
		public string user_defines { get => _user_defines; set => _user_defines = value; }
		public int status { get => _status; set => _status = value; }
		public int state_of_bill { get; set; }
		public string convert_user { get; set; }
		public string adjust_user { get; set; }
		public string original_invoice { get => _original_invoice; set => _original_invoice = value; }
		public string in_word { get => _in_word; set { _in_word = value; OnPropertyChanged(); } }
		public string document_no { get => _document_no; set => _document_no = value; }
		public string reason_content
		{
			get => _reason_content;
			set
			{
				_reason_content = value;
				OnPropertyChanged();
			}
		}
		public string document_id { get; set; }
		public string document_content { get; set; }
		public string document_attach_url { get => _document_attach_url; set => _document_attach_url = value; }
		public string invoice_cloud_id { get => _invoice_cloud_id; set => _invoice_cloud_id = value; }
		public bool send_mail { get => _send_mail; set => _send_mail = value; }
		public string xml_content { get => _xml_content; set => _xml_content = value; }
		public string invoice_note { get => _invoice_note; set => _invoice_note = value; }
		public bool is_auto_sign { get => _is_auto_sign; set => _is_auto_sign = value; }
		public List<string> delete_item_list { get; set; } = new List<string>();
		public List<InvoiceItem> invoice_item_list { get => _invoice_item_list; set => _invoice_item_list = value; }


		private string _state_of_bill_string = "";
		private string _id = "";
		private string _b_name = "";
		private string _b_company = "";
		private string _b_tax_code;
		private string _b_address = "";
		private string _b_phone_number = "";
		private string _b_fax_number = "";
		private string _b_email = "";
		private string _b_website = "";
		private string _b_bank_number = "";
		private string _b_bank_name = "";
		private bool _send_draft = false;
		private string _invoice_form_name;
		private string _invoice_form_id;
		private string _secret_key = "";
		private string _invoice_file_name = "";
		private string _invoice_serial_name;
		private string _invoice_serial_id;
		private string _invoice_ref = "";
		private int _invoice_number;
		private string _invoice_sign_date;
		private int _vat_rate = 10;
		private int _method_of_payment = 0;
		private string _currency_code = "";
		private double _exchange_rate = 1;
		private double _sub_total = 0;
		private double _vat_amount = 0;
		private double _total_amount = 0;
		private double _total_discount = 0;
		private int _service_charge_percent = 0;
		private double _total_service_charge = 0;
		private string _user_defines = "";
		private int _status;
		private string _invoice_cloud_id = "";
		private bool _send_mail = false;
		private string _xml_content = "";

		private string _invoice_note = "";
		private bool _is_auto_sign = false;
		private string _original_invoice = "";
		private string _in_word = "";
		private string _document_no = "";
		private string _reason_content = "";
		private string _document_attach_url = "";





		private List<InvoiceItem> _invoice_item_list;


		#region General Validation
		// Validates the Taxcode property, updating the _errors collection as needed.
		private bool Isb_tax_codeValid(string value)
		{
			if (value == null || value.Trim().Length == 0)
			{
				RemoveError("b_tax_code", Const.b_tax_code_ERROR);
				return true;
			}

			bool isValid = true;

			if (value.Length < 10)
			{
				AddError("b_tax_code", Const.b_tax_code_ERROR, false);
				isValid = false;
			}
			else RemoveError("b_tax_code", Const.b_tax_code_ERROR);

			return isValid;
		}

		private bool IsPositivedoubleValid(string property, double value, string errorSource)
		{
			bool isValid = true;

			if (value < 0)
			{
				AddError(property, Const.NotPositivedouble_ERROR + errorSource, false);
				isValid = false;
			}
			else RemoveError(property, Const.NotPositivedouble_ERROR + errorSource);

			return isValid;
		}

		private Dictionary<String, List<String>> _errors = new Dictionary<string, List<string>>();
		private int _discount_percent;

		public Dictionary<String, List<String>> errors
		{
			get
			{
				return _errors;
			}
			set
			{
				_errors = value;
			}
		}

		// Adds the specified error to the _errors collection if it is not 
		// already present, inserting it in the first position if isWarning is 
		// false. Raises the _errorsChanged event if the collection changes. 
		public void AddError(string propertyName, string error, bool isWarning)
		{
			if (!_errors.ContainsKey(propertyName))
				_errors[propertyName] = new List<string>();

			if (!_errors[propertyName].Contains(error))
			{
				if (isWarning) _errors[propertyName].Add(error);
				else _errors[propertyName].Insert(0, error);
				RaiseerrorsChanged(propertyName);
			}
		}

		// Removes the specified error from the _errors collection if it is
		// present. Raises the _errorsChanged event if the collection changes.
		public void RemoveError(string propertyName, string error)
		{
			if (_errors.ContainsKey(propertyName) &&
				_errors[propertyName].Contains(error))
			{
				_errors[propertyName].Remove(error);
				if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);
				RaiseerrorsChanged(propertyName);
			}
		}

		public void RaiseerrorsChanged(string propertyName)
		{
			if (ErrorsChanged != null)
				ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
		}
		/// impelent INotifyDataErrorInfo

		public bool HasErrors
		{
			get { return _errors.Count > 0; }
		}


		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors(string propertyName)
		{
			if (String.IsNullOrEmpty(propertyName) ||
				!_errors.ContainsKey(propertyName)) return null;
			return _errors[propertyName];
		}


		#endregion

		#region Validate in case Sing Invoice
		private bool isNullValid(string propertyname, string value, string errorSource)
		{
			bool isValid = true;

			if (value == null)
			{
				AddError(propertyname, Const.Null_code_ERROR + errorSource, false);
				isValid = false;
			}
			else RemoveError(propertyname, Const.Null_code_ERROR + errorSource);

			return isValid;

		}
		public void Sign_Validate()
		{
			//if (_b_tax_code == null) b_tax_code = "0";
			isNullValid("invoice_form_name", _invoice_form_name, "Mẫu số (Form)");
			isNullValid("invoice_serial_name", _invoice_serial_name, "kí hiệu (Serial)");
			isNullValid("invoice_sign_date", _invoice_sign_date, "Ngày ký hóa đơn");
		}
		#endregion
	}

	public class InvoiceItem
    {
		public string id { get; set; }
		public string item_code { get; set; }
        public string item_name { get; set; }
        public string unit_name { get; set; }
        public double unit_price { get; set; }
        public Double quantity { get; set; }
        public double item_total_amount_without_vat { get; set; }
        public int vat_percentage { get; set; }
        public double vat_amount { get; set; }
        public string current_code { get; set; }
        public string item_note { get; set; }
        public string item_type { get; set; }
        public double discount_amount { get; set; }
        public double total_amount { get; set; }
    }
}
