using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
    public class InvoiceAddListDraftResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public InvoiceAddListDraftResponseData data { get; set; }
    }

    public class InvoiceAddListDraftResponseData
    {
        public int[] success_add { get; set; }
        public int[] fail_add { get; set; }
    }

    public class InvoiceAddListDraft : ICloneable
    {
        public string id { get; set; }
        public string b_name { get; set; }
        public string b_company { get; set; }
        public string b_tax_code { get; set; }
        public string b_address { get; set; }
        public string b_phone_number { get; set; }
        public string b_fax_number { get; set; }
        public string b_email { get; set; }
        public string b_website { get; set; }
        public string b_bank_number { get; set; }
        public string b_bank_name { get; set; }
        public bool send_draft { get; set; }
        public string invoice_form_name { get; set; }
        public string invoice_form_id { get; set; }
        public string secret_key { get; set; }
        public string invoice_file_name { get; set; }
        public string invoice_serial_name { get; set; }
        public string invoice_serial_id { get; set; }
        public string invoice_ref { get; set; }
        public string original_invoice { get; set; }
        public int invoice_number { get; set; }
        public int vat_rate { get; set; }
        public int method_of_payment { get; set; }
        public string currency_code { get; set; }
        public double exchange_rate { get; set; }
        public double sub_total { get; set; }
        public double vat_amount { get; set; }
        public double total_amount { get; set; }
        public double total_discount { get; set; }
        public int service_charge_percent { get; set; }
        public double total_service_charge { get; set; }
        public string invoice_sign_date { get; set; }
        public string user_defines { get; set; }
        public int state_of_bill { get; set; }
        public int status { get; set; }
        public string in_word { get; set; }
        public string invoice_cloud_id { get; set; }
        public bool send_mail { get; set; }
        public string xml_content { get; set; }
        public bool is_auto_sign { get; set; }
        public string create_by { get; set; }
        public int create_time { get; set; }
        public List<InvoiceAddListDraftData> invoice_item_list { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class InvoiceAddListDraftData
    {
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string unit_name { get; set; }
        public double unit_price { get; set; }
        public double quantity { get; set; }
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
