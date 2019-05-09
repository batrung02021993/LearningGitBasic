using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using VinaInvoice.Common;
using VinaInvoice.Data;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand GaneralSettingCommand { get; set; }
        public ICommand ImportExcelCommand { get; set; }

        public ICommand InvoiceChangeCommand { get; set; }
        public ICommand InvoiceReportCommand { get; set; }
        public ICommand InvoiceSignedCommand { get; set; }
        public ICommand InvoiceCreateCommand { get; set; }
        public ICommand InvoiceDraftCommand { get; set; }
        public ICommand InvoiceTemplateCommand { get; set; }
        public ICommand InvoiceSerialCommand { get; set; }
        public ICommand InvoiceDeleteCommand { get; set; }

        public ICommand ItemListCommand { get; set; }
        public ICommand CustomerListCommand { get; set; }
        public ICommand UserListCommand { get; set; }
        public ICommand UserInfoCommand { get; set; }
        public ICommand SignOutCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        private string Path = AppDomain.CurrentDomain.BaseDirectory;

        // mọi thứ xử lý sẽ nằm trong này
        public MainViewModel()
        {
            try
            {
            Init();
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
                Isloaded = true;
				if (p == null)
					return;
				p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();

				if (loginWindow.DataContext == null)
					return;
				var loginVM = loginWindow.DataContext as LoginViewModel;

				if (loginVM.IsLogin)
				{
					p.Show();
					var profile = (ProfileData)ApplicationCache.GetItem("profile");					
				}
				else
				{
					p.Close();
				}				
			}
           );

            CreateCommand           = new RelayCommand<object>((p) => { return true; }, (p) => { UserListWindow wd = new UserListWindow(); wd.ShowDialog(); });
            GaneralSettingCommand   = new RelayCommand<object>((p) => { return true; }, (p) => { GaneralSettingWindow wd = new GaneralSettingWindow(); wd.ShowDialog(); });
            ImportExcelCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Excel_Utils.InvoiceImportExcelDraft(); });
            SignOutCommand          = new RelayCommand<object>((p) => { return true; }, (p) => { UserListWindow wd = new UserListWindow(); wd.ShowDialog(); });

            InvoiceReportCommand    = new RelayCommand<object>((p) => { return true; }, (p) => { BC26Window wd = new BC26Window(); wd.ShowDialog(); });
            InvoiceChangeCommand    = new RelayCommand<object>((p) => { return true; }, (p) => { InvoiceChangeListWindow wd = new InvoiceChangeListWindow(); wd.ShowDialog(); });
            InvoiceCreateCommand    = new RelayCommand<object>((p) => { return true; }, (p) => { InvoiceCreateWindow wd = new InvoiceCreateWindow(); wd.ShowDialog(); });
            InvoiceSerialCommand    = new RelayCommand<object>((p) => { return true; }, (p) => { InvoiceSerialWindow wd = new InvoiceSerialWindow(); wd.ShowDialog(); });
            InvoiceDeleteCommand    = new RelayCommand<object>((p) => { return true; }, (p) => { InvoiceDeleteWindow wd = new InvoiceDeleteWindow(); wd.ShowDialog(); });
            InvoiceTemplateCommand  = new RelayCommand<object>((p) => { return true; }, (p) => { InvoiceTemplateWindow wd = new InvoiceTemplateWindow(); wd.ShowDialog(); });
            InvoiceDraftCommand     = new RelayCommand<object>((p) => { return true; }, (p) => { InvoiceDraftWindow wd = new InvoiceDraftWindow(); wd.ShowDialog(); });
            InvoiceSignedCommand    = new RelayCommand<object>((p) => { return true; }, (p) => { InvoiceSignedWindow wd = new InvoiceSignedWindow(); wd.ShowDialog(); });

            ItemListCommand         = new RelayCommand<object>((p) => { return true; }, (p) => { ProductListWindow wd = new ProductListWindow(); wd.ShowDialog(); });
            CustomerListCommand     = new RelayCommand<object>((p) => { return true; }, (p) => { CustomerListWindow wd = new CustomerListWindow(); wd.ShowDialog(); });
            UserListCommand         = new RelayCommand<object>((p) => { return true; }, (p) => { UserListWindow wd = new UserListWindow(); wd.ShowDialog(); });
            UserInfoCommand         = new RelayCommand<object>((p) => { return true; }, (p) => { UserInfoWindow wd = new UserInfoWindow(); wd.ShowDialog(); });
            CloseCommand            = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void Init()
        {
            try
            {
                System.IO.File.WriteAllText(Path + "EditMonyAmount.txt", "false");
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void SetStatusBar()
        {
            try
            {
                var enterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");
                if (enterpriseData != null)
                    StatusBarString = enterpriseData.company_name;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
    }
}
