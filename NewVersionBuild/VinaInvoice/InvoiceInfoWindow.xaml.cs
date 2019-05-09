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
    /// Interaction logic for InvoiceInfoWindow.xaml
    /// </summary>
    public partial class InvoiceInfoWindow : Window
    {
        private InvoiceSignedViewModel _parent;
        public InvoiceInfoWindow(Invoice _Invoice)
        {
           
            InitializeComponent();
			var Viewmodel = this.DataContext as InvoiceInfoViewModel;
            this.TbxFile.Text = "";

            Viewmodel.Invoice = _Invoice;

            var Profile = (ProfileData)ApplicationCache.GetItem("profile");
            if (Profile.IsSupperAdmin)
            {
                this.ResignedButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.ResignedButton.Visibility = Visibility.Collapsed;

            }

            this.Closed += (sender, e) =>
            {
                Viewmodel = new InvoiceInfoViewModel();
                if (_parent != null)
                {
                    _parent.LoadListInvoice();
                }
            };
        }

        public void SetParent(InvoiceSignedViewModel context)
        {
            _parent = context;
        }
    }
}
