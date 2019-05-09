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

namespace VinaInvoice.ViewModel
{
	public class DefaultFormSerialViewModel : BaseViewModel
	{
		private IEnumerable<Form> _form;
		public IEnumerable<Form> FormLists { get { return _form; } set { _form = value; OnPropertyChanged(); } }
		private Form _sform;
		public Form sform
		{
			get
			{ return _sform; }
			set
			{
				SerialListUI.Clear();
				if (value == null) return;
				_sform = value;
				
				if (SerialList == null) getSerialList();
				if (SerialList == null) return;
					foreach (var s in SerialList)
				{
					if (s.form_id == sform.id) SerialListUI.Add(s);
				}
				sserial = SerialListUI.FirstOrDefault();

			}
		}

		private IEnumerable<InvoiceSerial> _serial;
		public IEnumerable<InvoiceSerial> SerialList { get { return _serial; } set { _serial = value; OnPropertyChanged(); } }
		private ObservableCollection<InvoiceSerial> _serialUI = new ObservableCollection<InvoiceSerial>();
		public ObservableCollection<InvoiceSerial> SerialListUI { get { return _serialUI; } set { _serialUI = value; OnPropertyChanged(); } }
		private InvoiceSerial _sserial;
		public InvoiceSerial sserial
		{
			get { return _sserial; }
			set
			{
				_sserial = value;
				OnPropertyChanged();
			}
		}

		private bool _sendmail = true;
		public bool isOk = false;
		public bool Sendmail
		{
			get => _sendmail;
			set
			{
				_sendmail = value;
				OnPropertyChanged();
			}
		}

		public ICommand Ok { get; set; }
		public ICommand Cancel { get; set; }



		public DefaultFormSerialViewModel()
		{
            try
            {
                isOk = false;
                getFormList();
                getSerialList();

			Cancel = new RelayCommand<Window>((p) => { return true; }, (p) => { if (p != null)
				{
					isOk = false;
					p.Close();
				} });

			Ok = new RelayCommand<Window>((p) =>
			{
				return true;
			}, (p) =>
			{
				isOk = true;
				p.Close();
			});
		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}


		public void getFormList()
		{
            try
            {
                var _Repository = new InvoiceFormRepository();

			_Repository.Page = 1;
			FormLists = _Repository.GetList();
			var listtemp = FormLists.ToList();

			FormLists = listtemp;
			if(FormLists.Count() > 0) sform = FormLists.FirstOrDefault();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		public void getSerialList()
		{
            try
            {
                var _Repository = new InvoiceSerialRepository();
                _Repository.Page = 1;
                SerialList = _Repository.GetList();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

	}
}
