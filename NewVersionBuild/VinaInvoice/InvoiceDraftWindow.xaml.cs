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
    /// Interaction logic for InvoiceDraftWindow.xaml
    /// </summary>
    public partial class InvoiceDraftWindow : Window
    {
        public InvoiceDraftViewModel viewModel;
        public InvoiceDraftWindow()
        {
            
            InitializeComponent();
            viewModel = this.DataContext as InvoiceDraftViewModel;

            int[] status = { 0 };
            int[] state_of_bill = { 0, 1, 3, 4 };
            viewModel.LoadListInvoice(status, state_of_bill);

            var Profile = (ProfileData)ApplicationCache.GetItem("profile");
            if (Profile.IsAdmin || Profile.IsSupperAdmin)
            {
                this.CbKeepInvoice.Visibility = Visibility.Visible;
            }
            else
            {
                this.CbKeepInvoice.Visibility = Visibility.Collapsed;

            }

            this.Closed += (sender, e) =>
            {
                var model = this.DataContext as InvoiceDraftViewModel;
                model = new InvoiceDraftViewModel();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
