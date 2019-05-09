using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.ViewModel;

namespace VinaInvoice.Model
{
    public class User
    {
        public int STT { get; set; }

        public string Id { get; set; }

        public string DisplayName { get; set; }

        public bool Status { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string PhoneNumber { get; set; }
    }
}
