using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model
{
    public class Customer
    {
		public int STT { get; set; }

		public string Id { get; set; }

        public bool IsSelected { get; set; }

        public string DisplayName { get; set; }

        public string TaxCode { get; set; }

        public string CompanyName { get; set; }

        public string Email { get; set; }

        public string PersonalTaxCode { get; set; }

        public string CompanyTaxCode { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Website { get; set; }

        public string FaxNumber { get; set; }

        public string Director { get; set; }

        public string BankNumber { get; set; }

        public string BankName { get; set; }

        public string Description { get; set; }
    }
}
