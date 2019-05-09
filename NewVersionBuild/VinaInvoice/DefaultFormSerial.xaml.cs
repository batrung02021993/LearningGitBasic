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
	/// Interaction logic for DefaultFormSerial.xaml
	/// </summary>
	public partial class DefaultFormSerial : Window
	{
		public DefaultFormSerial()
		{
			InitializeComponent();
			var defaultFormSerialVM = this.DataContext as DefaultFormSerialViewModel;
			defaultFormSerialVM.isOk = false;
			defaultFormSerialVM.Sendmail = true;


		}
	}
}
