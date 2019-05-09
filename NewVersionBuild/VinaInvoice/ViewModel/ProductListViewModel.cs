//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TuesPechkin;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;
using System.Net.Mail;

using System.IO;
using System.Drawing.Printing;
using System;
using VinaInvoice.Common;
using System.Data;
using VinaInvoice.Model.JsonObjectModel;
using OfficeOpenXml;
using VinaInvoice.Data.DataContext;

namespace VinaInvoice.ViewModel
{
    public class ProductListViewModel : BaseViewModel
    {
        ProductRepository _ProductRepository = new ProductRepository();

        public bool Isloaded = false;
        public ICommand CreateWindowCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand ChooseAll { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ImportProductExcelCommand { get; set; }
        public ICommand ProductDoubleClickCommand { get; set; }

        private ObservableCollection<Product> _ProductList;
        public ObservableCollection<Product> ProductList { get => _ProductList; set { _ProductList = value; OnPropertyChanged(); } }
        private ObservableCollection<SortMethod> _sortList;
        private SortMethod _sSortList;

        private Product _ProductItemSeleced = new Product();
        public Product ProductItemSeleced { get { return _ProductItemSeleced; } set { _ProductItemSeleced = value; } }

        private bool _isCheckAll = false;
        public bool IsCheckAll {
            get => _isCheckAll;
            set {
                _isCheckAll = value;
                OnPropertyChanged();
            }
        }

        private string _nameforsearching = "";
        public string Nameforsearching { get => _nameforsearching; set { _nameforsearching = value; OnPropertyChanged(); } }

        private string _SKUforsearching = "";
        public string SKUforsearching { get => _SKUforsearching; set { _SKUforsearching = value; OnPropertyChanged(); } }

        private int _page = 1;
        public int Page { get => _page; set { _page = value; OnPropertyChanged(); } }

        public ProductListViewModel()
        {
            try
            {
            StatusBarString = Const.STATUS_BAR_STRING;

            getSortList();

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Isloaded = true;
            });

            ImportProductExcelCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                Excel_Utils.ProductImportExcel();
                LoadListProduct(Page);
            });

            LoadListProduct(Page);

			CreateWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ProductCreateWindow wd = new ProductCreateWindow(this); wd.ShowDialog(); });

            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                FindListProduct(Page);
            });

            ChooseAll = new RelayCommand<object>((p) => { return true; }, (p) => {
                ChooseAllProduct();                      
            });

            NextPageCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                Page++;
                LoadListProduct(Page);
            });

            PreviousPageCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                if (Page > 1)
                {
                    Page--;
                    LoadListProduct(Page);
                }
                else
                {
                   //todo disable button Next
                }
            });

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
                p.Close();
            });

            DeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DeleteListProduct();
                LoadListProduct(Page);
            });
            ProductDoubleClickCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                ProductDoubleClick();
            });
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void RefreshListView()
        {
            try
        {
            if (ProductList != null)
            {
                List<Product> tempProduct = new List<Product>();
                foreach (Product p in ProductList)
                {
                    tempProduct.Add(p);
                }

                ProductList = new ObservableCollection<Product>();
                foreach (Product product in tempProduct)
                {
                    ProductList.Add(product);
                }
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void ChooseAllProduct()
        {
            try
        {
            if (IsCheckAll)
            {
                if (ProductList != null)
                {
                    foreach (Product p in ProductList)
                    {
                        p.IsSelected = true;
                    }
                }
            }
            else {
                if (ProductList != null)
                {
                    foreach (Product p in ProductList)
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

        public void LoadListProduct(int page)
		{
            try
		{						
			IEnumerable<Product> list ;
			_ProductRepository.Page = page;
			list = _ProductRepository.GetList();
            ProductList = new ObservableCollection<Product>();

            if (list.Count() > 0)
			{				
				int count = 0;
				foreach(Product p in list)
				{
					count++;
					p.Stt = count;
					ProductList.Add(p);
				}
			}
			else
			{
                //todo
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
                 new SortMethod(){Name="Mã hàng hóa"}
                ,new SortMethod(){Name="Đơn giá"}
                ,new SortMethod(){Name="Số lượng"}

            };
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void FindListProduct(int page)
		{
            try
            {
			_ProductRepository.Page = page;        

            ProductList = new ObservableCollection<Product>();
			IEnumerable<Product> list;

			if (Nameforsearching != "" && SKUforsearching != "")
			{
                //list = _ProductRepository.Find(product => product.Name == Nameforsearching && product.Id == SKUforsearching);
                list = _ProductRepository.Search<Product>(1, 0, 0, SKUforsearching);
            }
			else if (Nameforsearching != "")
			{
                //list = _ProductRepository.Find(product => product.Name == Nameforsearching);
                list = _ProductRepository.Search<Product>(0, 0, 0, Nameforsearching);
            }
			else if (SKUforsearching != "")
			{
                //list = _ProductRepository.Find(product => product.Id == SKUforsearching);
                list = _ProductRepository.Search<Product>(1, 0, 0, SKUforsearching);
            }
			else
			{
                //list = _ProductRepository.Find(product => true);
                LoadListProduct(Page);
                return;
            }

            //list = _ProductRepository.Search("ItemName", "San Pham E");

            //list = _ProductRepository.Search("ItemCode", "SKU");

            if (list != null)
			{
				int count = 0;
				foreach (Product p in list)
				{
					count++;
					p.Stt = count;
					ProductList.Add(p);                    
                }                
            }
			else
			{
				Page--;
				MessageBox.Show("You are in the last page !!!", Message.MSS_DIALOG_TITLE_ALERT);
			}
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void DeleteListProduct()
        {
            try
        {
            if (ProductList != null)
            {
                _ProductRepository.Delete(ProductList);
                MessageBox.Show("Xóa thành công");
                LoadListProduct(Page);
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
        public SortMethod sSortList
        {
            get { return _sSortList; }
            set { _sSortList = value; OnPropertyChanged("sSortList"); }
        }


        private void ProductDoubleClick()
        {
            try
            {
                ProductEditWindow wd = new ProductEditWindow(ProductItemSeleced);
                wd.ShowDialog();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }

        }
    }
}

   