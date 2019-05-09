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
using VinaInvoice.ViewModel;

namespace VinaInvoice
{
    /// <summary>
    /// Interaction logic for InvoiceDeleteWindow.xaml
    /// </summary>
    public partial class InvoiceDeleteWindow : Window
    {
        public InvoiceDeleteWindow()
        {
            InitializeComponent();
            var model = this.DataContext as InvoiceDeletedViewModel;

            model.LoadListInvoice();
            this.Closed += (sender, e) =>
            {
                model = new InvoiceDeletedViewModel();
            };

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
