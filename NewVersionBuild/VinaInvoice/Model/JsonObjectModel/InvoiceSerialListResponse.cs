using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InvoiceSerialListResponse
{
	public int code { get; set; }
	public string message { get; set; }
	public DataInvoiceSerialList data { get; set; }
}

public class DataInvoiceSerialList
{
	public int current_page { get; set; }
	public int max_item_one_page { get; set; }
	public InvoiceSerial[] invoice_serial_list { get; set; }
}

public class InvoiceSerial
{
	public string id { get; set; }
	public string form_name { get; set; }
	public string form_id { get; set; }
	public string serial_name { get; set; }
	public int start_number { get; set; }
	public int end_number { get; set; }
	public long quantity { get; set; }
	public int used_number { get; set; }
	public long stock_number { get; set; }
	public int last_used_number { get; set; }
	public int? using_date { get; set; }
	public bool _using { get; set; }
	public string create_by { get; set; }
	public int create_time { get; set; }
	public string update_by { get; set; }
	public int update_time { get; set; }
    public int last_used_date { get; set; }
}


