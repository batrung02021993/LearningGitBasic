using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
    public class ProductAddList : ICloneable
    {
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
        public int discount_percentage { get; set; }
        public double discount_amount { get; set; }
        public double total_amount { get; set; }
        public string item_code { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class ProductAddListResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public ProductAddListResponseData data { get; set; }
    }

    public class ProductAddListResponseData
    {
        public int[] success_add { get; set; }
        public int[] fail_add { get; set; }
    }
}



