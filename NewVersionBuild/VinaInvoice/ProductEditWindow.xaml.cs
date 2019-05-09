using System.Windows;
using VinaInvoice.Model;
using VinaInvoice.ViewModel;

namespace VinaInvoice
{
    /// <summary>
    /// Interaction logic for ProductEditWindow.xaml
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        public ProductEditWindow(Product _product)
        {
            InitializeComponent();

            var Viewmodel = this.DataContext as ProductEditViewModel;
            Viewmodel.Product = _product;
        }
    }
}
