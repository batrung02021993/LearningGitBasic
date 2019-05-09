using System;
using System.Windows;
using System.Windows.Input;
using VinaInvoice.Common;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;

namespace VinaInvoice.ViewModel
{
	public class CustomerEditViewModel : BaseViewModel
	{

        private Customer _customer = new Customer();
        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged();
            }
        }

        #region Comand
        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        #endregion

        CustomerUpdateRepository _Repository = new CustomerUpdateRepository();
        


        public CustomerEditViewModel()
        {
            SaveCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SaveCommandFunction(); });

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
                p.Close();
            }
             );
        }

        private void SaveCommandFunction()
        {
            try
            {
                if (!Validate()) return;

                _Repository.Update(Customer);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private bool Validate()
        {
            try
            {

            if (string.IsNullOrEmpty(Customer.CompanyTaxCode) == false && Customer.CompanyTaxCode.Length < 10)
            {
                MessageBox.Show("Mã số thuế không hợp lệ theo tiêu chuẩn Việt Nam", Message.MSS_DIALOG_TITLE_ALERT);
                return false;
            }
            if (string.IsNullOrEmpty(Customer.CompanyName) == true && string.IsNullOrEmpty(Customer.CompanyTaxCode) == false
                || string.IsNullOrEmpty(Customer.CompanyName) == false && string.IsNullOrEmpty(Customer.CompanyTaxCode) == true)
            {
                MessageBox.Show("Không được bỏ trống đồng thời tên công ty và mã số thuế", Message.MSS_DIALOG_TITLE_ALERT);
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
