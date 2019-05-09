using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
    public class CustomerAddList : ICloneable
    {
        public string email { get; set; }
        public string dislay_name { get; set; }
        public string tax_code { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }
        public string fax_number { get; set; }
        public string website { get; set; }
        public string bank_name { get; set; }
        public string bank_number { get; set; }
        public string company_tax_code { get; set; }
        public string company_name { get; set; }
        public string director { get; set; }
        public string description { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CustomerAddListResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public CustomerAddListResponseData data { get; set; }
    }

    public class CustomerAddListResponseData
    {
        public int[] success_add { get; set; }
        public int[] fail_add { get; set; }
    }
}