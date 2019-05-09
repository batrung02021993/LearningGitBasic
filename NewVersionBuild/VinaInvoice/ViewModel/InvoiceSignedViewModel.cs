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
using VinaInvoice.Model.JsonObjectModel;
using System.Data;
using System.IO;
using OfficeOpenXml;
using VinaInvoice.Common;
using System.Windows.Forms;

namespace VinaInvoice.ViewModel
{
    public class InvoiceSignedViewModel : BaseViewModel
    {

        InvoiceRepository _InvoiceRepository = new InvoiceRepository();

        #region Comand
        public ICommand CreateCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ExportCommandNormal { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand InvoiceDoubleClickCommand { get; set; }
        private bool IssearchPressed = false;
        #endregion

        public bool Isloaded = false;

        private ObservableCollection<Invoice> _InvoiceSignedList;
        public ObservableCollection<Invoice> InvoiceSignedList { get => _InvoiceSignedList; set { _InvoiceSignedList = value; OnPropertyChanged(); } }
		private Invoice _InvoiceItemSeleced = new Invoice();
		public Invoice InvoiceItemSeleced { get { return _InvoiceItemSeleced; } set { _InvoiceItemSeleced = value; } }

		private ObservableCollection<CBMethod> _sortList;

        public CBMethod _sSortList = new CBMethod();
        public CBMethod sSortList { get => _sSortList; set { _sSortList = value; OnPropertyChanged(); } }

        public string Companyforsearching { get => InvoiceInvoice_Variable.companyforsearching; set { InvoiceInvoice_Variable.companyforsearching = value; OnPropertyChanged(); } }
        public string TaxCodeforsearching { get => InvoiceInvoice_Variable.taxcodeforsearching; set { InvoiceInvoice_Variable.taxcodeforsearching = value; OnPropertyChanged(); } }
        public string InvoiceNumberforsearching { get => InvoiceInvoice_Variable.invoicenumberforsearching; set { InvoiceInvoice_Variable.invoicenumberforsearching = value; OnPropertyChanged(); } }
        public string InvoiceRefforsearching { get => InvoiceInvoice_Variable.invoicerefforsearching; set { InvoiceInvoice_Variable.invoicerefforsearching = value; OnPropertyChanged(); } }
        public string SerialNameforsearching { get => InvoiceInvoice_Variable.serialnameforsearching; set { InvoiceInvoice_Variable.serialnameforsearching = value; OnPropertyChanged(); } }

        public DateTime Start_date
		{
			get => InvoiceInvoice_Variable.start_date;
			set
			{
				InvoiceInvoice_Variable.start_date = value;
				OnPropertyChanged();
			}
		}
        public DateTime Stop_date { get => InvoiceInvoice_Variable.stop_date; set { InvoiceInvoice_Variable.stop_date = value; OnPropertyChanged(); } }

        private int _page = 1;
        public int Page { get => _page; set { _page = value; OnPropertyChanged(); } }

		int[] LoadStatus = { 1 };
		int[] LoadStateofBill = { 0, 1, 3, 4 };

        public InvoiceSignedViewModel()
        {
            try
        {
            StatusBarString = Const.STATUS_BAR_STRING;

            getSortList();

            SortCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
            var item = p;
                if (item != null)
                {
                    if (item.ToString().Equals("Increase"))
                        InvoiceInvoice_Variable.sort_method = 0;
                    else if (item.ToString().Equals("Decrease"))
                        InvoiceInvoice_Variable.sort_method = 1;
                    else if (item.ToString().Equals("Page_Increase"))
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

                if (!IssearchPressed)
                {
                    LoadListInvoice();
                }else
                {
                    Search(p);
                }

            });

            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            
            {
                IssearchPressed = true;
                Page = 1;
                Search(p); });

            ExportCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Export(); });

            ExportCommandNormal = new RelayCommand<object>((p) => { return true; }, (p) => { ExportNormal(); });

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Isloaded = true;
            });

            InvoiceDoubleClickCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
				InvoiceDoubleClick();
            });

            LoadListInvoice();
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void LoadListInvoice()
        {
            try
        {
            IEnumerable<Invoice> list;
            _InvoiceRepository.Page = Page;
            list = _InvoiceRepository.Search(5, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, "", LoadStatus, LoadStateofBill, null, null);

            if (list.Count() > 0)
            {
                InvoiceSignedList = new ObservableCollection<Invoice>();
                int count = 0;
                foreach (Invoice p in list)
                {
                    count++;
                    p.Stt = count;
                    InvoiceSignedList.Add(p);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You are in the last page !!!", Common.Message.MSS_DIALOG_TITLE_ALERT);
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

		private void InvoiceDoubleClick()
		{
            try {
			    var api = new InvoiceRestApi();
			    var response = api.GetInvoiceDetail(InvoiceItemSeleced.id);

			    if (response.code != Const.Code_Successful) System.Windows.MessageBox.Show(response.message);
		
			    InvoiceInfoWindow wd = new InvoiceInfoWindow(response.data);
                wd.SetParent(this);
                wd.ShowDialog();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void Sort(object item)
        {
            try
        {
            if (item != null)
            {
                if (item.ToString().Equals("Increase"))
                    InvoiceInvoice_Variable.sort_method = 0;
                else if (item.ToString().Equals("Decrease"))
                    InvoiceInvoice_Variable.sort_method = 1;
                else if (item.ToString().Equals("Page_Increase"))
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
            InvoiceSignedList = new ObservableCollection<Invoice>();

            if (Companyforsearching != "")
            {
                list = _InvoiceRepository.Search(0, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, Companyforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (TaxCodeforsearching != "")
            {
                list = _InvoiceRepository.Search(1, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, TaxCodeforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (InvoiceNumberforsearching != "")
            {
                list = _InvoiceRepository.Search(2, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceNumberforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (InvoiceRefforsearching != "")
            {
                list = _InvoiceRepository.Search(3, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceRefforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (SerialNameforsearching != "")
            {
                list = _InvoiceRepository.Search(4, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, SerialNameforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else
            {
                list = _InvoiceRepository.Search(5, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, "", LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }

            if (list.Count() > 0)
            {
                int count = 0;
                foreach (Invoice p in list)
                {
                    count++;
                    p.Stt = count;
                    InvoiceSignedList.Add(p);
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
            InvoiceSignedList = new ObservableCollection<Invoice>();

            if (Companyforsearching != "")
            {
                list = _InvoiceRepository.Search(0, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, Companyforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (TaxCodeforsearching != "")
            {
                list = _InvoiceRepository.Search(1, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, TaxCodeforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (InvoiceNumberforsearching != "")
            {
                list = _InvoiceRepository.Search(2, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceNumberforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (InvoiceRefforsearching != "")
            {
                list = _InvoiceRepository.Search(3, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceRefforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else if (SerialNameforsearching != "")
            {
                list = _InvoiceRepository.Search(4, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, SerialNameforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }
            else
            {
                list = _InvoiceRepository.Search(5, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, "", LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }

            if (list.Count() > 0)
            {                
                int count = 0;
                foreach (Invoice p in list)
                {
                    count++;
                    p.Stt = count;
                    InvoiceSignedList.Add(p);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Không tìm thấy hóa đơn bạn muốn", Common.Message.MSS_DIALOG_TITLE_ALERT);
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void Export()
        {
            try
            {
            // Save File Dialog
            SaveFileDialog saveFileDialog_export = new SaveFileDialog();
            saveFileDialog_export.InitialDirectory = @"C:\";
            saveFileDialog_export.Title = "Export Signed Invoice";
            saveFileDialog_export.CheckFileExists = false;
            saveFileDialog_export.CheckPathExists = false;
            saveFileDialog_export.DefaultExt = "xlsx";
            saveFileDialog_export.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog_export.FilterIndex = 2;
            saveFileDialog_export.RestoreDirectory = true;


            if (saveFileDialog_export.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    string export_excel_path = "";
                    export_excel_path = saveFileDialog_export.FileName;
                    var api = new InvoiceRestApi();

                    //Invoice_Detail_List[] invoice_list = api.GetInvoiceListDetail(InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
                    Invoice_Detail_List[] invoice_list = Search2InvoiceList();
                    DataTable export_table = Excel_Utils.InvoiceListToDataTableDetail(invoice_list);

                    if (File.Exists(export_excel_path))
                    {

                        File.Delete(export_excel_path);

                    }

                    using (ExcelPackage pck = new ExcelPackage(new FileInfo(export_excel_path)))
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ChiTiet");

                        var range = ws.Cells["A1"].LoadFromDataTable(export_table, true);
                        
                        //Format Excel Column
                        ws.Cells["O:O"].Style.Numberformat.Format = "DD/MM/YYYY";
                        ws.Cells["W:W"].Style.Numberformat.Format = "#";
                        ws.Cells["Y:Y"].Style.Numberformat.Format = "#";
                        ws.Cells["Z:Z"].Style.Numberformat.Format = "#";
                        ws.Cells["AB:AB"].Style.Numberformat.Format = "#";
                        ws.Cells["AC:AC"].Style.Numberformat.Format = "#";
                        ws.Cells["AJ:AJ"].Style.Numberformat.Format = "#";
                        ws.Cells["AL:AL"].Style.Numberformat.Format = "#";
                        ws.Cells["AN:AN"].Style.Numberformat.Format = "#";
                        ws.Cells["AO:AO"].Style.Numberformat.Format = "#";

                        pck.Save();
                    }
                    System.Windows.MessageBox.Show(VinaInvoice.Common.Message.MSS_ALERT_EXPORT_FILE_SUCCESS);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void ExportNormal()
        {
            try
            {
            // Save File Dialog
            SaveFileDialog saveFileDialog_export = new SaveFileDialog();
            saveFileDialog_export.InitialDirectory = @"C:\";
            saveFileDialog_export.Title = "Export Signed Invoice";
            saveFileDialog_export.CheckFileExists = false;
            saveFileDialog_export.CheckPathExists = false;
            saveFileDialog_export.DefaultExt = "xlsx";
            saveFileDialog_export.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog_export.FilterIndex = 2;
            saveFileDialog_export.RestoreDirectory = true;


            if (saveFileDialog_export.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    string export_excel_path = "";
                    export_excel_path = saveFileDialog_export.FileName;
                    var api = new InvoiceRestApi();

                    //Invoice_Detail_List[] invoice_list = api.GetInvoiceListDetail(InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
                    Invoice_Detail_List[] invoice_list = Search2InvoiceList();
                    DataTable export_table = Excel_Utils.InvoiceListToDataTableNormal(invoice_list);

                    if (File.Exists(export_excel_path))
                    {

                        File.Delete(export_excel_path);

                    }

                    using (ExcelPackage pck = new ExcelPackage(new FileInfo(export_excel_path)))
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ChiTiet");

                        var range = ws.Cells["A1"].LoadFromDataTable(export_table, true);

                        //Format Excel Column
                        ws.Cells["O:O"].Style.Numberformat.Format = "DD/MM/YYYY";
                        ws.Cells["W:W"].Style.Numberformat.Format = "#";
                        ws.Cells["Y:Y"].Style.Numberformat.Format = "#";
                        ws.Cells["Z:Z"].Style.Numberformat.Format = "#";
                        ws.Cells["AB:AB"].Style.Numberformat.Format = "#";
                        ws.Cells["AC:AC"].Style.Numberformat.Format = "#";

                        pck.Save();
                    }
                    System.Windows.MessageBox.Show(VinaInvoice.Common.Message.MSS_ALERT_EXPORT_FILE_SUCCESS);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private Invoice_Detail_List[] Search2InvoiceList()
        {
            List<Invoice_Detail_List> export_invoice_list = new List<Invoice_Detail_List>();
            try
            {
            IEnumerable<Invoice> invoice_list;
            _InvoiceRepository.Page = -1;

            if (Companyforsearching != "")
            {
                invoice_list = _InvoiceRepository.Search(0, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, Companyforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp, true);
            }
            else if (TaxCodeforsearching != "")
            {
                invoice_list = _InvoiceRepository.Search(1, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, TaxCodeforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp, true);
            }
            else if (InvoiceNumberforsearching != "")
            {
                invoice_list = _InvoiceRepository.Search(2, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceNumberforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp, true);
            }
            else if (InvoiceRefforsearching != "")
            {
                invoice_list = _InvoiceRepository.Search(3, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, InvoiceRefforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp, true);
            }
            else if (SerialNameforsearching != "")
            {
                invoice_list = _InvoiceRepository.Search(4, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, SerialNameforsearching, LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp, true);
            }
            else
            {
                invoice_list = _InvoiceRepository.Search(5, InvoiceInvoice_Variable.type_sort, InvoiceInvoice_Variable.sort_method, "", LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp, true);
            }

            if (invoice_list.Count() > 0)
            {
                foreach (Invoice p in invoice_list)
                {
                    Invoice_Detail_List invoice_tmp = new Invoice_Detail_List {
                        id = p.id,
                        b_name = p.b_name,
                        b_company = p.b_company,
                        b_tax_code = p.b_tax_code,
                        b_address = p.b_address,
                        b_email = p.b_email,
                        invoice_form_name = p.invoice_form_name,
                        invoice_serial_name = p.invoice_serial_name,
                        invoice_number = p.invoice_number,
                        invoice_sign_date = int.Parse(p.invoice_sign_date),
                        status = p.status,
                        invoice_ref = p.invoice_ref,
                        vat_rate = p.vat_rate,
                        sub_total = p.sub_total,
                        vat_amount = p.vat_amount,
                        total_amount = p.total_amount,
                        total_discount = p.total_discount,
                        service_charge_percent = p.service_charge_percent,
                        total_service_charge = p.total_service_charge,
                        currency_code = p.currency_code,
                        state_of_bill = p.state_of_bill,
                        original_invoice = p.original_invoice
                    };
                    List<Invoice_Detail_Item_List> item_tmp = new List<Invoice_Detail_Item_List>();

                    if (p.invoice_item_list != null)
                    {
                        foreach (InvoiceItem item in p.invoice_item_list)
                        {
                            item_tmp.Add(new Invoice_Detail_Item_List
                            {
                                item_name = item.item_name,
                                unit_name = item.unit_name,
                                quantity = item.quantity,
                                unit_price = item.unit_price,
                                vat_percentage = item.vat_percentage,
                                item_total_amount_without_vat = item.item_total_amount_without_vat,
                                discount_amount = item.discount_amount,
                                vat_amount = item.vat_amount,
                                total_amount = item.total_amount
                            });
                        };
                    }
                    invoice_tmp.invoice_item_list = item_tmp.ToArray();
                    export_invoice_list.Add((Invoice_Detail_List) invoice_tmp.Clone());
                }
            }

            _InvoiceRepository.Page = 1;
            LoadListInvoice();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
            return export_invoice_list.ToArray();
        }

        public ObservableCollection<CBMethod> SortLists
        {
            get { return _sortList; }
            set { _sortList = value; }
        }

        private void getSortList()
        {
            SortLists = new ObservableCollection<CBMethod>() {
                 new CBMethod(){Name="Ký hiệu hóa đơn"}
                ,new CBMethod(){Name="Số hóa đơn"}
                ,new CBMethod(){Name="Mã số thuế"}
                ,new CBMethod(){Name="Công ty"}
                ,new CBMethod(){Name="Số chứng từ"}
                ,new CBMethod(){Name="Thời gian"}
            };
        }

       
    }
}
