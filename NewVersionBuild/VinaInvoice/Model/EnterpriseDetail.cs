using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.ViewModel;

namespace VinaInvoice.Model
{
    public class EnterpriseDetail 
    {
        public int code { get; set; }
        public string message { get; set; }
        public EnterpriseDetailData data { get; set; }
    }

    public class EnterpriseDetailData : BaseViewModel
    {
        private string _taxCode;
        

        public string id { get; set; }
        public string company_name { get; set; }
        public string tax_code {
            get { return _taxCode; }
            set { _taxCode = value; OnPropertyChanged(); }
        }
        public string address { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string bank_number { get; set; }
        public string bank_name { get; set; }
        public bool active { get; set; }
        public string manage_by { get; set; }
        public string create_by { get; set; }
        public int create_time { get; set; }
        public int member_number { get; set; }
        public string serect_password { get; set; }
        public string token_serial { get; set; }
        public string mail_invoice { get; set; }
        

    }
}


