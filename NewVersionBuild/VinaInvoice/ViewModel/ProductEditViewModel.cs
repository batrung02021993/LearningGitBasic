using System;
using System.Windows;
using System.Windows.Input;
using VinaInvoice.Common;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;

namespace VinaInvoice.ViewModel
{
	public class ProductEditViewModel : BaseViewModel
	{

        private Product _product = new Product();
        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged();
            }
        }

        #region Comand
        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        #endregion

        ProductUpdateRepository _Repository = new ProductUpdateRepository();
        


        public ProductEditViewModel()
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

            _Repository.Update(Product);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private bool Validate()
        {
            return true;
        }
    }
}
