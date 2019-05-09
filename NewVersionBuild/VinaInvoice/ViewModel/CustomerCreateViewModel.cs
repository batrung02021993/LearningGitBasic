

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VinaInvoice.Common;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;

namespace VinaInvoice.ViewModel
{
	public class CustomerCreateViewModel : BaseViewModel
	{
		public CustomerListViewModel _parent;
		CustomerRepository _Repository = new CustomerRepository();

		public bool Isloaded = false;

		public ICommand SaveCommand { get; set; }
		public ICommand CloseCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }

        

        private Customer _Customer = new Customer();
		public Customer Customer { get => _Customer; set { _Customer = value; OnPropertyChanged(); } }


		public CustomerCreateViewModel()
		{
            StatusBarString = Const.STATUS_BAR_STRING;


            SaveCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SaveCommandFunction(); });

			CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
				p.Close();
				_parent.LoadList(1);
			}
			 );
            CloseWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
                p.Close();
                _parent.LoadList(1);
            }
             );

        }

		private void SaveCommandFunction()
		{
			if(!Validate()) return;
		
			MessageBox.Show(_Repository.Create(Customer), Message.MSS_DIALOG_TITLE_ALERT);

		}

		private bool Validate()
		{
            try
            {
                if (Customer.CompanyTaxCode != null && Customer.CompanyTaxCode.Length < 10)
                {
                    MessageBox.Show("Mã số thuế không hợp lệ theo tiêu chuẩn Việt Nam", Message.MSS_DIALOG_TITLE_ALERT);
                    return false;
                }
                if (String.IsNullOrWhiteSpace(Customer.DisplayName) && String.IsNullOrWhiteSpace(Customer.CompanyName) && String.IsNullOrWhiteSpace(Customer.CompanyTaxCode))
                {
                    MessageBox.Show("Không được bỏ trống đồng thời mã số thuế, tên khách hàng và tên công ty", Message.MSS_DIALOG_TITLE_ALERT);
                    return false;
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
            return true;
        }

	}
}