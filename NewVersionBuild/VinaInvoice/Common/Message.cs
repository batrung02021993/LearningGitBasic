using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Common
{
    public static class Message
    {
        
        //Message get permisson
        public static string MSS_ALERT_PERMISSON_AD_SUCCESS = "Kích hoạt quyền Admin thành công";
        public static string MSS_ALERT_PERMISSON_SA_SUCCESS = "Kích hoạt quyền Supper Admin thành công";
        public static string MSS_ALERT_PERMISSON_FAIL = "Lưu thành công";

        //Message invoice
        public static string MSS_ALERT_INVOICE_CONVERT_SUCCESS  = "Chuyển đổi hóa đơn thành công";
        public static string MSS_ALERT_INVOICE_CONVERT_FAIL     = "Chuyển đổi hóa đơn thất bại";

        //Message export
        public static string MSS_ALERT_EXPORT_FILE_SUCCESS = "Xuất file exel thành công!";
        public static string MSS_ALERT_EXPORT_FILE_FAIL = "Xuất file exel thất bại!";

        //Email Message
        public static string MSS_SEND_EMAIL_NO_CONFIG = "Vui lòng cài đặt thông tin gửi email!";
        public static string MSS_SEND_EMAIL_FAIL = "Gửi email thất bại, vui lòng thử lại...";
        public static string MSS_SEND_EMAIL_SUCCESS = "Đã gửi email thành công!";

        //Ganeral Setting
        public static string MSS_SAVE_SETTING_CONFIG_SUCCESS = "Lưu thông tin thành công!";

        //Message create invoice
        public static string MSS_CREATE_INVOICE_NO_PRODUCT = "Sản phẩm không được bỏ trống!";

        //Message Login
        public static string MSS_LOGIN_WRONG_PASS = "Sai mật khẩu, vui lòng thử lại!";
        public static string MSS_LOGIN_WRONG_EMAIL = "Email không tồn tại!";
        public static string MSS_LOGIN_SERVER_INTERNAL = "Server đang bảo trì, vui lòng thử lại sau!";
        public static string MSS_LOGIN_FAIL = "Đăng nhập thất bại, vui lòng thử lại!";
        public static string MSS_LOGIN_NO_NETWORK = "Kết nối mạng thất bại, vui lòng kiểm tra đường truyền internet!";

        //Message Alert
        public static string MSS_DIALOG_TITLE_ALERT = "Vina Invoice thông báo";

        //Message Draft Invoice
        public static string MSS_INV_SIGNED_SELECTED_NO_SUCCESS = "Ký không thành công, Vui lòng kiểm trả lại!";
        public static string MSS_INV_SIGNED_SELECTED_FAIL = "Ký thất bại, vui lòng thử lại!";

        //Message Invoice signed
        public static string MSS_INV_SIGNED_SIGNED = "Ký hóa đơn ";

        // Import Message
        public static string MSS_IMPORT_WRONG_FORMAT = "Định dạng File bị lỗi, vui lòng kiểm tra lại";

        //Error Exception
        public static string MSS_ERROR = "Đã xảy ra lỗi hệ thống, vui lòng liên hệ với quản trị viên!";

    }
}
