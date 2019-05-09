using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model

{
    public class Member
    {
        public int STT { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public bool Active { get; set; }

        public bool CreateAccount { get; set; }

        public bool DeleteAccount { get; set; }

        public bool CreateInvoice { get; set; }

        public bool SignInvoice { get; set; }

        public bool DraftInvoice { get; set; }

        public bool CancelInvoice { get; set; }

        public bool ViewInvoice { get; set; }

        public bool ViewMyInvoice { get; set; }

        public bool ChangeInvoice { get; set; }

        public bool ConvertInvoice { get; set; }

    }
}
