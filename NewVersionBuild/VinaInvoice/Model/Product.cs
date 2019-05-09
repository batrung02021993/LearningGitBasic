using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Common;
using VinaInvoice.ViewModel;

namespace VinaInvoice.Model
{
	public class ProductUI : BaseViewModel, System.ComponentModel.INotifyDataErrorInfo
	{
		private int _stt;
        private bool _isSelected = false;
		private string _id = "";
        private string _item_code = "";
        private string _name = "";
		private string _itemType = "";
		private string _unitName;
		private double _quantity = 0;
		private double _unitPrice = 0;
		private double _totalAmount = 0;
		private int _vatPercentage;
		private double _vatAmount = 0;
		private double _subTotal = 0;
        private int _indexTax = 3;
        private bool _isTaxEnable = true;
        private int _decimalNum;
        private string _totalAmountCheck = "";
        private Product _productSellected;
        public bool _isReverseCal;


        public bool IsReverseCal
        {
            get => _isReverseCal;
            set
            {
                if (_isReverseCal != value)
                {
                    _isReverseCal = value;
                    OnPropertyChanged();
                }
            }
        }

        public Product ProductSellected
        {
            get { return _productSellected; }
            set {
                if (value != null)
                {
                
                    _productSellected = value;            
                    Id = _productSellected.id;
                    ItemCode = _productSellected.item_code;
                    Name = _productSellected.item_name;
                    UnitName = _productSellected.unit_name;
                    UnitPrice = _productSellected.unit_price;
                    OnPropertyChanged();
                }
            }
             
        }

        public string IdDatabase { get; set; } = ""; // for delete in database

        public ProductUI()
		{
			_vatPercentage = 10;
            IsSelected = false;
            _decimalNum = (int)Int32.Parse(ConverterVariable.NUMBER_BEHIND_DOT);
        }

        public int IndexTax
        {
            get { return _indexTax; }
            set { _indexTax = value; OnPropertyChanged(); }
        }

        public bool IsTaxEnable
        {
            get { return _isTaxEnable; }
            set { _isTaxEnable = value; OnPropertyChanged(); }
        }

        public int Stt
		{
			get { return _stt; }
			set { _stt = value; OnPropertyChanged(); }
		}

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public string Id
		{
			get { return _id; }
			set { _id = value; OnPropertyChanged(); }
		}

        public string ItemCode
        {
            get { return _item_code; }
            set { _item_code = value; OnPropertyChanged(); }
        }

        public string Name
		{
			get { return _name; }
			set { _name = value; OnPropertyChanged(); }
		}

		public string ItemType
		{
			get { return _itemType; }
			set { _itemType = value; OnPropertyChanged(); }
		}

		public string UnitName
		{
			get { return _unitName; }
			set { _unitName = value; OnPropertyChanged(); }
		}
        public string TotalAmountCheck
        {
            get { return _totalAmountCheck; }
            set { _totalAmountCheck = value; OnPropertyChanged(); }
        }

        #region new function

        //Số lượng
        public Double Quantity
        {
            get { return _quantity; }
            set
            {
                if (IsPositivedoubleValid("Quantity", value, "Số Lượng Sản Phẩm") && _quantity != value)
                {
                    _quantity = value;

                    if (IsReverseCal)
                    {
                        // Tính ngược
                        UnitPrice = Math.Round(SubTotal / Quantity, _decimalNum);

                    }
                    else
                    {
                        //Tính xuôi
                        UpdateSubTotal();
                    }
                    OnPropertyChanged();

                }
            }
        }

        //Đơn giá
        public double UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                if (IsPositivedoubleValid("UnitPrice", value, "Đơn Giá Sản Phẩm") && _quantity != value)
                {
                    _unitPrice = value;
                    if (IsReverseCal)
                    {
                        // Tính ngược

                    }
                    else
                    {
                        //Tính xuôi
                        UpdateSubTotal();
                    }
                    OnPropertyChanged();
                   
                }


            }
        }

        //Thành tiền
        public double SubTotal
        {
            get { return _subTotal; }
            set
            {

                if ( _subTotal != value)
                {
                    _subTotal = value;
                    if (IsReverseCal)
                    {
                        // Tính ngược

                    }
                    else
                    {
                        //Tính xuôi                
                        UpdateVatAmount();
                        UpdateTotalAmountFromTax();
                    }
                    OnPropertyChanged();
                }

            }

        }

        //% Thuế
        public int VatPercentage
        {
            get { return _vatPercentage; }
            set
            {
                if (_vatPercentage != value)
                {
                    _vatPercentage = value;
                    if (IsReverseCal)
                    {
                        // Tính ngược
                        ReverseUpdate();
                    }
                    else
                    {
                        //Tính xuôi
                        UpdateVatAmount();
                        UpdateTotalAmountFromTax();
                    }
                    OnPropertyChanged();
                }

            }
        }

        //Tiền thuế
        public double VatAmount
        {
            get { return _vatAmount; }
            set
            {
                if (_vatAmount != value)
                {
                    _vatAmount = value;
                    if (IsReverseCal)
                    {
                        // Tính ngược

                    }
                    else
                    {
                        //Tính xuôi
              
                    }
                    OnPropertyChanged();
                }
            }
        }

        //Tổng tiền
        public double TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                if (_totalAmount != value)
                {
                    _totalAmount = value;
                    if (IsReverseCal)
                    {
                        // Tính ngược
                        ReverseUpdate();

                    }
                    else
                    {
                        //Tính xuôi
                        double a = _vatAmount;
                        if ((_vatAmount + _subTotal) != _totalAmount && _totalAmount > 0 && (_totalAmount - _subTotal > 0) && _vatAmount > 0)
                        {
                            VatAmount = _totalAmount - _subTotal;
                        }
                    }
                    OnPropertyChanged();
                }

            }
        }


        #region calculate

        private void ReverseUpdate()
        {
            int TaxRate;
            if (VatPercentage <= 0)
            {
                TaxRate = 0;
            }
            else
            {
                TaxRate = VatPercentage;
            }

            SubTotal = Math.Round(_totalAmount * 100 / (100 + TaxRate), _decimalNum);
            UnitPrice = Math.Round(SubTotal / Quantity, _decimalNum);
            VatAmount = TotalAmount - SubTotal;

            OnPropertyChanged();
        }

        private void UpdateVatAmount()
        {
            int TaxRate;
            if (VatPercentage <= 0)
            {
                TaxRate = 0;
            }
            else
            {
                TaxRate = VatPercentage;
            }
            VatAmount = Math.Round(_subTotal * TaxRate / 100, _decimalNum);


            OnPropertyChanged();
        }

        private void UpdateSubTotal()
        {
            SubTotal = _unitPrice * (double)_quantity;
            OnPropertyChanged();
        }

        private void UpdateTotalAmountFromTax()
        {
            int TaxRate;
            if (VatPercentage <= 0)
            {
                TaxRate = 0;
            }
            else
            {
                TaxRate = VatPercentage;
            }
            double temp = Math.Round(_subTotal * TaxRate / 100, _decimalNum);
            TotalAmount = _subTotal + temp;

            if ((_vatAmount + _subTotal) != _totalAmount && _totalAmount > 0 && (_totalAmount - _subTotal > 0) && _vatAmount > 0)
            {
                VatAmount = _totalAmount - _subTotal;
            }

            OnPropertyChanged();
        }

        #endregion calculate


        #endregion new function

        #region old function
        //      public Double Quantity
        //{
        //	get { return _quantity; }
        //	set
        //	{
        //		if (IsPositivedoubleValid("Quantity", value, "Số Lượng Sản Phẩm") && _quantity != value)
        //              {
        //                  _quantity = value;
        //              }

        //              UpdateSubTotal();
        //          }
        //      }

        //public double UnitPrice
        //{
        //	get { return _unitPrice; }
        //	set
        //	{
        //              if (_unitPrice == value)
        //              {
        //                  return;
        //              }
        //              else
        //              {
        //                  if (IsPositivedoubleValid("UnitPrice", value, "Đơn Giá Sản Phẩm"))
        //                  {
        //                      _unitPrice = value;
        //                  }
        //                  OnPropertyChanged();
        //                  UpdateSubTotal();

        //              }

        //	}
        //}

        //public double SubTotal
        //{
        //	get { return _subTotal; }
        //	set
        //	{
        //              if (_subTotal == value)
        //              {
        //                  return;
        //              }
        //              else
        //              {
        //                  _subTotal = value;
        //                  OnPropertyChanged();
        //                  UpdateVatAmount();
        //                  UpdateTotalAmountFromTax();

        //              }

        //              //OnPropertyChanged();
        //          }

        //}

        //public int  VatPercentage
        //{
        //	get { return _vatPercentage; }
        //	set
        //	{
        //              if(_vatPercentage == value)
        //              {
        //                  return;
        //              }
        //              else
        //              {
        //                  _vatPercentage = value;
        //                  OnPropertyChanged();

        //                  UpdateVatAmount();
        //                  UpdateTotalAmountFromTax();

        //              }

        //          }
        //}

        //public double VatAmount
        //{
        //	get { return _vatAmount; }
        //	set {
        //              _vatAmount = value;
        //              OnPropertyChanged();
        //              //UpdateTotalAmount();
        //          }
        //}

        //public double TotalAmount
        //{
        //	get { return _totalAmount; }
        //	set
        //	{
        //              if (_totalAmount == value)
        //              {
        //                  return;
        //              }
        //              else
        //              {
        //                  _totalAmount = value;
        //                  OnPropertyChanged();
        //                  //ReverseUpdate();
        //                  double a = _vatAmount;
        //                  if ((_vatAmount + _subTotal) != _totalAmount && _totalAmount > 0 && (_totalAmount - _subTotal > 0) && _vatAmount>0)
        //                  {
        //                      VatAmount = _totalAmount - _subTotal;
        //                  }
        //              }

        //	}

        //}

        //      private void ReverseUpdate()
        //      {
        //          int TaxRate;
        //          if (VatPercentage <= 0)
        //          {
        //              TaxRate = 0;
        //          }
        //          else
        //          {
        //              TaxRate = VatPercentage;
        //          }

        //          UnitPrice = Math.Round( _totalAmount * 100 / ((100 + TaxRate)*_quantity), _decimalNum);

        //          OnPropertyChanged();
        //      }

        //      private void UpdateVatAmount()
        //      {
        //          int TaxRate;
        //          if (VatPercentage <= 0)
        //          {
        //              TaxRate = 0;
        //          }
        //          else
        //          {
        //              TaxRate = VatPercentage;
        //          }
        //          VatAmount = Math.Round(_subTotal * TaxRate / 100, _decimalNum);

        //          if ((_vatAmount + _subTotal) != _totalAmount && _totalAmount > 0 && (_totalAmount - _subTotal > 0) && _vatAmount>0)
        //          {
        //              VatAmount = _totalAmount - _subTotal;
        //          }
        //          OnPropertyChanged();
        //      }

        //      private void UpdateSubTotal()
        //      {

        //          SubTotal = _unitPrice * (double)_quantity;
        //          OnPropertyChanged();
        //      }

        //      private void UpdateTotalAmountFromTax()
        //      {
        //          int TaxRate;
        //          if (VatPercentage <= 0)
        //          {
        //              TaxRate = 0;
        //          }
        //          else
        //          {
        //              TaxRate = VatPercentage;
        //          }
        //          double temp = Math.Round(_subTotal * TaxRate / 100);
        //          TotalAmount = _subTotal + temp;

        //          OnPropertyChanged();
        //      }

        #endregion old function


        #region General Validation	
        private bool IsPositivedoubleValid(string property, double value, string errorSource)
		{
			bool isValid = true;

			if (value < 0)
			{
				AddError(property, Const.NotPositivedouble_ERROR + errorSource, false);
				isValid = false;
			}
			else RemoveError(property, Const.NotPositivedouble_ERROR + errorSource);

			return isValid;
		}

		private Dictionary<String, List<String>> _errors = new Dictionary<string, List<string>>();
		public Dictionary<String, List<String>> errors
		{
			get
			{
				return _errors;
			}
			set
			{
				_errors = value;
			}
		}

		// Adds the specified error to the _errors collection if it is not 
		// already present, inserting it in the first position if isWarning is 
		// false. Raises the _errorsChanged event if the collection changes. 
		public void AddError(string propertyName, string error, bool isWarning)
		{
			if (!_errors.ContainsKey(propertyName))
				_errors[propertyName] = new List<string>();

			if (!_errors[propertyName].Contains(error))
			{
				if (isWarning) _errors[propertyName].Add(error);
				else _errors[propertyName].Insert(0, error);
				RaiseerrorsChanged(propertyName);
			}
		}

		// Removes the specified error from the _errors collection if it is
		// present. Raises the _errorsChanged event if the collection changes.
		public void RemoveError(string propertyName, string error)
		{
			if (_errors.ContainsKey(propertyName) &&
				_errors[propertyName].Contains(error))
			{
				_errors[propertyName].Remove(error);
				if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);
				RaiseerrorsChanged(propertyName);
			}
		}

		public void RaiseerrorsChanged(string propertyName)
		{
			if (ErrorsChanged != null)
				ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
		}
		/// impelent INotifyDataErrorInfo

		public bool HasErrors
		{
			get { return _errors.Count > 0; }
		}


		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors(string propertyName)
		{
			if (String.IsNullOrEmpty(propertyName) ||
				!_errors.ContainsKey(propertyName)) return null;
			return _errors[propertyName];
		}
		#endregion



	}

	public class Product :  System.ComponentModel.INotifyDataErrorInfo
	{

		// Use by UI and Rest API
		public string id { get; set; }

        public string item_code { get; set; }

        public string item_name
		{
			get => _item_name;
			set
			{
				if (isNullValid("item_name", value, "tên sản phẩm") || _item_name != value)
					_item_name = value;

			}
		}
		public string unit_name
		{
			get => _unit_name;
			set
			{
				if (_unit_name != value)
					_unit_name = value;
			}
		}
		public double unit_price
		{
			get => _unit_price;
			set
			{
				if (IsPositivedoubleValid("unit_price", value, "đơn giá") || _unit_price != value)
					_unit_price = value;

			}
		}
		public double quantity
		{
			get => _quantity;
			set
			{
				if (IsPositivedoubleValid("quantity", value, "số lượng") || _quantity != value)
					_quantity = value;

			}
		}
		public double item_total_amount_without_vat { get; set; }
		public int vat_percentage
		{
			get => _vat_percentage;
			set
			{
				//if (IsPositivedoubleValid("vat_percentage", value, "thuế suất") || _vat_percentage != value)
					_vat_percentage = value;

			}
		}
		public double vat_amount { get; set; }
		public string current_code { get; set; }
		public string item_note { get; set; }
		public string item_type { get; set; }
		public int discount_percentage { get; set; }
		public double discount_amount { get; set; }
		public double total_amount { get; set; }
		public string create_by { get; set; }
		public int create_time { get; set; }

		private string _item_name;
		private string _unit_name;
		private double _unit_price;
		private double _quantity;
		private int _vat_percentage;
		//Use by UI		
		public int Stt { get; set; }
		public bool IsSelected { get; set; }
        public string _totalAmountCheck { get; set; }

        public string TotalAmountCheck
        {
            get { return _totalAmountCheck; }
            set { _totalAmountCheck = value; }
        }

		#region General Validation
		public void ValidateFinal()
		{
			isNullValid("item_name", _item_name, "tên sản phẩm");
			//isNullValid("unit_name", _unit_name, "đơn vị tính");
		}
		private bool IsPositivedoubleValid(string property, double value, string errorSource)
		{
			bool isValid = true;

			if (value < 0)
			{
				AddError(property, Const.NotPositivedouble_ERROR + errorSource, false);
				isValid = false;
			}
			else RemoveError(property, Const.NotPositivedouble_ERROR + errorSource);

			return isValid;
		}

		private bool isNullValid(string propertyname, string value, string errorSource)
		{
			bool isValid = true;

			if (value == null)
			{
				AddError(propertyname, Const.Null_code_ERROR + errorSource, false);
				isValid = false;
			}
			else RemoveError(propertyname, Const.Null_code_ERROR + errorSource);

			return isValid;

		}

		private Dictionary<String, List<String>> _errors = new Dictionary<string, List<string>>();


		public Dictionary<String, List<String>> errors
		{
			get
			{
				return _errors;
			}
			set
			{
				_errors = value;
			}
		}

		// Adds the specified error to the _errors collection if it is not 
		// already present, inserting it in the first position if isWarning is 
		// false. Raises the _errorsChanged event if the collection changes. 
		public void AddError(string propertyName, string error, bool isWarning)
		{
			if (!_errors.ContainsKey(propertyName))
				_errors[propertyName] = new List<string>();

			if (!_errors[propertyName].Contains(error))
			{
				if (isWarning) _errors[propertyName].Add(error);
				else _errors[propertyName].Insert(0, error);
				RaiseerrorsChanged(propertyName);
			}
		}

		// Removes the specified error from the _errors collection if it is
		// present. Raises the _errorsChanged event if the collection changes.
		public void RemoveError(string propertyName, string error)
		{
			if (_errors.ContainsKey(propertyName) &&
				_errors[propertyName].Contains(error))
			{
				_errors[propertyName].Remove(error);
				if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);
				RaiseerrorsChanged(propertyName);
			}
		}

		public void RaiseerrorsChanged(string propertyName)
		{
			if (ErrorsChanged != null)
				ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
		}
		/// impelent INotifyDataErrorInfo

		public bool HasErrors
		{
			get { return _errors.Count > 0; }
		}


		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors(string propertyName)
		{
			if (String.IsNullOrEmpty(propertyName) ||
				!_errors.ContainsKey(propertyName)) return null;
			return _errors[propertyName];
		}
		#endregion



	}
}
