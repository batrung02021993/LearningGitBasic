using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VinaInvoice.Model;
using VinaInvoice.Data.Repository;
using VinaInvoice.Data.DataContext;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.IO;
using TuesPechkin;
using System.Drawing.Printing;
using System.IO.Compression;
using static VinaInvoice.Data.DataContext.InvoiceRestApi;
using VinaInvoice.Model.JsonObjectModel;
using VinaInvoice.Common;
using System.Net.Mail;
using System.Threading;
using System.Windows.Threading;
using System.Security;

namespace VinaInvoice.ViewModel
{
    public class InvoiceDraftViewModel : BaseViewModel
    {

		InvoiceRepository _InvoiceRepository = new InvoiceRepository();

        public bool Isloaded = false;
        public ICommand CreateCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand KeepCommand { get; set; }
        public ICommand InvoiceDoubleClickCommand { get; set; }
        public ICommand ChooseAll { get; set; }
		public ICommand SignSelectedInvoicesCommand { get; set; }
        public ICommand SortCommand { get; set; }

        private string CertificateStatus = "Không tìm thấy chứng thư số phù hợp.";
		private string _SerialNumber = "";
		private X509Certificate2 _x509;
		private InvoiceSerialization _InvoiceSerialization = new InvoiceSerialization();
		List<CustomerCare> customerCares = new List<CustomerCare>();


		private ObservableCollection<Invoice> _InvoiceList;
        public ObservableCollection<Invoice> InvoiceList { get => _InvoiceList; set { _InvoiceList = value; OnPropertyChanged(); } }
		public Invoice sInvoice { get; set; }
		private ObservableCollection<SortMethod> _sortList;

        public SortMethod _sSortList = new SortMethod();
        public SortMethod sSortList { get => _sSortList; set { _sSortList = value; OnPropertyChanged(); } }

        private Invoice invoiceSelectDetail;
		public bool KeepingCheckBoxStatus { get; set; } = false;

        public string Companyforsearching { get => InvoiceInvoice_Variable.companyforsearching; set { InvoiceInvoice_Variable.companyforsearching = value; OnPropertyChanged(); } }
        public string TaxCodeforsearching { get => InvoiceInvoice_Variable.taxcodeforsearching; set { InvoiceInvoice_Variable.taxcodeforsearching = value; OnPropertyChanged(); } }
        public string InvoiceNumberforsearching { get => InvoiceInvoice_Variable.invoicenumberforsearching; set { InvoiceInvoice_Variable.invoicenumberforsearching = value; OnPropertyChanged(); } }
        public string InvoiceRefforsearching { get => InvoiceInvoice_Variable.invoicerefforsearching; set { InvoiceInvoice_Variable.invoicerefforsearching = value; OnPropertyChanged(); } }
        public string SerialNameforsearching { get => InvoiceInvoice_Variable.serialnameforsearching; set { InvoiceInvoice_Variable.serialnameforsearching = value; OnPropertyChanged(); } }

        public DateTime Start_date { get => InvoiceInvoice_Variable.start_date; set { InvoiceInvoice_Variable.start_date = value; OnPropertyChanged(); } }
        public DateTime Stop_date { get => InvoiceInvoice_Variable.stop_date; set { InvoiceInvoice_Variable.stop_date = value; OnPropertyChanged(); } }

        private int _page = 1;
        public int Page { get => _page; set { _page = value; OnPropertyChanged(); } } 

 		private bool _isCheckAll = false;
        public bool IsCheckAll
        {
            get => _isCheckAll;
            set
            {
                _isCheckAll = value;
                OnPropertyChanged();
            }
        }

        private bool _showButton = true;
        public bool ShowButton
        {
            get => _showButton;
            set
            {
                _showButton = value;
                OnPropertyChanged();
            }
        }
        public int[] status = {0};
        public int[] state_of_bill = { 0,1,3,4 };
        //int sort_method = 0;
        public int type_sort = 5;

        private DateTime _start_date = new DateTime(2018, 1, 1, 0, 0, 0);
        public DateTime start_date { get => _start_date; set { _start_date = value; OnPropertyChanged(); } }

        private DateTime _stop_date = DateTime.Now;
        public DateTime stop_date { get => _stop_date; set { _stop_date = value; OnPropertyChanged(); } }

        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);


        public InvoiceDraftViewModel()
        {
            try
            {
                StatusBarString = Const.STATUS_BAR_STRING;

            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Search(p); });
            SortCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Sort(p); });

            CreateCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                InvoiceCreateWindow wd = new InvoiceCreateWindow();
                wd.SetParent(this);
                wd.ShowDialog();
            });

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {if(p!=null) p.Close(); });

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Isloaded = true;
                getSortList();                
            });
            LoadListInvoice(status,state_of_bill); // load invoice draft

            DeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DeleteListInvoiceDraft();
            });

            KeepCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                if (KeepingCheckBoxStatus == true)
                {
                    ShowButton = false;
                }
                else
                {
                    ShowButton = true;
                }
                KeepInvoice();
            });

            InvoiceDoubleClickCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                InvoiceDoubleClick();
            });

            ChooseAll = new RelayCommand<object>((p) => { return true; }, (p) => {
                ChooseAllInvoice();

            });

			SignSelectedInvoicesCommand = new RelayCommand<Window>((p) => 
			{			
				return true;
			}, (p) => {

				GetSerialNUmber();
				SearchCertificate();

				if (_x509 == null) {
					MessageBox.Show("Không tìm thấy chứng thư số !!!");
					return;
				}

				// if smart Card is using then get PIn in advandce
				bool IsNotUSB_or_correctPin = true;
				var rsa = _x509.PrivateKey as RSACryptoServiceProvider;
				if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
				{
					IsNotUSB_or_correctPin = HandleSmartCard(rsa.CspKeyContainerInfo.ProviderName, rsa.CspKeyContainerInfo.KeyContainerName, rsa.CspKeyContainerInfo.ProviderType);
				}

				// next is sign
				if (IsNotUSB_or_correctPin)
				{
					SignSelectedInvoices();
				}
				

			});
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		#region Sign List Invoices
		private void SignSelectedInvoices()
		{
            try
            {
                customerCares = new List<CustomerCare>();
                List<string> _id = new List<string>();
                var sSerialtemp = getSerialList();
                var sSerial = getSerialList().First();
                foreach (var i in InvoiceList)
                {
                    if (i.IsSelected)
                    {
                        _id.Add(i.id);
                    }
                }

			if (_id == null) return ;
			if (_id.Count() <= 0) return ;


			/// get default serial 
			DefaultFormSerial defaultFormSerial = new DefaultFormSerial();
			defaultFormSerial.ShowDialog();
			var defaultFormSerialVM = defaultFormSerial.DataContext as DefaultFormSerialViewModel;
			if ((!defaultFormSerialVM.isOk) || defaultFormSerialVM.sserial == null ) {
				MessageBox.Show("Chưa chọn ký hiệu hóa đơn mặc định !!!");
				return;
			}

			var serial = defaultFormSerialVM.sserial;
			//Call Api to sign list and get list invoice number
			InvoiceSignListRequest SignListRequest = new InvoiceSignListRequest
			{
				id = _id,
				invoice_form_id = serial.form_id,
				invoice_form_name = serial.form_name,
				invoice_serial_id = serial.id,
				invoice_serial_name = serial.serial_name,
				invoice_sign_date = GetTimeStamp(DateTime.Now)
			};

			InvoiceRestApi api = new InvoiceRestApi();
			var resSignList = api.SignListDraft(SignListRequest);
			if (resSignList.code != Const.Code_Successful)
			{
				MessageBox.Show(Message.MSS_INV_SIGNED_SELECTED_FAIL , Message.MSS_DIALOG_TITLE_ALERT);
				return;
			}
			if (resSignList.data.SuccessList.Length <= 0)
			{
				MessageBox.Show(Message.MSS_INV_SIGNED_SELECTED_NO_SUCCESS, Message.MSS_DIALOG_TITLE_ALERT);
				return;
			}
			// Sign XML success list
			foreach (var s in resSignList.data.SuccessList)
			{			
					// get Invoice Detail
					var apigetinvoice = new InvoiceRestApi();
					var res = apigetinvoice.GetInvoiceDetail(s.invoice_id);
					if (res.code != Const.Code_Successful) MessageBox.Show(res.message);
					var InvoiceCreate = res.data;

					// Creat XML and HTML and Sign
					string Ipath = AppDomain.CurrentDomain.BaseDirectory + "/Invoice";
					var FolderPath = Ipath + "//" + InvoiceCreate.b_tax_code + "_" + s.search_invoice_id + "_" + DateTime.Now.ToString("yyyy_dd_MM_HHmmss");
					Directory.CreateDirectory(FolderPath);
					var Spath = FolderPath + "/" + "Invoice";
					CreateInvoice(InvoiceCreate,s.invoice_number.ToString("D7"), s.search_invoice_id,Spath);
					SignInvoiceXML(Spath);
					customerCares.Add(new CustomerCare()
					{
					BasePath = Spath,
					InvoiceCreated = InvoiceCreate,
					zipath = FolderPath + ".zip",
					folderPath = FolderPath,
					_xmlUpdate = new Xml_Update()
					{
						invoice_id = s.invoice_id,
						xml_content = System.IO.File.ReadAllText(Spath + ".xml")
					}
					});
				
			}
		
			// update list XML
			List<Xml_Update> _updatelist = new List<Xml_Update>();
			foreach (var c in customerCares) _updatelist.Add(c._xmlUpdate);
			InvoiceRestApi apis = new InvoiceRestApi();
			var Saverequets = new InvoiceUpdateXMLRequest()
			{
				xml_update_list = _updatelist			
			};
			var Saveresponse = apis.SaveXML(Saverequets);

			if(Saveresponse.code != Const.Code_Successful)
			{
				MessageBox.Show("Ký thất bại, vui lòng thử lại !!!", "Vina invoice thông báo:");
				return;
			}
			//Create pdf, zip , send mail
			if (defaultFormSerialVM.Sendmail)
			{
				int Count = 0;
				foreach (var c in customerCares)
				{
					CreateInvoicePdf(c.BasePath, c.folderPath);
					ThreadPool.QueueUserWorkItem(
				  o =>
				  {
					  SendMailHtml(c);
					  Interlocked.Add(ref Count, 1);
				  });
				}
				while (Count < customerCares.Count)
				{
					Thread.Sleep(1000);
				}

				MessageBox.Show("Gửi mail thành công !!!", "Vina invoice thông báo:");
			}
			
			// Reload Draft List 
			MessageBox.Show("Ký thành công !!!", "Vina invoice thông báo:");
			LoadListInvoice(status, state_of_bill);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		public List<InvoiceSerial>  getSerialList()
		{
			var _Repository = new InvoiceSerialRepository();
            try
            {

			_Repository.Page = 1;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
			return  _Repository.GetList().ToList();

		}

		public void SendMailHtml(CustomerCare _CustomerInfor)
		{
            try
            {
                var InvoiceCreate = _CustomerInfor.InvoiceCreated;

			var enterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");

			// using for assign to send mail
			var Enterprise_Config_Detail = (Enterprise_ConfigData)ApplicationCache.GetItem("enterprise_config_Detail");
			if (Enterprise_Config_Detail == null)
			{
				MessageBox.Show(Message.MSS_SEND_EMAIL_NO_CONFIG);
				return;
			}
			else
			{

				string senderID = Enterprise_Config_Detail.mail_user_name;
				string senderPassword = Enterprise_Config_Detail.mail_password;
				string receiverID = InvoiceCreate.b_email;

				string subject = enterpriseData.company_name + " gửi Hóa đơn điện tử đến quý khách hàng";
				string body = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Template/send_signed.html", Encoding.UTF8);

				body = body.Replace("@EINVOICE_URL@", "https://hoadon.onlinevina.com.vn/invoice")
				.Replace("@EINVOICE@", "GTGT số :(" + InvoiceCreate.invoice_form_name + " " + InvoiceCreate.invoice_serial_name + "), " + InvoiceCreate.invoice_number.ToString("D7") + ".")
				.Replace("@BUYER_COMPANY@", InvoiceCreate.b_company)
				.Replace("@SELLER_COMPANY@", enterpriseData.company_name)
				.Replace("@EINVOICE_KEY@", InvoiceCreate.search_invoice_id)
				.Replace("@APP_NAME@", "Phần mềm hóa đơn điện tử Vina E-Invoice");

				string attachmentPath = _CustomerInfor.zipath;

				try
				{
					MailMessage mail = new MailMessage();
					mail.To.Add(receiverID);
					mail.From = new MailAddress(senderID);
					mail.Subject = subject;
					mail.Body = body;
					mail.IsBodyHtml = true;
					mail.Attachments.Add(new Attachment(attachmentPath));

					SmtpClient smtp = new SmtpClient();
					smtp.Host = Enterprise_Config_Detail.mail_smtp_server; //Or Your SMTP Server Address
					smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
					smtp.Port = Enterprise_Config_Detail.mail_port;
					smtp.EnableSsl = true;
					smtp.Send(mail);
					Console.Write("Email Sent Successfully");
				}
				catch (Exception ex)
				{
					MessageBox.Show(Message.MSS_SEND_EMAIL_FAIL);
					Console.Write("Exception in sendEmail:" + ex.Message);
				}
			}
		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void CreateInvoice(Invoice InvoiceCreate,string InvoiceNo,string SearchCode,string Spath)
		{
            try
            {
                var enterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");
                string Xsltpath = AppDomain.CurrentDomain.BaseDirectory + "Xslt" + "\\Current.xslt";


		///Get XSLT templete and save 
		// save Request
		FormDetailRequest formdetailrequet = new FormDetailRequest
			{
				enterprise_id = enterpriseData.id,
				id = InvoiceCreate.invoice_form_id
			};
			InvoiceRestApi api = new InvoiceRestApi();
			var response = api.GetForm(formdetailrequet);
			System.IO.File.WriteAllText(Xsltpath, response.data.xslt_content);


			/// map list items to Xml_Itemlist
			invoiceInvoiceDataItems _items = new invoiceInvoiceDataItems();

			int count = 0;
			foreach (var p in InvoiceCreate.invoice_item_list)
			{
				count++;
				var item = new invoiceInvoiceDataItemsItem()
				{
					currency = InvoiceCreate.currency_code,
					itemCode = p.item_code,
					itemDiscount = p.discount_amount.ToString(),
					itemName = p.item_name,
					itemTotalAmountWithoutVat = p.item_total_amount_without_vat.ToString(),
					quantity = p.quantity.ToString(),
					totalAmount = p.total_amount.ToString(),
					unitName = p.unit_name,
					unitPrice = p.unit_price.ToString(),
					vatAmount = p.vat_amount.ToString(),
					vatPercentage = p.vat_percentage.ToString(),
					lineNumber = count.ToString(),
				};
				_items.items.Add(item);
			}

			/// Create XML Object
			invoice iv = _InvoiceSerialization.Invoice;

			invoiceInvoiceData ivdata = iv.invoiceData;

			// metadata
			ivdata.id = "Data";
			/// Buyer infor
			ivdata.buyerAddressLine = InvoiceCreate.b_address;
			ivdata.buyerBankAccount = InvoiceCreate.b_bank_number;
			ivdata.buyerEmail = InvoiceCreate.b_email;
			ivdata.buyerBankName = InvoiceCreate.b_bank_name;
			ivdata.buyerDisplayName = InvoiceCreate.b_name;
			ivdata.buyerFaxNumber = InvoiceCreate.b_fax_number;
			ivdata.buyerLegalName = InvoiceCreate.b_company;
			ivdata.buyerPhoneNumber = InvoiceCreate.b_phone_number;
			ivdata.buyerTaxCode = InvoiceCreate.b_tax_code;

			/// Invoice Infor
			ivdata.invoiceAppRecordId = "";// not implement
			ivdata.invoiceIssuedDate = DateTime.Now.ToString(); // Ngay ki hoa don*/ /////////need review
			ivdata.invoiceName = InvoiceCreate.invoice_file_name;
			ivdata.invoiceNote = InvoiceCreate.invoice_note;
			ivdata.invoiceNumber = InvoiceNo;
			ivdata.invoiceSeries = InvoiceCreate.invoice_serial_name;
			ivdata.templateCode = InvoiceCreate.invoice_form_name;
			ivdata.additionalReferenceDesc = "";// not implement
			ivdata.adjustmentType = "";// not implement
			ivdata.contractNumber = "";// not implement
			ivdata.currencyCode = InvoiceCreate.currency_code;
			ivdata.delivery = "";// not implement
			switch (InvoiceCreate.method_of_payment)
			{
				case 0:
					ivdata.paymentMethodName = "Tiền mặt";
					break;
				case 1:
					ivdata.paymentMethodName = "Chuyển khoản";
					break;
				default:
					ivdata.paymentMethodName = "Tiền mặt/Chuyển khoản";
					break;
			}
			
			ivdata.invoiceSigned = "";// not implement
			ivdata.invoiceType = "";// not implement
			ivdata.originalInvoice = "";// not implement
			ivdata.invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
			{
				InvoiceTaxBreakdowns = new List<invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown>()
						{
							new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
							{
								vatPercentage = "",// example
								vatTaxableAmount = "",
								vatTaxAmount = ""
							}
						}

			};// not implement
			ivdata.payments = new invoiceInvoiceDataPayments()
			{
				payments = new List<invoiceInvoiceDataPaymentsPayment>()
						{
							new invoiceInvoiceDataPaymentsPayment()
							{

								paymentMethodNameExt = ivdata.paymentMethodName,
							}
						}
			};
			ivdata.printFlag = "False";// not implement
			ivdata.printSample = "";// not implement
			ivdata.qrCodeData = SearchCode;
			ivdata.referentNo = "";// not implement

			///Product infor
			ivdata.items = _items;
			ivdata.discountAmount = InvoiceCreate.total_discount.ToString();
			ivdata.ExchangeRate = InvoiceCreate.exchange_rate.ToString();
			ivdata.totalAmountWithoutVAT = InvoiceCreate.sub_total.ToString();
			ivdata.totalVATAmount = InvoiceCreate.vat_amount.ToString();
			ivdata.totalAmountWithVAT = InvoiceCreate.total_amount.ToString();
			ivdata.serviceChargePercent = InvoiceCreate.service_charge_percent.ToString();
			ivdata.signedDate = InvoiceCreate.invoice_sign_date;// not implement,
			ivdata.submittedDate = "";// not implement
			ivdata.systemCode = "";// not implement
			ivdata.totalAmountWithVATInWords = InvoiceCreate.in_word;
			ivdata.totalServiceCharge = InvoiceCreate.total_service_charge.ToString();
			ivdata.userDefines = "";// not implement
			ivdata.vatPercentageBill = "10";// not implement

			/// Seller Infor
			ivdata.sellerEmail = enterpriseData.email;
			ivdata.sellerAddressLine = enterpriseData.address;
			ivdata.sellerAppRecordId = "";// not implement
			ivdata.sellerBankAccount = enterpriseData.bank_number;
			ivdata.sellerBankName = enterpriseData.bank_name;
			ivdata.sellerContactPersonName = enterpriseData.manage_by;
			ivdata.sellerFaxNumber = enterpriseData.fax;
			ivdata.sellerLegalName = enterpriseData.company_name;
			ivdata.sellerPhoneNumber = enterpriseData.phone_number;
			ivdata.sellerSignedPersonName = "";// not implement
			ivdata.sellerSubmittedPersonName = "";// not implement
			ivdata.sellerTaxCode = enterpriseData.tax_code;
			ivdata.sellerWebsite = enterpriseData.website;

			/// Create files
			_InvoiceSerialization.ToInvoiceXml(Spath + ".xml");

			_InvoiceSerialization.ToInvoiceHtml(Spath + ".xml", Spath + ".html");

            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void SignInvoiceXML(string Spath)
		{
            try
            {
                //Signature invoice 			
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(Spath + ".xml");

			xmlDoc.InsertAfter(xmlDoc.CreateProcessingInstruction(
				  "xml-stylesheet",
				  "type='text/xsl' href='@invoiceTemplate'"), xmlDoc.FirstChild);


			SignXml(xmlDoc, _x509);

			//save xml
			xmlDoc.Save(Spath + ".xml");

			//create html file
			_InvoiceSerialization.ToInvoiceHtml(Spath + ".xml", Spath + ".html");
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void CreateInvoicePdf(string Spath, string folderpath)
		{
            try
            {
                var document = new HtmlToPdfDocument
                {
                    GlobalSettings =
                {
                    ProduceOutline = true,
                    DocumentTitle = "Pretty Websites",
                    PaperSize = PaperKind.A4,
                    Margins =
                    {
                        Unit = Unit.Centimeters
                    }
                },
                    Objects = {
                    //Path to e-invoice pattern
                    new ObjectSettings { PageUrl = Spath + ".html" },
				}
			};
			var converter = new StandardConverter(
				new PdfToolset(
					new WinAnyCPUEmbeddedDeployment(
						new TempFolderDeployment())));

			var result = converter.Convert(document);
			//Path to e-invoice pdf file
			using (var fs = new FileStream(Spath + ".pdf", FileMode.Create))
			{
				fs.Write(result, 0, result.Length);
				fs.Flush();
			}


			// Create Zip file		
			// Anh su dung bien Zipathfile de gui mail fil zip
			var zipPathfile = folderpath + ".zip";
			ZipFile.CreateFromDirectory(folderpath, zipPathfile);			
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		public string SignXml(XmlDocument Document, X509Certificate2 cert)
		{
            try
            {
			SignedXml signedXml = new SignedXml(Document);
			signedXml.SigningKey = cert.PrivateKey;
			signedXml.Signature.Id = "seller";

			// Create a reference to be signed.
			Reference reference = new Reference();
			reference.Uri = "";

			// Add an enveloped transformation to the reference.            
			XmlDsigEnvelopedSignatureTransform env =
			   new XmlDsigEnvelopedSignatureTransform(true);
			reference.AddTransform(env);

			//canonicalize
			XmlDsigC14NTransform c14t = new XmlDsigC14NTransform();
			reference.AddTransform(c14t);

			KeyInfo keyInfo = new KeyInfo();
			KeyInfoX509Data keyInfoData = new KeyInfoX509Data(cert);
			KeyInfoName kin = new KeyInfoName();
			kin.Value = "Public key of certificate";
			RSACryptoServiceProvider rsaprovider = (RSACryptoServiceProvider)cert.PublicKey.Key;
			RSAKeyValue rkv = new RSAKeyValue(rsaprovider);
			keyInfo.AddClause(kin);
			keyInfo.AddClause(rkv);
			keyInfo.AddClause(keyInfoData);
			signedXml.KeyInfo = keyInfo;

			// Add the reference to the SignedXml object.
			signedXml.AddReference(reference);

			// Compute the signature.
			signedXml.ComputeSignature();

			// Get the XML representation of the signature and save 
			// it to an XmlElement object.
			XmlElement xmlDigitalSignature = signedXml.GetXml();

			Document.DocumentElement.AppendChild(
				Document.ImportNode(xmlDigitalSignature, true));
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
			return Document.OuterXml;
		}

		private void GetSerialNUmber()
		{
            try
            {
			var Enterprise_Config_Detail = (Enterprise_ConfigData)ApplicationCache.GetItem("enterprise_config_Detail");		
			_SerialNumber = Enterprise_Config_Detail.token_serial;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void SearchCertificate()
		{
            try
		{
			string CerStatus = "red";

			string[] Serials = _SerialNumber.Split(new string[] { "||||" }, StringSplitOptions.None);
			X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
			X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
			X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
			foreach (X509Certificate2 x509 in fcollection)
			{
				foreach (var serial in Serials)
				{
					if (String.Compare(x509.GetSerialNumberString(), serial, true) == 0)
					{
						_x509 = x509;
						CertificateStatus = x509.FriendlyName + "\nIssuer by: " + x509.Issuer + ", Valid from :" + x509.NotBefore + " to :" + x509.NotAfter;
						CerStatus = "green";
						break;
					}
				}

				if (String.Compare(CerStatus, "green", true) == 0) break;
				x509.Reset();
				CerStatus = "red";

			}
			store.Close();
		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		// not use yet => may be use in future
		private bool SearchCertificate_StoredPin()
		{
			bool iscorrectPin = true;
            try
            {
			string[] Serials = _SerialNumber.Split(new string[] { "||||" }, StringSplitOptions.None);
			RSACryptoServiceProvider rsa;
			X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
			X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
			X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
			foreach (X509Certificate2 x509 in fcollection)
			{

				foreach (var serial in Serials)
				{
					if (String.Compare(x509.GetSerialNumberString(), serial, true) == 0)
					{

						CertificateStatus = x509.FriendlyName + "\nIssuer by: " + x509.Issuer + ", Valid from :" + x509.NotBefore + " to :" + x509.NotAfter;
						rsa = x509.PrivateKey as RSACryptoServiceProvider;
						if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
						{
							// inspect rsa.CspKeyContainerInfo.KeyContainerName Property
							// or rsa.CspKeyContainerInfo.ProviderName (your smartcard provider, such as 
							// "Schlumberger Cryptographic Service Provider" for Schlumberger Cryptoflex 4K
							// card, etc		
							iscorrectPin = HandleSmartCard(rsa.CspKeyContainerInfo.ProviderName, rsa.CspKeyContainerInfo.KeyContainerName, rsa.CspKeyContainerInfo.ProviderType);
						}
						else
						{
							// done care
						}
						break;
					}
				}
				x509.Reset();
			}
			store.Close();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
			return iscorrectPin;
		}

		private bool HandleSmartCard(string ProviderName, string KeyContainerName, int ProviderType)
		{
			PinDialog pinDialog = new PinDialog();
			var pindialogVM = pinDialog.DataContext as PinDialogViewModel;
			if (!pindialogVM.iscorected) pinDialog.ShowDialog();
			if (!pindialogVM.OK) return false;
			string PinCode = pindialogVM.Pin; // "1234567"; // NEED TO IMPLEMENT DIALOG FOR USER ENTER TOKEN'S PIN

			try
			{
				//if pin code is set then no windows form will popup to ask it
				SecureString pwd = GetSecurePin(PinCode);
				CspParameters csp = new CspParameters(ProviderType,
														ProviderName,
														KeyContainerName,
														new System.Security.AccessControl.CryptoKeySecurity(),
														pwd);
				RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(csp);
				// the pin code will be cached for next access to the smart card
			}
			catch (Exception ex)
			{
				MessageBox.Show("Mã Pin Không đúng !!!");
				return false;
			}

			X509Store store = new X509Store(StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadOnly);
			if ((ProviderName == "") || (KeyContainerName == ""))
			{
				MessageBox.Show("You must set Provider Name and Key Container Name");
				return false;
			}
			foreach (X509Certificate2 cert2 in store.Certificates)
			{
				if (cert2.HasPrivateKey)
				{
					RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
					if (rsa == null) continue; // not smart card cert again
					if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
					{
						if ((rsa.CspKeyContainerInfo.KeyContainerName == KeyContainerName) && (rsa.CspKeyContainerInfo.ProviderName == ProviderName))
						{
							//we find it
							_x509 = cert2;
							break;
						}
					}
				}
			}

			if (_x509 == null)
			{
				MessageBox.Show("Mã Pin Không đúng !!!");
				return false;
			}
			else
			{
				pindialogVM.iscorected = true;
				return true;

			}


		}

		private SecureString GetSecurePin(string PinCode)
		{
			SecureString pwd = new SecureString();
            try
            {
			foreach (var c in PinCode.ToCharArray()) pwd.AppendChar(c);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
			return pwd;
		}


		#endregion

		private void ChooseAllInvoice()
        {
            try
        {
            if (IsCheckAll)
            {
                if (InvoiceList != null)
                {
                    foreach (Invoice p in InvoiceList)
                    {
                        p.IsSelected = true;
                    }
                }
            }
            else
            {
                if (InvoiceList != null)
                {
                    foreach (Invoice p in InvoiceList)
                    {
                        p.IsSelected = false;
                    }
                }
            }
            RefreshListView();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
        private void RefreshListView()
        {
            try
        {
            if (InvoiceList != null)
            {
                List<Invoice> tempInvoice = new List<Invoice>();
                foreach (Invoice p in InvoiceList)
                {
                    tempInvoice.Add(p);
                }

                InvoiceList = new ObservableCollection<Invoice>();
                foreach (Invoice invoice in tempInvoice)
                {
                    InvoiceList.Add(invoice);
                }
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void KeepInvoice()
        {
            try
        {
			if (KeepingCheckBoxStatus)
			{
				_page = 1;
				status[0] = 2;
				LoadListInvoice(status, state_of_bill);
			}
			else
			{
				_page = 1;
				status[0] = 0;
				LoadListInvoice(status, state_of_bill);
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
			}
        }

        private void InvoiceDoubleClick()
        {
            try
            {
                var api = new InvoiceRestApi();
				var response = api.GetInvoiceDetail(sInvoice.id);

				if (response.code != Const.Code_Successful) MessageBox.Show(response.message);

				invoiceSelectDetail = response.data;

				InvoiceKeepWindow wd = new InvoiceKeepWindow(KeepingCheckBoxStatus, invoiceSelectDetail);
                wd.setParent(this);
                wd.ShowDialog();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }

        }

        public void LoadListInvoice(int[] status,int[] state_of_bill)
        {
            try
        {
            IEnumerable<Invoice> list;
            _InvoiceRepository.Page = Page;
            list = _InvoiceRepository.GetList(status, state_of_bill);

			InvoiceList = new ObservableCollection<Invoice>();
			if (list.Count() > 0)
            {           
                int count = 0;
                foreach (Invoice p in list)
                {
					count++;
					p.Stt = count;
                    InvoiceList.Add(p);
                }               
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void getSortList()
        {
            SortLists = new ObservableCollection<SortMethod>() {
                 new SortMethod(){Name="Ký hiệu hóa đơn"}
                ,new SortMethod(){Name="Số hóa đơn"}
                ,new SortMethod(){Name="Mã số thuế"}
                ,new SortMethod(){Name="Công ty"}
                ,new SortMethod(){Name="Số chứng từ"}
                ,new SortMethod(){Name="Thời gian"}
            };
        }


        private void DeleteListInvoiceDraft()
        {
            try
            {
            List<Invoice> id_list = new List<Invoice>();

            foreach (Invoice invoice in InvoiceList)
            {
                if (invoice.IsSelected)
                    id_list.Add(invoice);
            }
            if (id_list != null)
            {
                _InvoiceRepository.Delete(id_list);
                LoadListInvoice(status,state_of_bill);
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public ObservableCollection<SortMethod> SortLists
        {
            get { return _sortList; }
            set { _sortList = value; OnPropertyChanged("SortLists"); }
        }

        private void Sort(object item)
        {
            try
        {
            if (item != null)
            {
                //if (item.ToString().Equals("Increase"))
                //    InvoiceInvoice_Variable.sort_method = 0;
                //else if (item.ToString().Equals("Decrease"))
                //    InvoiceInvoice_Variable.sort_method = 1;

                  if (item.ToString().Equals("Page_Increase"))
                  {
                      Page++;
                      _InvoiceRepository.Page = Page;
                  }
                  else if (item.ToString().Equals("Page_Decrease"))
                  {
                      if (Page > 1)
                          Page--;
                      _InvoiceRepository.Page = Page;
                  }
            }

            if (sSortList.Name != null)
            {
                if (sSortList.Name.Equals("Công ty"))
                    InvoiceInvoice_Variable.type_sort = 0;
                else if (sSortList.Name.Equals("Mã số thuế"))
                    InvoiceInvoice_Variable.type_sort = 1;
                else if (sSortList.Name.Equals("Số hóa đơn"))
                    InvoiceInvoice_Variable.type_sort = 2;
                else if (sSortList.Name.Equals("Số chứng từ"))
                    InvoiceInvoice_Variable.type_sort = 3;
                else if (sSortList.Name.Equals("Ký hiệu hóa đơn"))
                    InvoiceInvoice_Variable.type_sort = 4;
                else
                    InvoiceInvoice_Variable.type_sort = 5;
            }

            IEnumerable<Invoice> list;
            _InvoiceRepository.Page = Page;
            InvoiceList = new ObservableCollection<Invoice>();

            if (Companyforsearching != "")
            {
                list = _InvoiceRepository.Search(0, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, Companyforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (TaxCodeforsearching != "")
            {
                list = _InvoiceRepository.Search(1, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, TaxCodeforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (InvoiceNumberforsearching != "")
            {
                list = _InvoiceRepository.Search(2, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceNumberforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (InvoiceRefforsearching != "")
            {
                list = _InvoiceRepository.Search(3, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceRefforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (SerialNameforsearching != "")
            {
                list = _InvoiceRepository.Search(4, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, SerialNameforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else
            {
                list = _InvoiceRepository.Search(5, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, "", status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }

            if (list.Count() > 0)
            {
                int count = 0;
                foreach (Invoice p in list)
                {
                    count++;
                    p.Stt = count;
                    InvoiceList.Add(p);
                }
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void Search(object item)
        {
            try
        {
            if (item != null)
            {
                if (item.ToString().Equals("Increase"))
                    InvoiceInvoice_Variable.sort_method = 0;
                else if (item.ToString().Equals("Decrease"))
                    InvoiceInvoice_Variable.sort_method = 1;
            }

            if (sSortList.Name != null)
            {
                if (sSortList.Name.Equals("Công ty"))
                    InvoiceInvoice_Variable.type_sort = 0;
                else if (sSortList.Name.Equals("Mã số thuế"))
                    InvoiceInvoice_Variable.type_sort = 1;
                else if (sSortList.Name.Equals("Số hóa đơn"))
                    type_sort = 2;
                else if (sSortList.Name.Equals("Số chứng từ"))
                    InvoiceInvoice_Variable.type_sort = 3;
                else if (sSortList.Name.Equals("Ký hiệu hóa đơn"))
                    InvoiceInvoice_Variable.type_sort = 4;
                else
                    InvoiceInvoice_Variable.type_sort = 5;
            }

            IEnumerable<Invoice> list;
            _InvoiceRepository.Page = Page;
            InvoiceList = new ObservableCollection<Invoice>();

            if (Companyforsearching != "")
            {
                list = _InvoiceRepository.Search(0, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, Companyforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (TaxCodeforsearching != "")
            {
                list = _InvoiceRepository.Search(1, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, TaxCodeforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (InvoiceNumberforsearching != "")
            {
                list = _InvoiceRepository.Search(2, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceNumberforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (InvoiceRefforsearching != "")
            {
                list = _InvoiceRepository.Search(3, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceRefforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (SerialNameforsearching != "")
            {
                list = _InvoiceRepository.Search(4, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, SerialNameforsearching, status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else
            {
                list = _InvoiceRepository.GetList(status, state_of_bill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }

            if (list.Count() > 0)
            {
                int count = 0;
                foreach (Invoice p in list)
                {
                    count++;
                    p.Stt = count;
                    InvoiceList.Add(p);
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy hóa đơn bạn muốn", Message.MSS_DIALOG_TITLE_ALERT);
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

		#region DateTime Convert
		/// <summary>
		/// The function convert timestamp to datetime
		/// </summary>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		private DateTime GetdatetimeFromStamp(double timestamp)
		{
			System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            try
            {
                dateTime = dateTime.AddSeconds(timestamp);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
			return dateTime;
		}

		/// <summary>
		/// The function return list date time beetween start day and end day
		/// </summary>
		/// <param name="startingDate"></param>
		/// <param name="endingDate"></param>
		/// <returns></returns>
		private ObservableCollection<SignDay> GetListSignDay(DateTime startingDate, DateTime endingDate)
		{
			List<DateTime> allDates = new List<DateTime>();
			ObservableCollection<SignDay> days = new ObservableCollection<SignDay>();
            try
            {
			//int starting = startingDate.Day;
			//int ending = endingDate.Day;

			for (DateTime date = endingDate; date >= startingDate; date = date.AddDays(-1))
				allDates.Add(date);
			foreach (var d in allDates)
			{
				var sday = new SignDay()
				{
					dayview = d.ToString("dd/MM/yyyy"),
					daytimeStamp = GetTimeStamp(d)

				};
				days.Add(sday);
			}
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
            
			return days;
		}

		/// <summary>
		/// Convert datetime to timstamp in string 
		/// </summary>
		/// <param name="day"></param>
		/// <returns></returns>
		private string GetTimeStamp(DateTime day)
		{
            int re = 0;
            try
            {
                TimeSpan span = day.Subtract(new DateTime(1970, 1, 1, 7, 0, 0, DateTimeKind.Local));
                re = (int)span.TotalSeconds;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
			return re.ToString();
		}


		#endregion

	}

	public class SortMethod
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

	public class CustomerCare
	{
		public string zipath { get; set; }
		public string folderPath { get; set; }
		public string BasePath { get; set; } // common path of xml and html file
		public Invoice InvoiceCreated { get; set; }			
		public Xml_Update _xmlUpdate { get; set; }
		
	}
}
