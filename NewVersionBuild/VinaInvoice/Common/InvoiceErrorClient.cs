using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Model;

namespace VinaInvoice.Common
{
    public class InvoiceErrorClient
    {

        public static void invoiceErrorClient(Exception ex, bool isDialog)
        {
            if (isDialog == true)
            {
                System.Windows.MessageBox.Show(Common.Message.MSS_ERROR, Common.Message.MSS_DIALOG_TITLE_ALERT);
            }

            // write log to file
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "LOG_ERROR_CLIENT.txt";
            //FileStream path = new FileStream(filePath, FileMode.Create);
            //using (StreamWriter writer = new StreamWriter(path, Encoding.UTF8))

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();


                writer.WriteLine("Type : " + ex.GetType().FullName);
                writer.WriteLine("Message : " + ex.Message);
                writer.WriteLine("StackTrace : " + ex.StackTrace);


            }


            // Write log to email
            string type = ex.GetType().FullName;
            string message = ex.Message;
            string stackTrace = ex.StackTrace;

            // var profile = (ProfileData)ApplicationCache.GetItem("profile");
            var Enterprise_Config_Detail = (Enterprise_ConfigData)ApplicationCache.GetItem("enterprise_config_Detail");
            if (Enterprise_Config_Detail == null)
            {
                return;
            }
            else
            {
                string senderID = Enterprise_Config_Detail.mail_user_name;
                string senderPassword = Enterprise_Config_Detail.mail_password;
                string receiverID = "unggiadrive@gmail.com";

                string subject = "VINAINVOICE EXCEPTION";
                string body = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Template/send_exception.html", Encoding.UTF8);

                body = body.Replace("@EMAIL@", Enterprise_Config_Detail.mail_user_name)
                .Replace("@ID@", Enterprise_Config_Detail.id)
                .Replace("@TIME@", DateTime.Now.ToString())
                .Replace("@TYPE@", type)
                .Replace("@MESSAGE@", message)
                .Replace("@STACKTRACE@", stackTrace);

                try
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(receiverID);
                    mail.From = new MailAddress(senderID);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = Enterprise_Config_Detail.mail_smtp_server;
                    smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
                    smtp.Port = Enterprise_Config_Detail.mail_port;
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    Console.Write("Email Sent Successfully");
                }
                catch { }
            }
        }
    }
}
