
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;
using System.Net.Mail;
using TuesPechkin;

using System.IO;
using System.Drawing.Printing;
using VinaInvoice.Common;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO.Compression;
using System.Globalization;
using System.Security;

namespace VinaInvoice.ViewModel
{
    public class InvoiceEditViewModel : BaseViewModel
    {
        #region Reverse calcualate
        public bool _isReverseCaculator;
        public bool IsReverseCaculator
        {
            get => _isReverseCaculator;
            set
            {
                if (_isReverseCaculator != value)
                {
                    _isReverseCaculator = value;
                    DoRefreshProductList();
                    Console.WriteLine(_isReverseCaculator + "");
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Comand
        public ICommand CloseCommand { get; set; }
        public ICommand ShowCertificateCommand { get; set; }
        public ICommand SignCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand RefreshProductCommand { get; set; }
        public ICommand RemoveProductCommand { get; set; }
        public ICommand KeepCommand { get; set; }
        public ICommand CreateCustomerCommand { get; set; }

        #endregion

        #region Private Object for internal process
        private string Path = AppDomain.CurrentDomain.BaseDirectory;
        private string _CertificateStatus = "Không tìm thấy chứng thư số phù hợp.";
        private string _SerialNumber = "";
        private X509Certificate2 _x509;
        private InvoiceSerialization _InvoiceSerialization = new InvoiceSerialization();
        private string InvoiceNo = "";
        private string Dpath = AppDomain.CurrentDomain.BaseDirectory + "/Draft";
        private string Ipath = AppDomain.CurrentDomain.BaseDirectory + "/Invoice";
        private string Spath = AppDomain.CurrentDomain.BaseDirectory + "/Draft";
        private string Xsltpath = AppDomain.CurrentDomain.BaseDirectory + "Xslt" + "\\Current.xslt";
        private string ZipPath = ""; // for directory 
        private string zipPathfile = ""; //for file  zip using to send email
        public bool ViewMode = false; // determine if any button save or sign are pressed
        private string SearchCode = "";

        public bool IsKeepInvoice;

        #endregion

        #region Binding Object

        public string CertificateStatus
        {
            get => _CertificateStatus;
            set
            {
                _CertificateStatus = value;
                OnPropertyChanged();
            }
        }

        private string _cerStatus = "red";
        public string CerStatus
        {
            get => _cerStatus;
            set
            {
                if (_cerStatus != value)
                {
                    _cerStatus = value;
                    OnPropertyChanged();
                }
            }
        }


        private ObservableCollection<Customer> _List;
        public ObservableCollection<Customer> DataList { get => _List; set { _List = value; OnPropertyChanged(); } }

        public Customer _customerSelected;
        public Customer CustomerSelected
        {
            get => _customerSelected;
            set
            {
                if (_customerSelected != value)
                {
                    _customerSelected = value;
                    if (_customerSelected != null)
                    {
                        InvoiceCreate.b_name = _customerSelected.DisplayName;
                        InvoiceCreate.b_company = _customerSelected.CompanyName;
                        InvoiceCreate.b_tax_code = _customerSelected.CompanyTaxCode;
                        InvoiceCreate.b_email = _customerSelected.Email;
                        InvoiceCreate.b_address = _customerSelected.Address;
                        InvoiceCreate.b_bank_name = _customerSelected.BankName;
                        InvoiceCreate.b_bank_number = _customerSelected.BankNumber;

                    }

                    OnPropertyChanged();
                }

            }
        }

        private ObservableCollection<string> _moneyUnit;
        public ObservableCollection<string> MoneyUnits { get { return _moneyUnit; } set { _moneyUnit = value; } }


        private Product _selectedProduct;

        private ObservableCollection<Customer> _DataCustomerList;
        CustomerRepository _Repository = new CustomerRepository();
        public ObservableCollection<Customer> DataCustomerList { get => _DataCustomerList; set { _DataCustomerList = value; OnPropertyChanged(); } }

        CustomerCreateRepository _customerCreateRepository = new CustomerCreateRepository();
        private ObservableCollection<Product> _DataProductList;

        ProductRepository _ProductRepository = new ProductRepository();
        public ObservableCollection<Product> DataProductList { get => _DataProductList; set { _DataProductList = value; OnPropertyChanged(); } }
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }

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
                IsMoreTax = sform.Is_more_tax;
                foreach (var s in SerialList)
                {
                    if (s.form_id == sform.id) SerialListUI.Add(s);
                }

                sserial = SerialListUI.FirstOrDefault();

                OnPropertyChanged();

            }
        }
        public Form sformDetail { get; set; }

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
                GetDateSigned();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SignDay> _dateSignedLists;
        public ObservableCollection<SignDay> DateSignedLists { get { return _dateSignedLists; } set { _dateSignedLists = value; OnPropertyChanged(); } }
        private SignDay _signDay;
        public SignDay sDateSigned { get { return _signDay; } set { _signDay = value; OnPropertyChanged(); } }

        private IEnumerable<string> _companyNames;
        public IEnumerable<string> CompanyNames { get => _companyNames; set { _companyNames = value; OnPropertyChanged(); } }
        public string SelectedCompanyName;

        private InvoiceSum _sum;
        public InvoiceSum invoiceSum { get => _sum; set { _sum = value; OnPropertyChanged(); } }
        private InvoiceSumExtention _sumExtend;
        public InvoiceSumExtention invoiceSumExtend { get => _sumExtend; set { _sumExtend = value; OnPropertyChanged(); } }

        private ObservableCollection<PaymentMethod> _paymentList;
        public ObservableCollection<PaymentMethod> PaymentLists { get { return _paymentList; } set { _paymentList = value; } }
        private PaymentMethod _spaymentMethod;
        public PaymentMethod spaymentMethod { get { return _spaymentMethod; } set { _spaymentMethod = value; OnPropertyChanged(); } }

        private Invoice _invoiceCreate = new Invoice();
        public Invoice InvoiceCreate
        {
            get => _invoiceCreate;
            set
            {
                _invoiceCreate = value;
                if (_invoiceCreate.invoice_item_list != null) MapingtoUI();
                OnPropertyChanged();
            }
        }

        private double _totalCharge;
        public double TotalCharge
        {

            get => _totalCharge;

            set
            {
                _totalCharge = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ProductUI> _ProductList;
        public ObservableCollection<ProductUI> ProductList
        {
            get => _ProductList;

            set
            {
                _ProductList = value;
                if (_ProductList != null)
                {
                    double sum = 0;
                    foreach (ProductUI a in ProductList)
                    {
                        sum = sum + a.SubTotal;
                    }
                    TotalCharge = sum;
                }
                OnPropertyChanged();
            }
        }

        private ProductUI _productItemSeleced { get; set; }
        public ProductUI ProductItemSeleced
        {
            get => _productItemSeleced;
            set
            {
                _productItemSeleced = value;
                OnPropertyChanged();
            }
        }

        private Product _productItemSelecedName { get; set; }
        public Product ProductItemSelecedName
        {
            get => _productItemSelecedName;
            set
            {
                if (value != null)
                {
                    _productItemSelecedName = value;

                    if (ProductItemSeleced != null)
                    {

                        ProductItemSeleced.UnitName = _productItemSelecedName.unit_name;
                        ProductItemSeleced.Id = _productItemSelecedName.id;
                        ProductItemSeleced.ItemCode = _productItemSelecedName.item_code;


                        OnPropertyChanged();

                    }
                }

            }
        }

        private ObservableCollection<string> _TaxCodeList;
        public ObservableCollection<string> TaxCodeList { get => _TaxCodeList; set { _TaxCodeList = value; } }
        public Taxcode staxcode { get; set; }


        private bool _isMoreTax = true;
        public bool IsMoreTax
        {
            get => _isMoreTax;
            set
            {
                if (_isMoreTax != value)
                {
                    _isMoreTax = value;
                    OnPropertyChanged();

                    setUITaxMore();
                }
            }
        }

        private void setUITaxMore()
        {
            if (ProductList != null)
            {
                DoRefreshProductList();

            }
        }

        private void DoRefreshProductList()
        {
            ObservableCollection<ProductUI> ProductListTemp = new ObservableCollection<ProductUI>();
            foreach (ProductUI item in ProductList)
            {
                if (!IsMoreTax)
                {
                    item.IndexTax = IndexTotalComboBox;
                }

                if (IsReverseCaculator)
                {
                    // Tính ngược
                    item.TotalAmountCheck = "true";
                }
                else
                {
                    item.TotalAmountCheck = Const.FLG_ActiveTotalAmount;
                }
                item.IsReverseCal = IsReverseCaculator;
                item.IsTaxEnable = IsMoreTax;
                ProductListTemp.Add(item);
            }


            ProductList = new ObservableCollection<ProductUI>();
            foreach (ProductUI item in ProductListTemp)
            {
                ProductList.Add(item);
            }
        }

        private int _indexTotalComboBox { get; set; }
        public int IndexTotalComboBox
        {
            get => _indexTotalComboBox;
            set
            {
                if (_indexTotalComboBox != value)
                {
                    _indexTotalComboBox = value;
                    if (!IsMoreTax)
                    {
                        SetItemTax(_indexTotalComboBox);

                    }
                    OnPropertyChanged();
                }
            }
        }

        private int _indexComboBox { get; set; }
        public int IndexComboBox
        {
            get => _indexComboBox;
            set
            {
                if (_indexComboBox != value)
                {
                    _indexComboBox = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _totalTaxVisibility = UiVisibility.VISIBLE;
        public string TotalTaxVisibility
        {
            get => _totalTaxVisibility;
            set
            {
                if (_totalTaxVisibility != value)
                {
                    _totalTaxVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        ///
        public InvoiceEditViewModel()
        {
            try
            {
            Init();

            //Command

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { if (p != null) { ClearForNewSign(); p.Close(); } });

            ShowCertificateCommand = new RelayCommand<object>((p) =>
            {
                if (_x509 == null) return false;
                return true;
            }, (p) =>
            {

                ShowCertificate();
            });

            SignCommand = new RelayCommand<Window>((p) =>
            {
                if (_x509 == null) return false;
                if (sserial == null || sform == null) return false;
                return true;
            }, (p) =>
            {
                //decouple with UI
                UImaping();

                //Validate for DaySign 
                InvoiceCreate.invoice_sign_date = sDateSigned.daytimeStamp;

                // Custom Validate
                InvoiceCreate.Sign_Validate();

                //Validate General
                if (InvoiceCreate.errors.Count > 0)
                {
                    var e = InvoiceCreate.errors.FirstOrDefault();
                    if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
                    return;
                }

                // Validate Item List
                if (ProductList.Count > 0)
                {
                    foreach (var pro in ProductList)
                    {
                        if (pro.HasErrors)
                        {
                            var e = pro.errors.FirstOrDefault();
                            if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
                            return;
                        }

                    }
                }

                bool IsNotUSB_or_correctPin = true;
                var rsa = _x509.PrivateKey as RSACryptoServiceProvider;
                if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                {
                    IsNotUSB_or_correctPin = HandleSmartCard(rsa.CspKeyContainerInfo.ProviderName, rsa.CspKeyContainerInfo.KeyContainerName, rsa.CspKeyContainerInfo.ProviderType);
                }
                //// next is sign
                if (IsNotUSB_or_correctPin)
                {
                    SignInvoice();

                    // then clear data in UI for next sign
                    ClearForNewSign();

                    // finally close the window
                    p.Close();
                }


            });

            SaveCommand = new RelayCommand<object>((p) =>
            {
                if (sserial == null || sform == null) return false;
                return true;
            }, (p) =>
            {
                //InvoiceCreate.sub_total = invoiceSum.SubTotal;
                //InvoiceCreate.vat_amount = invoiceSum.VatTotal;
                //InvoiceCreate.total_amount = invoiceSum.TotalAmount;

                UImaping();

                //Validate General
                if (InvoiceCreate.errors.Count > 0)
                {
                    var e = InvoiceCreate.errors.FirstOrDefault();
                    if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
                    return;
                }

                // Validate Item List
                if (ProductList.Count > 0)
                {
                    foreach (var pro in ProductList)
                    {
                        if (pro.HasErrors)
                        {
                            var e = pro.errors.FirstOrDefault();
                            if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
                            return;
                        }

                    }
                }
                SaveInvoice();
            });

            SendCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {

                UImaping();

                //Validate General
                if (InvoiceCreate.errors.Count > 0)
                {
                    var e = InvoiceCreate.errors.FirstOrDefault();
                    if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
                    return;
                }

                // Validate Item List
                if (ProductList.Count > 0)
                {
                    foreach (var pro in ProductList)
                    {
                        if (pro.HasErrors)
                        {
                            var e = pro.errors.FirstOrDefault();
                            if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
                            return;
                        }

                    }
                }
                SaveInvoice();
                SendMailDraft(InvoiceCreate);
            });

            ViewCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ViewInvoice();
            });

            AddProductCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                AddProductInvoice();
            });

            RemoveProductCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RemoveProductInvoice();
            });

            RefreshProductCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RefreshProduct();
            });
            KeepCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                //decouple with UI
                UImaping();

                //Validate for DaySign 
                if (sDateSigned != null) InvoiceCreate.invoice_sign_date = sDateSigned.daytimeStamp;


                //Validate General
                if (InvoiceCreate.errors.Count > 0)
                {
                    var e = InvoiceCreate.errors.FirstOrDefault();
                    if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
                    return;
                }

                // Validate Item List
                if (ProductList.Count > 0)
                {
                    foreach (var pro in ProductList)
                    {
                        if (pro.HasErrors)
                        {
                            var e = pro.errors.FirstOrDefault();
                            if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
                            return;
                        }

                    }
                }

                // next is keep
                KeepInvoice();


            });

            CreateCustomerCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CreateCustomerFunction(); });
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void CreateCustomerFunction()
        {
            try
            {
                if (!Validate()) return;

                MessageBox.Show(_customerCreateRepository.Create(InvoiceCreate), Message.MSS_DIALOG_TITLE_ALERT);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private bool Validate()
        {
            try
            {
                if (string.IsNullOrEmpty(InvoiceCreate.b_tax_code) == false && InvoiceCreate.b_tax_code.Length < 10)
                {
                    MessageBox.Show("Mã số thuế không hợp lệ theo tiêu chuẩn Việt Nam", Message.MSS_DIALOG_TITLE_ALERT);
                    return false;
                }
                if (string.IsNullOrEmpty(InvoiceCreate.b_company) == true && string.IsNullOrEmpty(InvoiceCreate.b_tax_code) == false
                    || String.IsNullOrWhiteSpace(InvoiceCreate.b_company) == false && string.IsNullOrEmpty(InvoiceCreate.b_tax_code) == true)
                {
                    MessageBox.Show("Không được bỏ trống đồng thời tên công ty và mã số thuế", Message.MSS_DIALOG_TITLE_ALERT);
                    return false;
                }
                if (string.IsNullOrEmpty(InvoiceCreate.b_name) == true && string.IsNullOrEmpty(InvoiceCreate.b_company) == true
                   && String.IsNullOrEmpty(InvoiceCreate.b_tax_code) == true)
                {
                    MessageBox.Show("Không được bỏ trống đồng thời tên khách hàng, tên công ty và mã số thuế", Message.MSS_DIALOG_TITLE_ALERT);
                    return false;
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
            
            return true;
        }

        #region Prepare Function
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
                    _status = "draft";
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

        private void GetDataTest()
        {
            try
            {
                InvoiceCreate.b_company = "Công ty cổ phần công nghệ ARI";
                InvoiceCreate.b_email = "trandung@onlinevina.vn";
                //InvoiceCreate.b_tax_code = "0199235822";
                InvoiceCreate.b_address = "22/1/2 Nguyễn Văn Săng, p.Tân Sơn Nhì, q.Tân phú, TPHCM";
                //InvoiceCreate.b_bank_number = "245622355-2333";
                InvoiceCreate.in_word = "Một trăm sáu mươi lăm nghìn VND";
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        /// <summary>
        /// Sign
        /// the Invoice need to availble
        /// </summary>
        private void SignInvoiceXML()
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
                try
                {
                    signedXml.ComputeSignature();

                }
                catch
                {
                    MessageBox.Show(Message.MSS_INV_SIGNED_SELECTED_FAIL);
                }

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

        private void AddProductInvoice()
        {
            //var Count = ProductList.Count + 1;
            //ProductList.Add(new ProductUI()
            //{
            //	Stt = Count,
            //	Id = "" + Count,
            //	Name = "sanpham" + Count,
            //	ItemType = "",
            //	UnitName = "cái",
            //	Quantity = 1,
            //	UnitPrice = 100,
            //	VatPercentage = 10
            //});

            //OnPropertyChanged();
            int Count = 1;

            try
            {
                if (ProductList != null)
                {
                    Count = ProductList.Count + 1;

            }
            else
            {
                ProductList = new ObservableCollection<ProductUI>();
            }

            Const.FLG_ActiveTotalAmount = SettingEnableSubTotal();
            ProductUI itemNew = new ProductUI()
            {
                Stt = Count,
                ItemCode = "",
                Id = "",
                Name = "",
                ItemType = "",
                UnitName = "",
                Quantity = 1,
                UnitPrice = 0,
                VatPercentage = 10,
                IsTaxEnable = IsMoreTax,
                IsReverseCal = IsReverseCaculator,
                TotalAmountCheck = Const.FLG_ActiveTotalAmount
            };

            if (!IsMoreTax)
            {
                int taxValue;
                if (IndexTotalComboBox <= 1)
                {
                    taxValue = 0;
                }
                else if (IndexTotalComboBox == 2)
                {
                    taxValue = 5;
                }
                else if (IndexTotalComboBox == 3)
                {
                    taxValue = 10;
                }
                else
                {
                    taxValue = 10;
                }
                itemNew.VatPercentage = taxValue;
                itemNew.IndexTax = IndexTotalComboBox;

            }

            ProductList.Add(itemNew);

            OnPropertyChanged();
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public string SettingEnableSubTotal()
        {
            string jsonstringlu = "";
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(Path + "EditMonyAmount.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    jsonstringlu = sr.ReadToEnd();
                    if (jsonstringlu.Equals("true"))
                    {
                        jsonstringlu = "false";
                    }
                    else
                    {
                        jsonstringlu = "true";
                    }
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
            return jsonstringlu;
        }

        private void RemoveProductInvoice()
        {
            //if (ProductItemSeleced != null)
            //{
            //	int count = 0;
            //	foreach (var p in ProductList)
            //	{
            //		if (p.Stt != ProductItemSeleced.Stt)
            //		{
            //			count++;
            //			p.Stt = count;
            //		}

            //	}
            //if(!String.IsNullOrWhiteSpace(ProductItemSeleced.IdDatabase))	InvoiceCreate.delete_item_list.Add(ProductItemSeleced.IdDatabase);
            //	ProductList.Remove(ProductItemSeleced);
            try
            {
            ObservableCollection<ProductUI> temp = new ObservableCollection<ProductUI>();
            int count = 0;
            if (ProductList.Count > 0) foreach (var p in ProductList)
                {
                    if (p.IsSelected) temp.Add(p);
                    else
                    {
                        count++;
                        p.Stt = count;
                    }
                }
            if (temp.Count > 0) foreach (var p in temp)
                {
                    if (!String.IsNullOrWhiteSpace(p.IdDatabase)) InvoiceCreate.delete_item_list.Add(p.IdDatabase);
                    ProductList.Remove(p);
                }

        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void SendMailHtml(Invoice InvoiceCreate, string ScreenFlag)
        {
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
                //string body = File.ReadAllText(Spath + ".html", Encoding.UTF8);

                if (ScreenFlag == Const.FLG_SCREEN_INFO)
                {
                    SearchCode = InvoiceCreate.search_invoice_id;
                    InvoiceNo = InvoiceCreate.invoice_number.ToString("D7");
                }

                body = body.Replace("@EINVOICE_URL@", "https://hoadon.onlinevina.com.vn/invoice")
                .Replace("@EINVOICE@", "GTGT số :(" + InvoiceCreate.invoice_form_name + " " + InvoiceCreate.invoice_serial_name + "), " + InvoiceNo + ".")
                .Replace("@BUYER_COMPANY@", InvoiceCreate.b_company)
                .Replace("@SELLER_COMPANY@", enterpriseData.company_name)
                .Replace("@EINVOICE_KEY@", SearchCode)
                .Replace("@APP_NAME@", Const.APP_NAME);

                string attachmentPath = zipPathfile;

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

        public void SendMailDraft(Invoice InvoiceCreate)
        {
            // using for assign to send mail
            var enterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");
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
                string body = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Template/send_draft.html", Encoding.UTF8);
                //string body = File.ReadAllText(Spath + ".html", Encoding.UTF8);

                body = body.Replace("@EINVOICE_URL@", "https://hoadon.onlinevina.com.vn/invoice")
                .Replace("@EINVOICE@", "GTGT số :(" + sform.form_name + " " + sserial.serial_name + ") ")
                .Replace("@BUYER_COMPANY@", InvoiceCreate.b_company)
                .Replace("@SELLER_COMPANY@", enterpriseData.company_name)
                .Replace("@APP_NAME@", Const.APP_NAME);

                string attachmentPath = Spath + ".html";


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
                    MessageBox.Show(Message.MSS_SEND_EMAIL_SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Message.MSS_SEND_EMAIL_FAIL);

                    Console.Write("Exception in sendEmail:" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Create PDF file from HTML Function
        /// </summary>
        /// <returns></returns>
        private void CreateInvoicePdf()
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
            zipPathfile = ZipPath + ".zip";
            ZipFile.CreateFromDirectory(ZipPath, zipPathfile);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private int GetIndexFromTax(int taxValue)
        {
            int result = 0;
            try
            {
            if (taxValue == -1)
            {
                result = 0;
            }
            else if (taxValue == 0)
            {
                result = 1;
            }
            else if (taxValue == 5)
            {
                result = 2;
            }
            else if (taxValue == 10)
            {
                result = 3;
            }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }

            return result;
        }
        #endregion

        #region From  UI to XML
        private void MapingtoUI()
        {
            try
            {
            // map to ProducList
            ProductList = new ObservableCollection<ProductUI>();
            int count = 0;
            Const.FLG_ActiveTotalAmount = SettingEnableSubTotal();
            foreach (var i in InvoiceCreate.invoice_item_list)
            {
                count++;
                ProductList.Add(new ProductUI()
                {
                    Stt = count,
                    ItemCode = i.item_code,
                    Name = i.item_name,
                    ItemType = i.item_type,
                    UnitName = i.unit_name,
                    Quantity = i.quantity,
                    UnitPrice = i.unit_price,
                    VatPercentage = i.vat_percentage,
                    IdDatabase = i.id,
                    IndexTax = GetIndexFromTax(i.vat_percentage),
                    TotalAmountCheck = Const.FLG_ActiveTotalAmount

                });
            }

            //map form
            foreach (var f in FormLists)
            {
                if (InvoiceCreate.invoice_form_id == null)
                {
                    break;
                }
                else
                {
                    if (f.id == InvoiceCreate.invoice_form_id)
                    {
                        sform = f;
                        break;
                    }
                }

            }

            //map serial 
            foreach (var f in SerialListUI)
            {
                if (f.id == InvoiceCreate.invoice_serial_id)
                {
                    sserial = f;
                    break;
                }
            }

            //map products
            foreach (var w in PaymentLists)
            {
                if (w.id == InvoiceCreate.method_of_payment) spaymentMethod = w;
            }

            var signday = DateTimeConvert.GetdatetimeFromStamp(Convert.ToDouble(InvoiceCreate.invoice_sign_date));
            var signdaystring = signday.ToString("dd/MM/yyyy");
            //map daysign 
            if (DateSignedLists != null)
            {
                foreach (var f in DateSignedLists)
                {
                    if (f.dayview == signdaystring)
                    {
                        sDateSigned = f;
                        break;
                    }
                }
            }

            //Tax commbo box
            IndexTotalComboBox = GetIndexFromTax(InvoiceCreate.vat_rate);

            // incase of KeepInvoice
            if (IsKeepInvoice)
            {
                var lists = new List<InvoiceSerial>();
                var listf = new List<Form>();
                listf.Add(sform);
                lists.Add(sserial);

                SerialList = lists;
                FormLists = listf;

            }
            //map Discount charge
            invoiceSumExtend = new InvoiceSumExtention(InvoiceCreate.sub_total);
            invoiceSumExtend.DiscountTotal = InvoiceCreate.total_discount;
            invoiceSumExtend.ServiceChargeTotal = InvoiceCreate.total_service_charge;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }



        /// <summary>
        /// for asign and map parameter that can not binding directly from UI
        /// The function then decouple with UI and bussiness logic
        /// </summary>
        private void UImaping()
        {
            try
            {
            // map list products 
            var items = new List<InvoiceItem>();
            foreach (var p in _ProductList)
            {
                items.Add(new InvoiceItem
                {
                    id = p.IdDatabase,
                    current_code = InvoiceCreate.currency_code,
                    discount_amount = 0,//not implement 
                    item_code = p.ItemCode,
                    item_name = p.Name,
                    item_note = "",//not implement 
                    item_total_amount_without_vat = p.SubTotal,
                    item_type = p.ItemType,
                    quantity = p.Quantity,
                    total_amount = p.TotalAmount,
                    unit_name = p.UnitName,
                    unit_price = p.UnitPrice,
                    vat_amount = p.VatAmount,
                    vat_percentage = p.VatPercentage,


                });
            }
            if (sform != null && sserial != null)
            {
                InvoiceCreate.invoice_form_name = sform.form_name;
                InvoiceCreate.invoice_form_id = sform.id;
                InvoiceCreate.invoice_serial_name = sserial.serial_name;
                InvoiceCreate.invoice_serial_id = sserial.id;
                InvoiceCreate.method_of_payment = spaymentMethod.id;
                InvoiceCreate.invoice_item_list = items;
            }
            // Round 
            InvoiceCreate.total_amount = Math.Round(InvoiceCreate.total_amount, 0);
            InvoiceCreate.sub_total = Math.Round(InvoiceCreate.sub_total, 0);
            foreach (var p in InvoiceCreate.invoice_item_list)
            {
                p.total_amount = Math.Round(p.total_amount, 0);
                p.item_total_amount_without_vat = Math.Round(p.item_total_amount_without_vat, 0);
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void CreateInvoice()
        {
            try
            {
                //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US"); // chuan Anh
                //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR"); // Chuan Viet Nam
                //MessageBox.Show(Thread.CurrentThread.CurrentCulture.Name);
                string formatStringRound = String.Format("F{0:D}", 0);
                string formatString = String.Format("F{0:D}", StringFormat.decimalPlaces);

                var enterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");
                var Enterprise_Config_Detail = (Enterprise_ConfigData)ApplicationCache.GetItem("enterprise_config_Detail");


                ///Get XSLT templete and save 
                // save Request
                FormDetailRequest formdetailrequet = new FormDetailRequest
                {
                    enterprise_id = enterpriseData.id,
                    id = sform.id
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
                        itemDiscount = p.discount_amount.ToString(formatString),
                        itemName = p.item_name,
                        itemTotalAmountWithoutVat = p.item_total_amount_without_vat.ToString(formatStringRound),
                        quantity = p.quantity.ToString(),
                        totalAmount = p.total_amount.ToString(formatStringRound),
                        unitName = p.unit_name,
                        unitPrice = p.unit_price.ToString(formatString),
                        vatAmount = p.vat_amount.ToString(formatString),
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
                if (sDateSigned != null) ivdata.invoiceIssuedDate = sDateSigned.dayview; // Ngay ki hoa don*/
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
                ivdata.paymentMethodName = spaymentMethod.name;
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
                                paymentMethodNameExt = spaymentMethod.name,
                            }
                        }
                };
                ivdata.printFlag = "False";// not implement
                ivdata.printSample = "";// not implement
                ivdata.qrCodeData = SearchCode;
                ivdata.referentNo = "";// not implement

                ///Product infor
                ivdata.items = _items;
                ivdata.discountPercent = InvoiceCreate.discount_percent.ToString(formatString);
                ivdata.discountAmount = InvoiceCreate.total_discount.ToString(formatString);
                ivdata.ExchangeRate = InvoiceCreate.exchange_rate.ToString(formatString);
                ivdata.totalAmountWithoutVAT = InvoiceCreate.sub_total.ToString(formatString);
                ivdata.totalVATAmount = InvoiceCreate.vat_amount.ToString(formatString);
                ivdata.totalAmountWithVAT = InvoiceCreate.total_amount.ToString(formatStringRound);
                ivdata.serviceChargePercent = InvoiceCreate.service_charge_percent.ToString();
                ivdata.signedDate = InvoiceCreate.invoice_sign_date;// not implement,
                ivdata.submittedDate = "";// not implement
                ivdata.systemCode = "";// not implement
                ivdata.totalAmountWithVATInWords = InvoiceCreate.in_word;
                ivdata.totalServiceCharge = InvoiceCreate.total_service_charge.ToString(formatString);
                ivdata.userDefines = "";// not implement
                ivdata.vatPercentageBill = InvoiceCreate.vat_rate.ToString();// not implement

                /// Seller Infor
                /// 
                ivdata.sellerEmail = enterpriseData.mail_invoice;
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
            catch(Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }



        }

        //private void CreateInvoice()
        //{
        //	//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US"); // chuan Anh
        //	//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR"); // Chuan Viet Nam
        //	//  MessageBox.Show(Thread.CurrentThread.CurrentCulture.Name);
        //	string formatString = String.Format("F{0:D}", StringFormat.decimalPlaces);

        //	var enterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");


        //	///Get XSLT templete and save 
        //	// save Request
        //	FormDetailRequest formdetailrequet = new FormDetailRequest
        //	{
        //		enterprise_id = enterpriseData.id,
        //		id = sform.id
        //	};
        //	InvoiceRestApi api = new InvoiceRestApi();
        //	var response = api.GetForm(formdetailrequet);
        //	System.IO.File.WriteAllText(Xsltpath, response.data.xslt_content);


        //	/// map list items to Xml_Itemlist
        //	invoiceInvoiceDataItems _items = new invoiceInvoiceDataItems();

        //	int count = 0;

        //	foreach (var p in InvoiceCreate.invoice_item_list)
        //	{
        //		count++;
        //		var item = new invoiceInvoiceDataItemsItem()
        //		{
        //			currency = InvoiceCreate.currency_code,
        //			itemCode = p.item_code,
        //			itemDiscount = p.discount_amount.ToString(formatString),
        //			itemName = p.item_name,
        //			itemTotalAmountWithoutVat = p.item_total_amount_without_vat.ToString(formatString),
        //			quantity = p.quantity.ToString(),
        //			totalAmount = p.total_amount.ToString(formatString),
        //			unitName = p.unit_name,
        //			unitPrice = p.unit_price.ToString(formatString),
        //			vatAmount = p.vat_amount.ToString(formatString),
        //			vatPercentage = p.vat_percentage.ToString(),
        //			lineNumber = count.ToString(),
        //		};
        //		_items.items.Add(item);
        //	}

        //	/// Create XML Object
        //	invoice iv = _InvoiceSerialization.Invoice;

        //	invoiceInvoiceData ivdata = iv.invoiceData;

        //	// metadata
        //	ivdata.id = "Data";
        //	/// Buyer infor
        //	ivdata.buyerAddressLine = InvoiceCreate.b_address;
        //	ivdata.buyerBankAccount = InvoiceCreate.b_bank_number;
        //	ivdata.buyerEmail = InvoiceCreate.b_email;
        //	ivdata.buyerBankName = InvoiceCreate.b_bank_name;
        //	ivdata.buyerDisplayName = InvoiceCreate.b_name;
        //	ivdata.buyerFaxNumber = InvoiceCreate.b_fax_number;
        //	ivdata.buyerLegalName = InvoiceCreate.b_company;
        //	ivdata.buyerPhoneNumber = InvoiceCreate.b_phone_number;
        //	ivdata.buyerTaxCode = InvoiceCreate.b_tax_code;

        //	/// Invoice Infor
        //	ivdata.invoiceAppRecordId = "";// not implement
        //	if (sDateSigned != null) ivdata.invoiceIssuedDate = sDateSigned.dayview; // Ngay ki hoa don*/
        //	ivdata.invoiceName = InvoiceCreate.invoice_file_name;
        //	ivdata.invoiceNote = InvoiceCreate.invoice_note;
        //	ivdata.invoiceNumber = InvoiceNo;
        //	ivdata.invoiceSeries = InvoiceCreate.invoice_serial_name;
        //	ivdata.templateCode = InvoiceCreate.invoice_form_name;
        //	ivdata.additionalReferenceDesc = "";// not implement
        //	ivdata.adjustmentType = "";// not implement
        //	ivdata.contractNumber = "";// not implement
        //	ivdata.currencyCode = InvoiceCreate.currency_code;
        //	ivdata.delivery = "";// not implement
        //	ivdata.paymentMethodName = spaymentMethod.name;
        //	ivdata.invoiceSigned = "";// not implement
        //	ivdata.invoiceType = "";// not implement
        //	ivdata.originalInvoice = "";// not implement
        //	ivdata.invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
        //	{
        //		InvoiceTaxBreakdowns = new List<invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown>()
        //				{
        //					new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
        //					{
        //						vatPercentage = "",// example
        //vatTaxableAmount = "",
        //						vatTaxAmount = ""
        //					}
        //				}

        //	};// not implement
        //	ivdata.payments = new invoiceInvoiceDataPayments()
        //	{
        //		payments = new List<invoiceInvoiceDataPaymentsPayment>()
        //				{
        //					new invoiceInvoiceDataPaymentsPayment()
        //					{
        //						paymentMethodNameExt = spaymentMethod.name,
        //					}
        //				}
        //	};
        //	ivdata.printFlag = "False";// not implement
        //	ivdata.printSample = "";// not implement
        //	ivdata.qrCodeData = SearchCode;
        //	ivdata.referentNo = "";// not implement

        //	///Product infor
        //	ivdata.items = _items;
        //	ivdata.discountAmount = InvoiceCreate.total_discount.ToString(formatString);
        //	ivdata.ExchangeRate = InvoiceCreate.exchange_rate.ToString(formatString);
        //	ivdata.totalAmountWithoutVAT = InvoiceCreate.sub_total.ToString(formatString);
        //	ivdata.totalVATAmount = InvoiceCreate.vat_amount.ToString(formatString);
        //	ivdata.totalAmountWithVAT = InvoiceCreate.total_amount.ToString(formatString);
        //	ivdata.serviceChargePercent = InvoiceCreate.service_charge_percent.ToString();
        //	ivdata.signedDate = InvoiceCreate.invoice_sign_date;// not implement,
        //	ivdata.submittedDate = "";// not implement
        //	ivdata.systemCode = "";// not implement
        //	ivdata.totalAmountWithVATInWords = InvoiceCreate.in_word;
        //	ivdata.totalServiceCharge = InvoiceCreate.total_service_charge.ToString(formatString);
        //	ivdata.userDefines = "";// not implement
        //	ivdata.vatPercentageBill = InvoiceCreate.vat_rate.ToString(); // not implement

        //	/// Seller Infor
        //	ivdata.sellerEmail = enterpriseData.email;
        //	ivdata.sellerAddressLine = enterpriseData.address;
        //	ivdata.sellerAppRecordId = "";// not implement
        //	ivdata.sellerBankAccount = enterpriseData.bank_number;
        //	ivdata.sellerBankName = enterpriseData.bank_name;
        //	ivdata.sellerContactPersonName = enterpriseData.manage_by;
        //	ivdata.sellerFaxNumber = enterpriseData.fax;
        //	ivdata.sellerLegalName = enterpriseData.company_name;
        //	ivdata.sellerPhoneNumber = enterpriseData.phone_number;
        //	ivdata.sellerSignedPersonName = "";// not implement
        //	ivdata.sellerSubmittedPersonName = "";// not implement
        //	ivdata.sellerTaxCode = enterpriseData.tax_code;
        //	ivdata.sellerWebsite = enterpriseData.website;

        //	/// Create files
        //	_InvoiceSerialization.ToInvoiceXml(Spath + ".xml");

        //	_InvoiceSerialization.ToInvoiceHtml(Spath + ".xml", Spath + ".html");


        //}


        #endregion

        #region Invoice Logic Process

        public void refreshListProduct()
        {
            try
            {
            if (ProductList != null)
            {
                ObservableCollection<ProductUI> ProductListTemp = new ObservableCollection<ProductUI>();
                foreach (ProductUI item in ProductList)
                {
                    if (!IsMoreTax)
                    {
                        item.IndexTax = IndexTotalComboBox;
                    }
                    item.IsTaxEnable = IsMoreTax;

                    ProductListTemp.Add(item);
                }


                ProductList = new ObservableCollection<ProductUI>();
                foreach (ProductUI item in ProductListTemp)
                {
                    ProductList.Add(item);
                }
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }


        public void RefreshProduct()
        {
            try
        {
            invoiceSum = new InvoiceSum();
            if (ProductList != null && ProductList.Count > 0)
            {
                foreach (ProductUI list in ProductList)
                {
                    invoiceSum.SubTotal = invoiceSum.SubTotal + list.SubTotal * InvoiceCreate.exchange_rate;
                    invoiceSum.VatTotal = invoiceSum.VatTotal + list.VatAmount * InvoiceCreate.exchange_rate;
                    Console.WriteLine("sp: " + list.VatAmount);
                    invoiceSum.TotalAmount = invoiceSum.TotalAmount + list.TotalAmount * InvoiceCreate.exchange_rate;
                }
                #region Discount && Service charge Computing implement by DUC TRAN
                invoiceSumExtend.SubTotal = invoiceSum.SubTotal;
                int Tax = 10;
                if (!IsMoreTax) Tax = InvoiceCreate.vat_rate;
                ///discount
                double discountVat = invoiceSumExtend.DiscountTotal * Tax / 100;
                invoiceSum.VatTotal -= discountVat;
                invoiceSum.SubTotal -= invoiceSumExtend.DiscountTotal;
                invoiceSum.TotalAmount = invoiceSum.TotalAmount - invoiceSumExtend.DiscountTotal - discountVat;
                InvoiceCreate.discount_percent = (int)invoiceSumExtend.DiscountPercent;
                InvoiceCreate.total_discount = invoiceSumExtend.DiscountTotal;
                /// service Charge		
                double servicechargeVat = invoiceSumExtend.ServiceChargeTotal * Tax / 100;
                invoiceSum.VatTotal += servicechargeVat;
                invoiceSum.SubTotal += invoiceSumExtend.ServiceChargeTotal;
                invoiceSum.TotalAmount += invoiceSumExtend.ServiceChargeTotal + servicechargeVat;
                InvoiceCreate.service_charge_percent = (int)invoiceSumExtend.ServiceChargePercent;
                InvoiceCreate.total_service_charge = invoiceSumExtend.ServiceChargeTotal;
                #endregion

                //if (!IsMoreTax)
                //{
                //    int taxValue = 0;

                //    if (InvoiceCreate.vat_rate > 0)
                //    {
                //        taxValue = InvoiceCreate.vat_rate ;
                //    }

                //    invoiceSum.VatTotal = invoiceSum.SubTotal * taxValue/100;
                //    invoiceSum.TotalAmount = invoiceSum.SubTotal + invoiceSum.VatTotal;

                //}
                //Lam tron
                invoiceSum.SubTotal = Math.Round(invoiceSum.SubTotal, 0, MidpointRounding.AwayFromZero);
                invoiceSum.VatTotal = Math.Round(invoiceSum.VatTotal, 0, MidpointRounding.AwayFromZero);
                invoiceSum.TotalAmount = Math.Round(invoiceSum.TotalAmount, 0, MidpointRounding.AwayFromZero);

                InvoiceCreate.sub_total = invoiceSum.SubTotal;
                InvoiceCreate.vat_amount = invoiceSum.VatTotal;
                InvoiceCreate.total_amount = invoiceSum.TotalAmount;


                OnPropertyChanged();
            }
            InvoiceCreate.in_word = StringFormat.UppercaseFirst(StringFormat.So_chu(InvoiceCreate.total_amount) + " đồng");
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }


        private void ViewInvoice()
        {
            try
        {
            if (ViewMode)
            {
                System.Diagnostics.Process.Start(Spath + ".html");
                ViewMode = false;

            }
            else
            {
                //decouple with Bussiness Logic
                UImaping();
                // config parameter for saving draft 
                InvoiceCreate.status = 0;
                InvoiceCreate.invoice_sign_date = "";
                InvoiceNo = "";

                // Create XML,Html,pdf
                Spath = "TemptInvoice";
                CreateInvoice();
                System.Diagnostics.Process.Start(Spath + ".html");
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void SaveInvoice()
        {
            try
            {
            ViewMode = true;

            InvoiceCreate.vat_amount = invoiceSum.VatTotal;
            InvoiceCreate.total_amount = invoiceSum.TotalAmount;

            // config parameter for saving draft 
            InvoiceCreate.status = 0;
            InvoiceCreate.invoice_sign_date = "";
            InvoiceNo = "";

            // Create XML,Html,pdf
            ZipPath = Dpath + "//" + "InvoiceDraft";
            Directory.CreateDirectory(ZipPath);
            Spath = ZipPath + "//" + "Draft";
            CreateInvoice();

            InvoiceCreate.xml_content = System.IO.File.ReadAllText(Spath + ".xml");

            // save Request
            InvoiceRestApi api = new InvoiceRestApi();
            var response = api.Edit(InvoiceCreate);
            if (response != null)
            {
                MessageBox.Show(response.message);
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void SignInvoice()
        {
            try
            {
            #region force Ground
            ViewMode = true;

            // config parameter for saving draft 		
            InvoiceCreate.xml_content = "";
            InvoiceCreate.status = 1;

            // sign Request
            InvoiceRestApi api = new InvoiceRestApi();
            var response = api.Edit(InvoiceCreate);
            if (response == null) return;
            if (response.code != Const.Code_Successful)
            {
                MessageBox.Show(response.message);
                return;
            }
            SearchCode = response.data.search_invoice_id;
            InvoiceNo = response.data.invoice_number.ToString("D7");
            ZipPath = Ipath + "//" + InvoiceCreate.b_tax_code + "_" + response.data.search_invoice_id + "_" + DateTime.Now.ToString("yyyy_dd_MM_HHmmss");
            Directory.CreateDirectory(ZipPath);
            Spath = ZipPath + "/" + GetNameInvoice(sform.form_name, sserial.serial_name, response.data.search_invoice_id, sDateSigned.daytimeStamp, "1");
            CreateInvoice();
            SignInvoiceXML();


            // save Request
            InvoiceRestApi apis = new InvoiceRestApi();
            var Saverequets = new InvoiceUpdateXMLRequest()
            {

                xml_update_list = new List<Xml_Update>()
                {
                    new Xml_Update()
                    {
                        invoice_id = response.data.invoice_id,
                        xml_content = System.IO.File.ReadAllText(Spath + ".xml")
                    }
                }

            };
            var Saveresponse = apis.SaveXML(Saverequets);

            MessageBox.Show(Message.MSS_INV_SIGNED_SIGNED + response.message);
            ViewInvoice();

            CreateInvoicePdf();

            //Sendmail
            if (InvoiceCreate.send_mail)
            {
                SendMailHtml(InvoiceCreate, Const.FLG_SCREEN_EDIT);
            }
            #endregion

            #region Background Task

            //BackgroundWorker worker = new BackgroundWorker();
            //worker.WorkerSupportsCancellation = true;
            //worker.DoWork += delegate (object s, DoWorkEventArgs args)
            //{
            //	CreateInvoicePdf();
            //	if (InvoiceCreate.send_mail)
            //	{
            //		SendMailHtml(invoiceTemp);
            //	}

            //};

            //worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
            //{
            //	if (args.Error != null)
            //	{
            //		Debug.WriteLine("Error");
            //	}
            //	Debug.WriteLine("Running Done");
            //	worker.CancelAsync();
            //};

            //worker.RunWorkerAsync();

            #endregion
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
            #region force Ground
            ViewMode = true;

            // config parameter for saving draft 		
            InvoiceCreate.xml_content = "";
            InvoiceCreate.status = 2;

            // sign Request
            InvoiceRestApi api = new InvoiceRestApi();
            var response = api.Create(InvoiceCreate);

            if (response.code != Const.Code_Successful)
            {
                MessageBox.Show(response.message);
                return;
            }
            SearchCode = response.data.search_invoice_id;
            InvoiceNo = response.data.invoice_number.ToString("D7");
            ZipPath = Dpath + "//" + "InvoiceDraft";
            Directory.CreateDirectory(ZipPath);
            Spath = ZipPath + "//" + "Draft";
            CreateInvoice();

            // save Request
            InvoiceRestApi apis = new InvoiceRestApi();
            var Saverequets = new InvoiceUpdateXMLRequest()
            {
                xml_update_list = new List<Xml_Update>()
                {
                    new Xml_Update()
                    {
                        invoice_id = response.data.invoice_id,
                        xml_content = System.IO.File.ReadAllText(Spath + ".xml")
                    }
                }

            };
            var Saveresponse = apis.SaveXML(Saverequets);

            MessageBox.Show(response.message + " And " + Saveresponse.message);

            #endregion
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
        #endregion

        #region Function Using For InforViewModel#
        public void ViewInvoiceXML()
        {
            try
        {
            string xmlTempPath = "InvoiceTemp.xml";
            System.IO.File.WriteAllText(xmlTempPath, InvoiceCreate.xml_content);

            if (InvoiceCreate.status == 3)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlTempPath);
                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");
                var node = xmlDoc.SelectSingleNode("//inv:invoiceData//inv:invoiceType", nsmgr);
                node.InnerText = "DELETED";
                xmlDoc.Save(xmlTempPath);

            }
            System.Diagnostics.Process.Start(xmlTempPath);
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void ViewInvoiceHtml()
        {
            try
            {
            string xmlTempPath = "InvoiceTemp.xml";
            string htmlTempPath = "Invoicetemp.html";
            if (!GetXslt())
            {
                MessageBox.Show("Không tải về được file xslt mẫu, Vui lòng thử lại !!!", "Vina invoice thông báo:");
                return;
            }
            System.IO.File.WriteAllText(xmlTempPath, InvoiceCreate.xml_content);

            if (InvoiceCreate.status == 3)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlTempPath);
                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");
                var node = xmlDoc.SelectSingleNode("//inv:invoiceData//inv:invoiceType", nsmgr);
                node.InnerText = "DELETED";
                xmlDoc.Save(xmlTempPath);

            }
            _InvoiceSerialization.ToInvoiceHtml(xmlTempPath, htmlTempPath);
            System.Diagnostics.Process.Start(htmlTempPath);
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void ViewInvoicePdf()
        {
            try
        {
            string xmlTempPath = "InvoiceTemp.xml";
            string htmlTempPath = "Invoicetemp.html";
            string pdfTempPath = "Invoicetemp.pdf";
            System.IO.File.WriteAllText(xmlTempPath, InvoiceCreate.xml_content);

            if (InvoiceCreate.status == 3)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlTempPath);
                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");
                var node = xmlDoc.SelectSingleNode("//inv:invoiceData//inv:invoiceType", nsmgr);
                node.InnerText = "DELETED";
                xmlDoc.Save(xmlTempPath);

            }
            _InvoiceSerialization.ToInvoiceHtml(xmlTempPath, htmlTempPath);

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
                    new ObjectSettings { PageUrl = htmlTempPath },
                }
            };
            var converter = new StandardConverter(
                new PdfToolset(
                    new WinAnyCPUEmbeddedDeployment(
                        new TempFolderDeployment())));

            var result = converter.Convert(document);
            //Path to e-invoice pdf file
            using (var fs = new FileStream(pdfTempPath, FileMode.Create))
            {
                fs.Write(result, 0, result.Length);
                fs.Flush();
            }

            System.Diagnostics.Process.Start(pdfTempPath);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void ConvertInvoice()
        {
            try
        {
            string temptPath = "ConvertInvoiceTemp.xml";
            System.IO.File.WriteAllText(temptPath, InvoiceCreate.xml_content);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(temptPath);
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");
            var node = xmlDoc.SelectSingleNode("//inv:invoiceData//inv:printFlag", nsmgr);
            node.InnerText = "True";
            xmlDoc.Save(temptPath);

            InvoiceCreate.xml_content = System.IO.File.ReadAllText(temptPath);

            // call API to Convert
            // server will Save new singed invoice and send back to clinet the id
            InvoiceRestApi apis = new InvoiceRestApi();
            var requets = new InvoiceConvertRequest()
            {
                id = InvoiceCreate.id,
                xml_content = InvoiceCreate.xml_content
            };
            var response1 = apis.Convert(requets);
            if (response1.code != Const.Code_Successful)
            {
                MessageBox.Show(Message.MSS_ALERT_INVOICE_CONVERT_FAIL);
                return;
            }
            else MessageBox.Show(Message.MSS_ALERT_INVOICE_CONVERT_SUCCESS);


            //View 
            string htmlpath = "ConvertInvoiceTemp.html";
            _InvoiceSerialization.ToInvoiceHtml(temptPath, htmlpath);
            System.Diagnostics.Process.Start(htmlpath);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void SendMail()
        {
            try
        {
            string xmlTempPath = "";

            ZipPath = Ipath + "//" + InvoiceCreate.b_tax_code + "_" + InvoiceCreate.search_invoice_id + "_" + DateTime.Now.ToString("yyyy_dd_MM_HHmmss");
            Directory.CreateDirectory(ZipPath);
            Spath = ZipPath + "/" + GetNameInvoice(InvoiceCreate.invoice_form_name, InvoiceCreate.invoice_serial_name, InvoiceCreate.id, InvoiceCreate.invoice_sign_date, "1");


            if (InvoiceCreate.status == 3)
            {
                xmlTempPath = "InvoiceTemp.xml";
                System.IO.File.WriteAllText(xmlTempPath + ".xml", InvoiceCreate.xml_content);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlTempPath);
                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");
                var node = xmlDoc.SelectSingleNode("//inv:invoiceData//inv:invoiceType", nsmgr);
                node.InnerText = "DELETED";
                xmlDoc.Save(xmlTempPath);

            }
            else
            {
                xmlTempPath = Spath;
                System.IO.File.WriteAllText(xmlTempPath + ".xml", InvoiceCreate.xml_content);
            }


            _InvoiceSerialization.ToInvoiceHtml(xmlTempPath + ".xml", Spath + ".html");
            CreateInvoicePdf();
            if (InvoiceCreate.status == 1)
            {
                SendMailHtml(InvoiceCreate, Const.FLG_SCREEN_INFO);

            }
            else
            {
                SendMailHtml(InvoiceCreate, Const.FLG_SCREEN_EDIT);

            }

            MessageBox.Show(Message.MSS_SEND_EMAIL_SUCCESS, Message.MSS_DIALOG_TITLE_ALERT);
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private bool GetXslt()
        {
            try
            {
                var enterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");

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
                return true;
            }
            catch
            {
                return false;
            }

        }

        #endregion

        #region Prepare And PreLoad Data
        private void ClearForNewSign()
        {
            InvoiceCreate = new Invoice();
        }

        /// <summary>
        /// Main Prepare
        /// This call all function for preload data
        /// </summary>
        public void Init()
        {
            try
            {
            StatusBarString = Const.STATUS_BAR_STRING;
            IsReverseCaculator = false;
        
            var Enterprise_Config_Detail = (Enterprise_ConfigData)ApplicationCache.GetItem("enterprise_config_Detail");

            InvoiceCreate.send_mail = true;
            _SerialNumber = Enterprise_Config_Detail.token_serial;
            invoiceSum = new InvoiceSum();
            invoiceSumExtend = new InvoiceSumExtention(0);

            //GetDataTest();
            // for sugestion function
            PrepareProductList();
            PrepareCustomerList();

            TaxCodeList = Const.GetTaxCodeList();
            MoneyUnits = Const.GetMoneyUnit();
            PaymentLists = Const.GetPaymentList();

            SearchCertificate();
            getFormList();
            getSerialList();

            RefreshProduct();
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void PrepareCustomerList()
        {
            try
        {
            IEnumerable<Customer> list;
            DataCustomerList = new ObservableCollection<Customer>();
            int count = 0;
            _Repository.Page = -1; // get all customer
            list = _Repository.GetList();

            if (list.Count() > 0)
            {


                foreach (Customer p in list)
                {
                    count++;
                    p.STT = count;
                    DataCustomerList.Add(p);
                }
            }
            else
            {
                return;
            }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void PrepareProductList()
        {
            try
        {
            IEnumerable<Product> list;
            DataProductList = new ObservableCollection<Product>();
            int count = 0;
            _ProductRepository.Page = -1;//get all product
            list = _ProductRepository.GetList();

            if (list.Count() > 0)
            {


                foreach (Product p in list)
                {
                    count++;
                    p.Stt = count;
                    DataProductList.Add(p);
                }
            }
            else
            {
                return;
            }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void SetItemTax(int Index)
        {
            try
        {
            if (ProductList != null)
            {

                ObservableCollection<ProductUI> ProductListTemp = new ObservableCollection<ProductUI>();
                foreach (ProductUI item in ProductList)
                {
                    int taxValue;
                    if (Index <= 1)
                    {
                        taxValue = 0;
                    }
                    else if (Index == 2)
                    {
                        taxValue = 5;
                    }
                    else if (Index == 3)
                    {
                        taxValue = 10;
                    }
                    else
                    {
                        taxValue = 10;
                    }
                    item.VatPercentage = taxValue;
                    item.IndexTax = Index;
                    item.IsTaxEnable = IsMoreTax;
                    ProductListTemp.Add(item);
                }


                ProductList = new ObservableCollection<ProductUI>();
                foreach (ProductUI item in ProductListTemp)
                {
                    ProductList.Add(item);
                }

                invoiceSum = new InvoiceSum();
                if (ProductList != null && ProductList.Count > 0)
                {
                    foreach (ProductUI list in ProductList)
                    {
                        invoiceSum.SubTotal = invoiceSum.SubTotal + list.SubTotal * InvoiceCreate.exchange_rate;
                        invoiceSum.VatTotal = invoiceSum.VatTotal + list.VatAmount * InvoiceCreate.exchange_rate;
                        invoiceSum.TotalAmount = invoiceSum.TotalAmount + list.TotalAmount * InvoiceCreate.exchange_rate;
                    }
                    //Làm tròn
                    invoiceSum.SubTotal = Math.Round(invoiceSum.SubTotal, 0);
                    invoiceSum.VatTotal = Math.Round(invoiceSum.VatTotal, 0);
                    invoiceSum.TotalAmount = Math.Round(invoiceSum.TotalAmount, 0);

                    InvoiceCreate.sub_total = invoiceSum.SubTotal;
                    InvoiceCreate.vat_amount = invoiceSum.VatTotal;
                    InvoiceCreate.total_amount = invoiceSum.TotalAmount;


                    OnPropertyChanged();
                }
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }



        /// <summary>
        /// Search Customer Function
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        private List<Invoice_Customer_Search> CustomerSearch(CustomerSearchRequest searchType)
        {
            CustomerRestApi customerRestApi = new CustomerRestApi();
            List<Invoice_Customer_Search> invoice_Customer_Searches_List  = new List<Invoice_Customer_Search>();
            try
            {
                invoice_Customer_Searches_List = customerRestApi.SearchApiBase(searchType).data.invoice_customer_list;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }

            return invoice_Customer_Searches_List;
        }

        /// <summary>
        /// Search Product Function
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        private List<Product> ProductSearch(ProductSearchRequest searchType)
        {
            ProductRestApi RestApi = new ProductRestApi();
            List<Product> products_List = new List<Product>();
            try
            {
                products_List = RestApi.SearchApiBase(searchType).data.invoice_product_list;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }

            return products_List;
        }

        private void GetDateSigned()
        {
            try
        {
            DateSignedLists = new ObservableCollection<SignDay>();
            if (sserial == null) return;
            DateSignedLists = DateTimeConvert.GetListSignDay(DateTimeConvert.GetdatetimeFromStamp((double)sserial.last_used_date), DateTime.Now);

            if (IsKeepInvoice)
            {
                InvoiceRestApi RestApi = new InvoiceRestApi();
                InvoiceKeepDate keepInvoiceDay = RestApi.GetDayOfInvoiceKeep(InvoiceCreate.id);

                var signdayfrom = DateTimeConvert.GetdatetimeFromStamp((double)keepInvoiceDay.start_date);
                var signdayto = DateTimeConvert.GetdatetimeFromStamp((double)keepInvoiceDay.stop_date);
                DateSignedLists = DateTimeConvert.GetListSignDay(signdayfrom, signdayto);

            }
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

        private void ShowCertificate()
        {
            try
            {
                if (_x509 != null)
                {
                    X509Certificate2UI.DisplayCertificate(_x509);
                    MessageBox.Show(System.Text.Encoding.Default.GetString(_x509.RawData));
                }
                else
                {
                    MessageBox.Show("Không tìm thấy chứng thư số thích hợp");
                }
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
            //_SerialNumber = "540806ddf6d0c8eb0382cc6dac0eab02"; // for test NEED TO DELETE THIS LINE 

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
            try { 
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
                try { 
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
                    }catch { }
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
            catch(Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
            return true;
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




    }

}
