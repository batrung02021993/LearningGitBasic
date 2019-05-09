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
    /// Interaction logic for InvoiceKeepWindow.xaml
    /// </summary>
    public partial class InvoiceKeepWindow : Window
    {
        public InvoiceEditViewModel Viewmodel;
        private InvoiceDraftViewModel _parent;

        public InvoiceKeepWindow(bool IsKeepInvoice, Invoice invoice)
        {
  
            Viewmodel = new InvoiceEditViewModel();
            DataContext = Viewmodel;

            Viewmodel.IsKeepInvoice = IsKeepInvoice;
            Viewmodel.InvoiceCreate = invoice;

            InitializeComponent();


            var Profile = (ProfileData)ApplicationCache.GetItem("profile");

            if (Profile.IsAdmin)
            {
                this.KeepButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.KeepButton.Visibility = Visibility.Collapsed;

            }

            this.GotFocus += (sender, e) =>
            {
                Viewmodel.RefreshProduct();
            };

            this.Closed += (sender, e) =>
            {
                if (_parent != null)
                {
                    int[] status = { 0 };

                    if (IsKeepInvoice)
                    {
                        status[0] = 2;
                    }

                    int[] state_of_bill = { 0, 1, 3, 4 };
                    _parent.LoadListInvoice(status, state_of_bill);
                }

            };
        }

        public void setParent(InvoiceDraftViewModel invoiceDraftViewModel)
        {
            _parent = invoiceDraftViewModel;
        }

    }
}
