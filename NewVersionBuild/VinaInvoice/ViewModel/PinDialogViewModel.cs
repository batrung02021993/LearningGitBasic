using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VinaInvoice.Common;

namespace VinaInvoice.ViewModel
{
	public class PinDialogViewModel : BaseViewModel
	{
		
			public ICommand PasswordChangedCommand { get; set; }
		public ICommand OKCommand { get; set; }
		public ICommand CancelCommand { get; set; }
		public bool OK = false;
		public bool iscorected = false;
		private string _Pin = "";
		public string Pin
		{
			get => _Pin;
			set
			{
				if (_Pin != value)
				{
					_Pin = value;
					OnPropertyChanged();
				}
			}
		}
		public PinDialogViewModel()
		{
            try
		{
			PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Pin = p.Password.Trim(); });

			OKCommand = new RelayCommand<Window>((p) =>
			{				
				return true;
			}, (p) =>
			{
				OK = true;
				if (p != null)
				{
					p.Close();
				}
			});

			CancelCommand = new RelayCommand<Window>((p) =>
			{				
				return true;
			}, (p) =>
			{
				Pin = "";
				OK = false;
				if (p != null)
				{
					p.Close();
				}
			});

		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
           
		}

	}
}
