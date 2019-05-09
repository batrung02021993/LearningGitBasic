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
    /// Interaction logic for ProductCreateWindow.xaml
    /// </summary>
    public partial class ProductCreateWindow : Window
    {
        ProductCreateViewModel viewmodel;

        public ProductCreateWindow(ProductListViewModel parent)
        {
            InitializeComponent();
            viewmodel = this.DataContext as ProductCreateViewModel;
            viewmodel._parent = parent;
            
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AutoCompleteBox_TextChanged(object sender, RoutedEventArgs e)
        {
            viewmodel.UpdateUnitName( this.AcbxUnitName.Text);
        }
    }
}
