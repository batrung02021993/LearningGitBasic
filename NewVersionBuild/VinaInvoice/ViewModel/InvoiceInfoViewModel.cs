using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Xsl;
using TuesPechkin;
using VinaInvoice.Common;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.ViewModel
{
	public class InvoiceInfoViewModel : BaseViewModel
	{
		#region Comand
		public ICommand CloseCommand { get; set; }
		public ICommand GetScanFileCommand { get; set; }
		public ICommand ExecuteCommand { get; set; }
		public ICommand ResigedCommand { get; set; }
		public ICommand SendEmailCommand { get; set; }
		public ICommand ViewHTMLCommand { get; set; }
		public ICommand ViewPDFCommand { get; set; }
		public ICommand ViewXMLCommand { get; set; }
		public ICommand ViewDocumentCommand { get; set; }

		#endregion

		#region Internal Object
		public bool SendEmailStatus { get; set; } = false;
		public bool ViewHTMLStatus { get; set; } = false;
		public bool ViewPDStatus { get; set; } = false;
		public bool ViewXMLStatus { get; set; } = false;
		public bool ViewDocumentStatus { get; set; } = false;
		private string Reportpath = "CurrentReport.pdf";
	
		private Invoice _invoice = new Invoice();
		public Invoice Invoice { get => _invoice; set {
                _invoice = value;
                SignDay = DateTimeConvert.GetdatetimeFromStamp((double)Double.Parse(Invoice.invoice_sign_date)).ToString(("dd/MM/yyyy")); 
                OnPropertyChanged(); } }


		private bool canExcute = false;
		private InvoiceEditViewModel keepViewModel = new InvoiceEditViewModel();

      
        #endregion

        #region Binding Object
        private string _FilesCanPath = null;
		public string FilesCanPath { get => _FilesCanPath; set { _FilesCanPath = value; OnPropertyChanged(); } }

		public ObservableCollection<CBMethod> MajorLists
		{
			get { return _majorLists; }
			set { _majorLists = value; OnPropertyChanged("SortLists"); }
		}
		public CBMethod SmajorList
		{
			get { return _sMajorLists; }
			set { _sMajorLists = value;
                UpdateResonContent(_sMajorLists.Name);
                OnPropertyChanged(); }
		}

		private string _SignDay;
		public string SignDay { get => _SignDay; set { _SignDay = value; OnPropertyChanged(); } }



		private ObservableCollection<CBMethod> _majorLists;
		private CBMethod _sMajorLists;

		#endregion

		public InvoiceInfoViewModel()
		{
            try
            {
                StatusBarString = Const.STATUS_BAR_STRING;

            GetSortList();

            var Profile = (ProfileData)ApplicationCache.GetItem("profile");
          //  ButtonVisibility = Profile.IsSupperAdmin;

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { if (p != null) p.Close(); });

			GetScanFileCommand = new RelayCommand<object>((p) =>
			{
				return true;
			}, (p) =>
			{
				GetScanFile();
			});

			ExecuteCommand = new RelayCommand<Window>((p) =>
			{
				if (Invoice.status == 3 || Invoice.state_of_bill == 2) return false;
				if (SmajorList.Name == Const.MAJOR_INVOICE_CONVERT) return true;
				return canExcute;
			}, (p) =>
			{
				// Validate
				if (SmajorList == null) return;
				string c = SmajorList.Name;

				if (c != Const.MAJOR_INVOICE_CONVERT)
				{
					if (Invoice.document_no == "" || Invoice.document_no == null)
					{
						MessageBox.Show("Vui lòng nhập số biên bản !!!", "Vina invoice thông báo:");
						return;
					}
				}


				if (c == Const.MAJOR_INVOICE_DELETE) DeleteInvoice();
				else if (c == Const.MAJOR_INVOICE_REPLACE) ChangeInvoice();
				else if (c == Const.MAJOR_INVOICE_EDIT) AdjustInvoice();
				else if (c == Const.MAJOR_INVOICE_EDIT_INFO) AdjustInvoiceInfo();
				else if (c == Const.MAJOR_REPORT_EDIT) AdjustInvoiceReport();
				else if (c == Const.MAJOR_INVOICE_CONVERT)
				{
					Invoice.state_of_bill = 2;
					ConvertInvoice();
				}

				FilesCanPath = "";
				canExcute = false;
				p.Close();

			});

			ResigedCommand = new RelayCommand<object>((p) =>
			{

				return true;
			}, (p) =>
			{
				Resign();
			});

			SendEmailCommand = new RelayCommand<object>((p) =>
			{
				ActiveButtonInit();
				return SendEmailStatus;
			}, (p) =>
			{
				Sendmail();
			});

			ViewHTMLCommand = new RelayCommand<object>((p) =>
			{
				ActiveButtonInit();
				return ViewHTMLStatus;
			}, (p) =>
			{
				ViewHTML();
			});

			ViewPDFCommand = new RelayCommand<object>((p) =>
			{
				ActiveButtonInit();
				return ViewPDStatus;
			}, (p) =>
			{
				ViewPDF();
			});

			ViewXMLCommand = new RelayCommand<object>((p) =>
			{
				if (Invoice.status == 3) return false;
				ActiveButtonInit();
				return ViewXMLStatus;
			}, (p) =>
			{
				ViewXML();
			});

			ViewDocumentCommand = new RelayCommand<object>((p) =>
			{
				if (Invoice.status == 3) return false;
				ActiveButtonInit();
				return ViewDocumentStatus;
			}, (p) =>
			{
				ViewDocument();
			});
		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void ActiveButtonInit()
		{
            try
            {
                if (Invoice.state_of_bill == 1)
                {
                    ViewXMLStatus = true;
                    ViewPDStatus = true;
                    ViewHTMLStatus = true;
                    SendEmailStatus = true;
                    ViewDocumentStatus = true;
                }
                else if (Invoice.state_of_bill == 2)
                {
                    ViewXMLStatus = true;
                    ViewPDStatus = true;
                    ViewHTMLStatus = true;
                    SendEmailStatus = true;
                    ViewDocumentStatus = false;
                }
                else if (Invoice.state_of_bill == 3)
                {
                    ViewXMLStatus = true;
                    ViewPDStatus = true;
                    ViewHTMLStatus = true;
                    SendEmailStatus = true;
                    ViewDocumentStatus = true;
                }
                else if (Invoice.state_of_bill == 4)
                {
                    ViewXMLStatus = true;
                    ViewPDStatus = true;
                    ViewHTMLStatus = true;
                    SendEmailStatus = true;
                    ViewDocumentStatus = true;
                }
                else
                {
                    ViewXMLStatus = true;
                    ViewPDStatus = true;
                    ViewHTMLStatus = true;
                    SendEmailStatus = true;
                    ViewDocumentStatus = false;
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void ViewDocument()
		{
            try
            {
			//get document
			InvoiceRestApi apis = new InvoiceRestApi();
			var requets = new InvoiceDocumentRequest()
			{
				document_id = Invoice.document_id
			};
			var response1 = apis.GetDucumentContent(requets);

			if (response1.code != Const.Code_Successful)
			{
				MessageBox.Show(response1.message);
				return;
			}



		   byte[] sPDFDecoded = Convert.FromBase64String(response1.data.document_content);
			File.WriteAllBytes(Reportpath, sPDFDecoded);
			System.Diagnostics.Process.Start(Reportpath);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void ViewXML()
		{
            try
            {
                keepViewModel.InvoiceCreate = Invoice;
                keepViewModel.ViewInvoiceXML();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void ViewPDF()
		{
            try
            {
                keepViewModel.InvoiceCreate = Invoice;
                keepViewModel.ViewInvoicePdf();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

        private void ViewHTML()
		{
            try
            {
                keepViewModel.InvoiceCreate = Invoice;
                keepViewModel.ViewInvoiceHtml();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

        public void Sendmail()
        {
            try
            {
                keepViewModel.InvoiceCreate = Invoice;
                keepViewModel.SendMail();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

        #region business logic
        private void ChangeInvoice()
		{
            try
            {
			// call API to change
			// server will create new draft and send back to clinet the id
			InvoiceRestApi apis = new InvoiceRestApi();
			var requets = new InvoiceChangeRequest()
			{
				document_content = Invoice.document_content,
				document_no = Invoice.document_no,
				id = Invoice.id,
				reason_content = Invoice.reason_content
			};
			var response1 = apis.Change(requets);
			if (response1.code != Const.Code_Successful)
			{
				MessageBox.Show(response1.message);
				return;
			}
			// get invoice Draft by call api 
			var api = new InvoiceRestApi();
			var response2 = api.GetInvoiceDetail(response1.data.invoice_id);
			if (response2.code != Const.Code_Successful)
			{
				MessageBox.Show(response2.message);
				return;
			}

			var invoiceSelectDetail = response2.data;

			//
			InvoiceKeepWindow wd = new InvoiceKeepWindow(false, invoiceSelectDetail); wd.ShowDialog();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void ConvertInvoice()
		{
            try
            {
			keepViewModel.InvoiceCreate = Invoice;
			keepViewModel.ConvertInvoice();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void AdjustInvoiceReport()
		{
            try
            {
			// call API to change
			// server will create new draft and send back to clinet the id
			InvoiceRestApi apis = new InvoiceRestApi();
			var requets = new InvoiceChangeRequest()
			{
				document_content = Invoice.document_content,
				document_no = Invoice.document_no,
				id = Invoice.id,
				reason_content = Invoice.reason_content,
			};
			var response1 = apis.AdjustReport(requets);
			if (response1.code != Const.Code_Successful)
			{
				MessageBox.Show(response1.message);
				return;
			}
			else MessageBox.Show(response1.message);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void AdjustInvoiceInfo()
		{
            try
            {
			// call API to change
			// server will create new draft and send back to clinet the id
			InvoiceRestApi apis = new InvoiceRestApi();
			var requets = new InvoiceChangeRequest()
			{
				document_content = Invoice.document_content,
				document_no = Invoice.document_no,
				id = Invoice.id,
				reason_content = Invoice.reason_content,
				state_of_bill = 4
			};
			var response1 = apis.Adjust(requets);
			if (response1.code != Const.Code_Successful)
			{
				MessageBox.Show(response1.message);
				return;
			}
			// get invoice Draft by call api 
			var api = new InvoiceRestApi();
			var response2 = api.GetInvoiceDetail(response1.data.invoice_id);
			if (response2.code != Const.Code_Successful)
			{
				MessageBox.Show(response2.message);
				return;
			}

			var invoiceSelectDetail = response2.data;

			//
			InvoiceKeepWindow wd = new InvoiceKeepWindow(false, invoiceSelectDetail); wd.ShowDialog();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void AdjustInvoice()
		{
            try
            {
			// call API to change
			// server will create new draft and send back to clinet the id
			InvoiceRestApi apis = new InvoiceRestApi();
			var requets = new InvoiceChangeRequest()
			{
				document_content = Invoice.document_content,
				document_no = Invoice.document_no,
				id = Invoice.id,
				reason_content = Invoice.reason_content,
				state_of_bill = 3
			};
			var response1 = apis.Adjust(requets);
			if (response1.code != Const.Code_Successful)
			{
				MessageBox.Show(response1.message);
				return;
			}
			// get invoice Draft by call api 
			var api = new InvoiceRestApi();
			var response2 = api.GetInvoiceDetail(response1.data.invoice_id);
			if (response2.code != Const.Code_Successful)
			{
				MessageBox.Show(response2.message);
				return;
			}

			var invoiceSelectDetail = response2.data;

			//
			InvoiceKeepWindow wd = new InvoiceKeepWindow(false, invoiceSelectDetail); wd.ShowDialog();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void DeleteInvoice()
		{
            try
            {
			// call API to change
			// server will create new draft and send back to clinet the id
			InvoiceRestApi apis = new InvoiceRestApi();
			var requets = new InvoiceChangeRequest()
			{
				document_content = Invoice.document_content,
				document_no = Invoice.document_no,
				id = Invoice.id,
				reason_content = Invoice.reason_content,
			};
			var response1 = apis.DeleteLogic(requets);
			if (response1.code != Const.Code_Successful)
			{
				MessageBox.Show(response1.message);
				return;
			}
			else MessageBox.Show(response1.message);
		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void Resign()
		{
            try
            {
			var api = new InvoiceRestApi();
            var Profile = (ProfileData)ApplicationCache.GetItem("profile");

            var response = api.ChangeInvoiceSignToKeep(Invoice.id, Profile.keySuperAdin);
			MessageBox.Show(response.message);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}
		#endregion

		#region internal Function
		private void GetScanFile()
		{
			try
			{
				//get Image
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Filter = "Pdf Files|*.pdf";
				if (dlg.ShowDialog() == false)
				{
					canExcute = false;
				}
				FilesCanPath = dlg.FileName;
				// convert pdf to base64
				byte[] Array = System.IO.File.ReadAllBytes(FilesCanPath);
				Invoice.document_content = Convert.ToBase64String(Array);

				canExcute = true;
			}
			catch
			{
				canExcute = false;
			}


		}
        private string GetNameInvoice(string FormId, string SerialId, string InvoiceId, string DateSigned, string Status)
        {
            string InvoiceName = "";
            string _status = "signed";
            string serialName = SerialId.Replace("/", "-");
            string formName = FormId.Replace("/", "-");

            try
            {
            if (Status == Const.STATUS_INVOICE_DRAFT)
            {
                _status = "Draft";
            }
            else
            {
                _status = "Invoice";
            }
            InvoiceName = InvoiceId + "_" + formName + "_" + serialName + "_No_" + "_" + DateSigned + "_" + _status;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
            return InvoiceName;
        }

        private void UpdateResonContent(string selectedMethod)
        {
            try
            {
            string prefix = "["+ Invoice.invoice_form_name + " " + Invoice.invoice_serial_name + " " + Invoice.invoice_number + " Ngày " + SignDay + "]..";

            if(SmajorList.Name == Const.MAJOR_INVOICE_DELETE)
            {
                Invoice.reason_content = Const.MAJOR_INVOICE_DELETE_RESON + prefix;
            }
            else if (SmajorList.Name == Const.MAJOR_INVOICE_REPLACE){

                Invoice.reason_content = Const.MAJOR_INVOICE_REPLACE_RESON + prefix;
            }
            else if (SmajorList.Name == Const.MAJOR_REPORT_EDIT)
            {
                Invoice.reason_content = Const.MAJOR_REPORT_EDIT_RESON + prefix;
            }
            else if (SmajorList.Name == Const.MAJOR_INVOICE_EDIT_INFO)
            {
                Invoice.reason_content = Const.MAJOR_INVOICE_EDIT_INFO_RESON + prefix;
            }
            else if (SmajorList.Name == Const.MAJOR_INVOICE_EDIT)
            {
                Invoice.reason_content = Const.MAJOR_INVOICE_EDIT_RESON + prefix;
            }
            else if (SmajorList.Name == Const.MAJOR_INVOICE_CONVERT)
            {
                Invoice.reason_content = Const.MAJOR_INVOICE_CONVERT_RESON + prefix;
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

		private void GetSortList()
		{
			MajorLists = new ObservableCollection<CBMethod>() {
				 new CBMethod(){Name= Const.MAJOR_INVOICE_DELETE}
				,new CBMethod(){Name= Const.MAJOR_INVOICE_REPLACE}
				,new CBMethod(){Name= Const.MAJOR_REPORT_EDIT }
				,new CBMethod(){Name= Const.MAJOR_INVOICE_EDIT_INFO}
				,new CBMethod(){Name= Const.MAJOR_INVOICE_EDIT }
			    ,new CBMethod(){Name= Const.MAJOR_INVOICE_CONVERT}
			};
		}

		#endregion


	}
}
