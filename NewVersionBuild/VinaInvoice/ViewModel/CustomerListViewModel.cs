using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VinaInvoice.Common;
using VinaInvoice.Data;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.ViewModel
{
    public class CustomerListViewModel : BaseViewModel
    {
        CustomerRepository _Repository = new CustomerRepository();

        public bool Isloaded = false;
        public ICommand CreateWindowCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand ChooseAll { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ImportCustomerExcelCommand{ get; set; }
        public ICommand CustomerDoubleClickCommand { get; set; }

        private ObservableCollection<Customer> _List;
        public ObservableCollection<Customer> DataList { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<SortMethod> _sortList;
        private SortMethod _sSortList;

        private Customer _CustomerItemSeleced = new Customer();
        public Customer CustomerItemSeleced { get { return _CustomerItemSeleced; } set { _CustomerItemSeleced = value; } }

        private string _nameforsearching = "";
        public string Nameforsearching { get => _nameforsearching; set { _nameforsearching = value; OnPropertyChanged(); } }

        private string _Codeforsearching = "";
        public string Codeforsearching { get => _Codeforsearching; set { _Codeforsearching = value; OnPropertyChanged(); } }

        private string _customerNameForResearch = "";
        public string CustomerNameForResearch { get => _customerNameForResearch; set { _customerNameForResearch = value; OnPropertyChanged(); } }

         

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

        public CustomerListViewModel()
        {
            try
            {
                StatusBarString = Const.STATUS_BAR_STRING;

            getSortList();

            ImportCustomerExcelCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                Excel_Utils.CustomerImportExcel();
                LoadList(Page);
            });

            LoadList(Page);

            CreateWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CustomerCreateWindow wd = new CustomerCreateWindow(this); wd.ShowDialog(); });

            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                FindList(Page);
            }
            );

            ChooseAll = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ChooseAllCustomer();
            }
            );

            NextPageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Page++;
                LoadList(Page);
            }
            );

            PreviousPageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
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

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            }
            );

            DeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                DeleteListCustomer();
            }
            );
            CustomerDoubleClickCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                CustomerDoubleClick();
            });
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
                if (DataList != null)
                {
                    List<Customer> tempProduct = new List<Customer>();
                    foreach (Customer p in DataList)
                    {
                        tempProduct.Add(p);
                    }

                DataList = new ObservableCollection<Customer>();
                foreach (Customer customer in tempProduct)
                {
                    DataList.Add(customer);
                }
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void ChooseAllCustomer()
        {
            try
            {
                if (IsCheckAll)
                {
                    if (DataList != null)
                    {
                        foreach (Customer p in DataList)
                        {
                            p.IsSelected = true;
                        }
                    }
                }
                else
                {
                    if (DataList != null)
                    {
                        foreach (Customer p in DataList)
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
        private void FindList(int page)
        {
            try
            {
                DataList = new ObservableCollection<Customer>();

            IEnumerable<Customer> list;

            if (Nameforsearching != "" && Codeforsearching != "")
            {
                //list = _Repository.Find(product => product.CompanyName == Nameforsearching && product.CompanyTaxCode == Codeforsearching);
                list = _Repository.Search<Customer>(0, 0, 0, Codeforsearching);
            }
            else if (Nameforsearching != "")
            {
                //list = _Repository.Find(product => product.CompanyName == Nameforsearching);
                list = _Repository.Search<Customer>(2, 0, 0, Nameforsearching);
            }
            else if (Codeforsearching != "")
            {
                //list = _Repository.Find(product => product.CompanyTaxCode == Codeforsearching);
                list = _Repository.Search<Customer>(0, 0, 0, Codeforsearching);
            }
            else if (CustomerNameForResearch != "")
            {
                //list = _Repository.Find(product => product.CompanyTaxCode == Codeforsearching);
                list = _Repository.Search<Customer>(1, 0, 0, CustomerNameForResearch);
            }
                else
            {
                //list = _Repository.Find(product => true);
                LoadList(Page);
                return;
            }

            if (list != null)
            {
                int count = 0;
                foreach (Customer p in list)
                {
                    count++;
                    p.STT = count;
                    DataList.Add(p);
                }
            }
            else
            {
               //if Page--;
               // MessageBox.Show("You are in the last page !!!", Message.MSS_DIALOG_TITLE_ALERT);
                }
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
                IEnumerable<Customer> list;
                DataList = new ObservableCollection<Customer>();
                _Repository.Page = page;
                list = _Repository.GetList();

            if (list.Count() > 0)
            {
                int count = 0;
                foreach (Customer p in list)
                {
                    count++;
                    p.STT = count;
                    DataList.Add(p);
                }
            }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void DeleteListCustomer()
        {
            try
            {
                if (DataList != null)
                {
                    _Repository.Delete(DataList);
                    MessageBox.Show("Xóa thành công");

                LoadList(Page);
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void getSortList()
        {
            try
            {
            sSortList = new SortMethod() { Name = "Ký hiệu hóa đơn" };

            SortLists = new ObservableCollection<SortMethod>() {
                 new SortMethod(){Name="Tên công ty"}
                ,new SortMethod(){Name="Mã số thuế"}
            };
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
        public SortMethod sSortList
        {
            get { return _sSortList; }
            set { _sSortList = value; OnPropertyChanged("sSortList"); }
        }

        private void CustomerDoubleClick()
        {
            try
            {
                CustomerEditWindow wd = new CustomerEditWindow(CustomerItemSeleced);
                wd.ShowDialog();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
    }
}
