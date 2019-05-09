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
  
	public class InvoiceTemplateViewModel : BaseViewModel
	{
		InvoiceFormRepository _Repository = new InvoiceFormRepository();

		public bool Isloaded = false;
		public ICommand NextPageCommand { get; set; }
		public ICommand PreviousPageCommand { get; set; }

		private ObservableCollection<Invoice_Form_View> _DataList;
		public ObservableCollection<Invoice_Form_View> DataList { get => _DataList; set { _DataList = value; OnPropertyChanged(); } }

		private int _page = 1;
		public int Page { get => _page; set { _page = value; OnPropertyChanged(); } }

		public InvoiceTemplateViewModel()
		{
            try
            {
            StatusBarString = Const.STATUS_BAR_STRING;

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
			IEnumerable<Form> list;
			_Repository.Page = page;
			list = _Repository.GetList();

			if (list.Count() > 0)
			{
				DataList = new ObservableCollection<Invoice_Form_View>();
				int count = 0;
				foreach (Form e in list)
				{					
					count++;
					var iv = new Invoice_Form_View()
					{
						create_by = e.create_by,
						id = e.id,
						form_name = e.form_name,
						STT = count
					};

					if (e._using) iv._using = "Có thể sử dụng";
					else iv._using = "Đang chờ xét duyệt";
					double timestamp = e.create_time;
					System.DateTime dateTime = new System.DateTime(1970, 1, 1, 7, 0, 0, 0);
					dateTime = dateTime.AddSeconds(timestamp);
					iv.create_time = dateTime.ToString();
					DataList.Add(iv);
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


	public class Invoice_Form_View
	{
		public int STT { get; set; }
		public string id { get; set; }
		public string form_name { get; set; }
		public string _using { get; set; }
		public string create_by { get; set; }
		public string create_time { get; set; }
	}

}
