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
    /// Interaction logic for CustomerCreateWindow.xaml
    /// </summary>
    public partial class CustomerCreateWindow : Window
    {
        public CustomerCreateWindow(CustomerListViewModel parent)
        {
            InitializeComponent();
			var listviewmodel = this.DataContext as CustomerCreateViewModel;
			listviewmodel._parent = parent;

            this.Closed += (sender, e) =>
            {           

                if (parent != null)
                {
                    int[] status = { 0 };
                    int[] state_of_bill = { 0, 1, 3, 4 };
                    listviewmodel._parent.LoadList(1);
                }
            };
        }
    }
}
