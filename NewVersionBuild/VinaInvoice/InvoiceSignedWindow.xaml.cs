using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for InvoiceSignedWindow.xaml
    /// </summary>
    public partial class InvoiceSignedWindow : Window
    {
        public InvoiceSignedViewModel viewModel;
        public InvoiceSignedWindow()
        {
            viewModel = new InvoiceSignedViewModel();
            DataContext = viewModel;

            InitializeComponent();

			//var model = this.DataContext as InvoiceSignedViewModel;
   //         model.LoadListInvoice();
 
        }

		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }


}
