using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;
using System.Windows.Controls;
using VinaInvoice.Common;

namespace VinaInvoice.ViewModel
{
    public class ProductCreateViewModel: BaseViewModel
    {
		public ProductListViewModel _parent;
        public string x = "";

		ProductRepository _Repository = new ProductRepository();

		public bool Isloaded = false;
		
		public ICommand SaveCommand { get; set; }
		public ICommand CloseCommand { get; set; }

        private IEnumerable<string> _unitNames;
        public IEnumerable<string> UnitNames { get => _unitNames; set { _unitNames = value; OnPropertyChanged(); } }
        public string SelectedName;

        private string dataValue;
        public string DataValue {
            get { return dataValue; }
            set { dataValue = value; OnPropertyChanged("dataValue"); }
    
        }

        private Product _Product = new Product();
		public Product Product { get => _Product; set { _Product = value; OnPropertyChanged(); } }

        private ObservableCollection<MoneyUnit> _moneyUnit;
        private MoneyUnit _sMoneyUnit;
        private ObservableCollection<TaxValue> _taxUnit;
        private TaxValue _sTaxUnit;


        public ProductCreateViewModel()
		{
            try
            {
            getMoneyUnit();
            getTaxUnit();
            getUnitNamesList();

            SaveCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SaveCommandFunction(); });

			CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {		
				p.Close();
				_parent.LoadListProduct(1);
			}
			 );
		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

		private void SaveCommandFunction()
        {
            try
            {
            //Validate General
			Product.ValidateFinal();
			if (Product.errors.Count > 0)
			{
				var e = Product.errors.FirstOrDefault();
				if (e.Value.Count > 0) MessageBox.Show(e.Value.FirstOrDefault(), "VinaInvoice thông báo: ");
				return;
			}

			MessageBox.Show(_Repository.Create(Product), Message.MSS_DIALOG_TITLE_ALERT);			
		}
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

        private void getUnitNamesList()
        {
            try
            {
                UnitNames = new ObservableCollection<string>() { "m²", "cm²", "dm²", "m³", "cm³", "dm³", "mm³", "Cái", "Gói" };
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void UpdateUnitName(string _value)
        {
            try
            {
                Product.unit_name = _value;
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
        private void getTaxUnit()
        {
            try
        {
            TaxUnits = new ObservableCollection<TaxValue>() {
                  new TaxValue(0)
                 ,new TaxValue(5)
                 ,new TaxValue(10)
                 ,new TaxValue(15)
                 ,new TaxValue(-1)

            };
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void getMoneyUnit()
        {
            try
            {
            MoneyUnits = new ObservableCollection<MoneyUnit>() {
                 new MoneyUnit(){Id=1, Name="VND"}
                ,new MoneyUnit(){Id=2, Name="USD"}
                ,new MoneyUnit(){Id=3, Name="EUR"}
                ,new MoneyUnit(){Id=4, Name="JPY"}};
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
            base.OnPropertyChanged(propertyName);

                if (propertyName == "DataValue")
                {
                    Console.WriteLine("1");
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public ObservableCollection<MoneyUnit> MoneyUnits
        {
            get { return _moneyUnit; }
            set { _moneyUnit = value; }
        }

        public MoneyUnit SMoneyUnit
        {
            get { return _sMoneyUnit; }
            set { _sMoneyUnit = value; Product.current_code = _sMoneyUnit.Name; }
        }

        public ObservableCollection<TaxValue> TaxUnits
        {
            get { return _taxUnit; }
            set { _taxUnit = value; }
        }

        public TaxValue STaxUnit
        {
            get { return _sTaxUnit; }
            set { _sTaxUnit = value; }
        }

    }
}
