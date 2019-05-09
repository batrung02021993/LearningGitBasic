using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model
{
    public class SerialDetail
    {
        public int code { get; set; }
        public string message { get; set; }
        public SerialDetailData data { get; set; }
    }

    public class SerialDetailData
    {
        public string ID { get; set; }
        public string form_name { get; set; }
        public string form_id { get; set; }
        public string serial_name { get; set; }
        public int start_number { get; set; }
        public int end_number { get; set; }
        public int quantity { get; set; }
        public int used_number { get; set; }
        public int stock_number { get; set; }
        public int last_used_number { get; set; }
        public int using_date { get; set; }
        public bool _using { get; set; }
        public string create_by { get; set; }
        public int create_time { get; set; }
        public int last_used_date { get; set; }
    }
}