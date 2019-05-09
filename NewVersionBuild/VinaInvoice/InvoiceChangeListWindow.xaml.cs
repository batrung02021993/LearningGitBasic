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
    /// Interaction logic for InvoiceChangeListWindow.xaml
    /// </summary>
    public partial class InvoiceChangeListWindow : Window
    {
        InvoiceChangeModel ViewModel;
        public InvoiceChangeListWindow()
        {
            InitializeComponent();
            ViewModel = this.DataContext as InvoiceChangeModel;
            ViewModel.LoadListInvoice();

            this.Closed += (sender, e) =>
            {
                ViewModel = new InvoiceChangeModel();                
            };
        }


    }
}
