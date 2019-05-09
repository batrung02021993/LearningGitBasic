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
using VinaInvoice.Common;

namespace VinaInvoice.ViewModel
{
    class InvoiceDeletedViewModel : BaseViewModel
    {
        InvoiceRepository _InvoiceRepository = new InvoiceRepository();

        public bool Isloaded = false;
        public ICommand CreateCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand InvoiceDoubleClickCommand { get; set; }

        private ObservableCollection<Invoice> _InvoiceDeletedList;
        public ObservableCollection<Invoice> InvoiceDeletedList { get => _InvoiceDeletedList; set { _InvoiceDeletedList = value; OnPropertyChanged(); } }
        public Invoice sInvoice { get; set; }

        private ObservableCollection<CBMethod> _sortList;

        public CBMethod _sSortList = new CBMethod();
        public CBMethod sSortList { get => _sSortList; set { _sSortList = value; OnPropertyChanged(); } }

        public string Companyforsearching { get => InvoiceInvoice_Variable.companyforsearching; set { InvoiceInvoice_Variable.companyforsearching = value; OnPropertyChanged(); } }
        public string TaxCodeforsearching { get => InvoiceInvoice_Variable.taxcodeforsearching; set { InvoiceInvoice_Variable.taxcodeforsearching = value; OnPropertyChanged(); } }
        public string InvoiceNumberforsearching { get => InvoiceInvoice_Variable.invoicenumberforsearching; set { InvoiceInvoice_Variable.invoicenumberforsearching = value; OnPropertyChanged(); } }
        public string InvoiceRefforsearching { get => InvoiceInvoice_Variable.invoicerefforsearching; set { InvoiceInvoice_Variable.invoicerefforsearching = value; OnPropertyChanged(); } }
        public string SerialNameforsearching { get => InvoiceInvoice_Variable.serialnameforsearching; set { InvoiceInvoice_Variable.serialnameforsearching = value; OnPropertyChanged(); } }

        public DateTime Start_date { get => InvoiceInvoice_Variable.start_date; set { InvoiceInvoice_Variable.start_date = value; OnPropertyChanged(); } }
        public DateTime Stop_date { get => InvoiceInvoice_Variable.stop_date; set { InvoiceInvoice_Variable.stop_date = value; OnPropertyChanged(); } }

        private int _page = 1;
        public int Page { get => _page; set { _page = value; OnPropertyChanged(); } }

		int[] LoadStatus = { 3 };
		int[] LoadStateofBill = { 0,1,2,3,4 };

		public InvoiceDeletedViewModel()
        {
            try
            {
                getSortList();

            StatusBarString = Const.STATUS_BAR_STRING;


            SortCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Sort(p); });

            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Search(p); });

            ExportCommand = new RelayCommand<object>((p) => { return true; }, (p) => {  });

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Isloaded = true;
            });
            InvoiceDoubleClickCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
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
                list = _InvoiceRepository.GetList(LoadStatus, LoadStateofBill);

            if (list.Count() > 0)
            {
                InvoiceDeletedList = new ObservableCollection<Invoice>();
                int count = 0;
                foreach (Invoice p in list)
                {
                    count++;
                    p.Stt = count;
                    InvoiceDeletedList.Add(p);
                }
            }
            else
            {
                //MessageBox.Show("You are in the last page !!!", "VinaInvoice notify:");
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
                var response = api.GetInvoiceDetail(sInvoice.id);

                if (response.code != Const.Code_Successful) MessageBox.Show(response.message);

                InvoiceInfoWindow wd = new InvoiceInfoWindow(response.data); wd.ShowDialog();
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
            InvoiceDeletedList = new ObservableCollection<Invoice>();

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
                    InvoiceDeletedList.Add(p);
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
            InvoiceDeletedList = new ObservableCollection<Invoice>();

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
                list = _InvoiceRepository.GetList(LoadStatus, LoadStateofBill, InvoiceInvoice_Variable.start_date_timestamp, InvoiceInvoice_Variable.stop_date_timestamp);
            }

            if (list.Count() > 0)
            {
                int count = 0;
                foreach (Invoice p in list)
                {
                    count++;
                    p.Stt = count;
                    InvoiceDeletedList.Add(p);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Không tìm thấy hóa đơn bạn muốn", Message.MSS_DIALOG_TITLE_ALERT);
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
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
