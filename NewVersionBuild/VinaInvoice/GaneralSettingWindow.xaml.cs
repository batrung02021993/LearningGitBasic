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
    /// Interaction logic for GaneralSettingWindow.xaml
    /// </summary>
    public partial class GaneralSettingWindow : Window
    {
        public GenaralSettingViewModel viewModel;
        public GaneralSettingWindow()
        {
            GenaralSettingViewModel genaralSetgtingViewModel = new GenaralSettingViewModel();
            viewModel = new GenaralSettingViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void TabablzControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
