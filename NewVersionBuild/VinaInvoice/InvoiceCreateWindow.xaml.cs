using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VinaInvoice.Model;
using VinaInvoice.ViewModel;

namespace VinaInvoice
{
    /// <summary>
    /// Interaction logic for InvoiceCreateWindow.xaml
    /// </summary>
    public partial class InvoiceCreateWindow : Window
    {
        private InvoiceDraftViewModel _parent;
        public InvoiceCreateViewModel viewModel;
        public InvoiceCreateWindow()
        {
            viewModel = new InvoiceCreateViewModel();
            DataContext = viewModel;

            InitializeComponent();


            //	viewModel = this.DataContext as InvoiceCreateViewModel;
            //viewModel.ProductList = new System.Collections.ObjectModel.ObservableCollection<ProductUI>();          
            // viewModel.Init();

            var Profile = (ProfileData)ApplicationCache.GetItem("profile");      
            if (Profile.IsAdmin)
            {
                this.KeepButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.KeepButton.Visibility = Visibility.Collapsed;

            }

            this.Closed += (sender, e) =>
			{                
                if (_parent != null)
                {
                    int[] status = { 0 };
                    int[] state_of_bill = { 0, 1, 3, 4 };
                    _parent.LoadListInvoice(status, state_of_bill);
                }
            };
            this.GotFocus += (sender, e) =>
             {
                 viewModel.RefreshProduct();
             };

        }


		public void SetParent(InvoiceDraftViewModel context)
        {
            _parent = context;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }   
     
    }
}
