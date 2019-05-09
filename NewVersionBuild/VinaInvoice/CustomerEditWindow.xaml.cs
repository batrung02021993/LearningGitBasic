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
    /// Interaction logic for CustomerEditWindow.xaml
    /// </summary>
    public partial class CustomerEditWindow : Window
    {
        public CustomerEditWindow(Customer _customer)
        {
            InitializeComponent();

            var Viewmodel = this.DataContext as CustomerEditViewModel;
            Viewmodel.Customer = _customer;
        }
    }
}
