

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VinaInvoice.Common;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.ViewModel
{

	public class InvoiceSerialViewModel : BaseViewModel
	{
		InvoiceSerialRepository _Repository = new InvoiceSerialRepository();

		public bool Isloaded = false;
		public ICommand NextPageCommand { get; set; }
		public ICommand PreviousPageCommand { get; set; }

		private ObservableCollection<InvoiceSerial> _DataList;
		public ObservableCollection<InvoiceSerial> DataList { get => _DataList; set { _DataList = value; OnPropertyChanged(); } }
        private InvoiceSerial _serialItemSelected;
        public InvoiceSerial SerialItemSelected {
            get => _serialItemSelected;
            set {
             
         
                _serialItemSelected = value;
                OnPropertyChanged();

               
            }
        }

        private int _page = 1;
		public int Page { get => _page; set { _page = value; OnPropertyChanged(); } }


		public InvoiceSerialViewModel()
		{
            StatusBarString = Const.STATUS_BAR_STRING;

            try
            {
            LoadList(Page);
			NextPageCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
				Page++;
				LoadList(Page);
			}
 );

			PreviousPageCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
				if (Page > 1)
				{
					Page--;
					LoadList(Page);
				}
				else
				{
					MessageBox.Show("You are in the first page !!!", Message.MSS_DIALOG_TITLE_ALERT);
				}
			}
		 );
		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

		public void LoadList(int page)
        {
            try
            {
			IEnumerable<InvoiceSerial> list;
			_Repository.Page = page;
			list = _Repository.GetList();

			if (list.Count() > 0)
			{
				DataList = new ObservableCollection<InvoiceSerial>();
				int count = 0;
				foreach (var e in list)
				{
					count++;
						
					DataList.Add(e);
				}
			}
			else
			{
				MessageBox.Show("You are in the last page !!!", Message.MSS_DIALOG_TITLE_ALERT);
			}


		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
    }
}