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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginViewModel viewModel;
        public LoginWindow()
        {
            InitializeComponent();
            viewModel = this.DataContext as LoginViewModel;
        }

        private void FloatingPasswordBox_TextInput_1(object sender, TextCompositionEventArgs e)
		{

		}

        private void AcbxUserName_TextChanged(object sender, RoutedEventArgs e)
        {
            viewModel.UpdateUseName(this.AcbxUserName.Text);
        }
    }
}
