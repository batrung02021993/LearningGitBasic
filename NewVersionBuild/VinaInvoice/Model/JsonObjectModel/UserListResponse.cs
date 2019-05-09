using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model.JsonObjectModel
{

    public class UserListResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public DataUser data { get; set; }
    }

    public class DataUser
    {
        public int current_page { get; set; }
        public int max_item_one_page { get; set; }
        public User_List[] user_list { get; set; }

    }

    public class User_List
    {
        public string id { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public bool   active { get; set; }
        public string create_by { get; set; }
        public int create_time { get; set; }
    }

}
