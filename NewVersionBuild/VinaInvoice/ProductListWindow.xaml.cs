using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ProductListWindow.xaml
    /// </summary>
    public partial class ProductListWindow : Window
    {
        ProductListViewModel model;

        public ProductListWindow()
        {
            InitializeComponent();
            model = this.DataContext as ProductListViewModel;
            model.LoadListProduct(1);

            this.Closed += (sender, e) =>
            {
                model = new ProductListViewModel();
            };

        }

    }
}
