using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Common;

namespace VinaInvoice.Model
{
	public class ProfileData
	{
        private bool _isAdmin = false;
        private bool _isSuperAdmin = false;


        public string id { get; set; }
		public string email { get; set; }
		public string token { get; set; }
		public string role { get; set; }
        public string keySuperAdin { get; set; }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
            }
        }
        public bool IsSupperAdmin {
            get => _isSuperAdmin;
            set {
                _isSuperAdmin = value;
            }
        }
        public string company_id { get; set; }
		public Enterprise enterprise { get; set; }
		public class Enterprise
		{
			public bool create_account { get; set; }
			public bool delete_account { get; set; }
			public bool create_invoice { get; set; }
			public bool sign_invoice { get; set; }
			public bool draft_invoice { get; set; }
			public bool cancel_invoice { get; set; }
			public bool view_invoice { get; set; }
			public bool view_owner_invoice { get; set; }
			public bool change_invoice { get; set; }
			public bool convert_invoice { get; set; }
		}

        public string getUserRole(string role)
        {
            string RoleString = "";
            if (role.Equals("SA"))
            {
                RoleString = Const.ROLE_USER_SA;
            }
            else if (role.Equals("AM")){
                RoleString = Const.ROLE_USER_AM;
            }
            else if (role.Equals("CB")){
                RoleString = Const.ROLE_USER_CB;
            }
            else if (role.Equals("AC")){
                RoleString = Const.ROLE_USER_AC;
            }else{
                RoleString = Const.ROLE_USER_CT;
            }

            return RoleString;
        }


    }
}
