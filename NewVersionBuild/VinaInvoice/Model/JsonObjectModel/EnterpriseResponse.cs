using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{
    public class EnterpriseResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public DataMember data { get; set; }
    }

    public class DataMember
    {
        public int current_page { get; set; }
        public int max_item_one_page { get; set; }
        public Enterprise_Members[] enterprise_members { get; set; }

    }

    public class Enterprise_Members
    {
        public string id { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public bool active { get; set; }
        public bool create_account { get; set; }
        public bool delete_account { get; set; }
        public bool create_invoice { get; set; }
        public bool sign_invoice { get; set; }
        public bool draft_invoice { get; set; }
        public bool cancel_invoice { get; set; }
        public bool view_invoice { get; set; }
        public bool view_my_invoice { get; set; }
        public bool change_invoice { get; set; }
        public bool convert_invoice { get; set; }
    }
}
