using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model
{
    public class EnterPrise
    {
        public string id { get; set; }

        public string company_name { get; set; }

        public string tax_code { get; set; }

        public string address { get; set; }

        public string phone_number { get; set; }

        public bool active { get; set; }

        public string manage_by { get; set; }

        public string create_by { get; set; }

        public int create_time { get; set; }

        public int member_number { get; set; }

        public string serect_password { get; set; }  

    }
}
