using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.ViewModel;

namespace VinaInvoice.Common
{
    public static class Const
    {
        //Ip config 

        public static string BASE_API_RELEASE = "https://hoadon.onlinevina.com.vn/api/v1/";
        //"http://103.221.223.87:80/api/v1/";
        //public static string BASE_API_RELEASE = "http://13.250.18.235/api/v1/";
        //public static string BASE_API_RELEASE = "http://localhost:8085/api/v1/";

        public static string BASE_API_DEBUG = "http://35.187.241.25:80/api/v1/";
        public static string BASE_API_DEBUG1 = "https://dev.onlinevina.com.vn/api/v1/";
        public static string APP_NAME       = "Phần mềm hóa đơn điện tử ViNa Invoice";

        public static string STATUS_BAR_STRING { get; set; } 


        //Major Invoice
        public static string MAJOR_INVOICE_DELETE      = "Hủy hóa đơn";
        public static string MAJOR_INVOICE_REPLACE     = "Lập hóa đơn thay thế";
        public static string MAJOR_INVOICE_EDIT        = "Lập hóa đơn Điều chỉnh";
        public static string MAJOR_REPORT_EDIT         = "Lập biên bản Điều chỉnh";
        public static string MAJOR_INVOICE_EDIT_INFO   = "Lập hóa đơn Điều chỉnh thông tin";
		public static string MAJOR_INVOICE_CONVERT	   = "Chuyển đổi hóa đơn";

        public static string MAJOR_INVOICE_DELETE_RESON     = "Hủy hóa đơn, Hóa đơn số : ";
        public static string MAJOR_INVOICE_REPLACE_RESON    = "Lập hóa đơn Thay thế, Hóa đơn số : ";
        public static string MAJOR_INVOICE_EDIT_RESON       = "Lập hóa đơn Điều chỉnh, Hóa đơn số : ";
        public static string MAJOR_REPORT_EDIT_RESON        = "Lập biên bản Điều chỉnh, Hóa đơn số : ";
        public static string MAJOR_INVOICE_EDIT_INFO_RESON  = "Lập hóa đơn Điều chỉnh thông tin, Hóa đơn số : ";
        public static string MAJOR_INVOICE_CONVERT_RESON    = "Chuyển đổi thành hóa đơn giấy, Hóa đơn số : ";


        //Tax value
        public static string TAX_VALUE_0    = "0%";
        public static string TAX_VALUE_5    = "5%";
        public static string TAX_VALUE_10   = "10%";
        public static string TAX_VALUE_NO   = "Không chịu thuế";

        //Status Invoice
        public static string STATUS_INVOICE_DRAFT = "0";
        public static string STATUS_INVOICE_SIGNED = "1";

        //Role of User
        public static string ROLE_USER_SA = "Supper Admin";
        public static string ROLE_USER_AM = "Admin";
        public static string ROLE_USER_AC = "Đại lý";
        public static string ROLE_USER_CB = "Cộng tác viên";
        public static string ROLE_USER_CT = "Doanh nghiệp";

        //Sendmail flag
        public static string FLG_SCREEN_INFO = "InvoiceInfoScreen";
        public static string FLG_SCREEN_EDIT = "InvoiceEditScreen";

        //Check status texbox SubTotal
        public static string FLG_ActiveTotalAmount = "false";


        #region Message Box Content 
        public const string b_tax_code_ERROR = "Mã số thuế không hợp lệ tiêu chuẩn Việt Nam";
		public const string Null_code_ERROR = "Vui lòng không bỏ trống ";
		public const string NotPositivedouble_ERROR = "Vui lòng nhập số lớn hơn không vào ";

		public const string Internet_ERROR = "Vui lòng kiểm tra kết nối Internet !!!";
		#endregion

		#region Server Code Response
		public static int Code_Successful = 50000;
		#endregion


		//TaxList
		public static ObservableCollection<string> GetTaxCodeList()
        {
            return new ObservableCollection<string>()
            {
                "Không chịu thuế",
                "0%",
                "5%",
                "10%"
            };
        }
        
        //Payment list
        public static ObservableCollection<PaymentMethod> GetPaymentList()
        {
            return new ObservableCollection<PaymentMethod>() {

                new PaymentMethod
                {
                      id = 0,
                      name = "Tiền mặt"
                },
                new PaymentMethod
                {
                    id = 1,
                    name =  "Chuyển khoản"
                },
                new PaymentMethod
                {
                    id = 2,
                    name =  "Tiền mặt/Chuyển khoản"
                },
                  new PaymentMethod
                {
                    id = 3,
                    name =  "Thẻ tín dụng"
                }

            };

        }

        //Money list
        public static ObservableCollection<string> GetMoneyUnit()
        {
            return new ObservableCollection<string>() {
                "VND",
                "USD",
                "EUR",
                "JPY"
            };
        }

        //Year list
        public static ObservableCollection<int> GetYear()
        {
            return new ObservableCollection<int>() {
                2019,
                2018,
                2017,
                2016
            };
        }
        //Month list
        public static ObservableCollection<int> GetMonth()
        {
            return new ObservableCollection<int>() {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10,
                11,
                12
            };
        }

        //Period List
        public static ObservableCollection<int> GetPeriod()
        {
            return new ObservableCollection<int>() {
                1,
                2,
                3,
                4
            };
        }

    }

    public  class ConverterVariable
    {
		public static string NUMBER_BEHIND_DOT { get; set; } = "0";
		
		
    }

    public class UiVisibility
    {
        public static string VISIBLE { get; set; }      = "Visible";
        public static string COLLAPSED { get; set; }    = "Collapsed";
        public static string HIDDEN { get; set; }       = "Hidden";

    }


}
