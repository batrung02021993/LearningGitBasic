using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using VinaInvoice.Common;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Model;

namespace VinaInvoice.ViewModel
{
	public class B26ViewModel : BaseViewModel
	{
		#region Comand
		public ICommand CloseCommand { get; set; }
		public ICommand ExportB26XMLCommand { get; set; }
		public ICommand View { get; set; }
        public ICommand ReportPeriodCommand { get; set; }

        

        #endregion

        #region Private Object for internal process
        B26GetlistResponse GetlistResponse;
		private int _periodType = 1;
		private int _periodValue = 1;
		public DuringTimePeriod PeriodReport;

        private EnterpriseDetailData _enterpriseData;
        public EnterpriseDetailData EnterpriseData { get => _enterpriseData; set { _enterpriseData = value; OnPropertyChanged(); } }


        public ObservableCollection<int> _yearList;
        public ObservableCollection<int> YearList { get => _yearList; set { _yearList = value; OnPropertyChanged(); } }
        private int _selectedYear = 2019;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if(value != _selectedYear)
                {
                    _selectedYear = value;
                    TextReportLine1 = GetTexReport(_selectedMonth, _selectedYear);
                    GetB26List();
					GetTimePeriod();
					OnPropertyChanged();
                }
            
            }
        }


        public ObservableCollection<int> _monthList;
        public ObservableCollection<int> MonthList { get => _monthList; set { _monthList = value; OnPropertyChanged(); } }
        private int _selectedMonth = 1;
        public int SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (value != _selectedMonth)
                {
                    _selectedMonth = value;
                    TextReportLine1 = GetTexReport(_selectedMonth, _selectedYear);
                    GetB26List();
					GetTimePeriod();
					OnPropertyChanged();
                }
            }
        }

        public string _period = "Tháng:";
        public string Period { get => _period; set { _period = value; OnPropertyChanged(); } }

        public string _textReportLine1 = "Tháng:1 năm 2019";
        public string TextReportLine1 { get => _textReportLine1; set { _textReportLine1 = value; OnPropertyChanged(); } }
        
        public bool _isCheckBoxPeriod;
        public bool IsCheckBoxPeriod { get => _isCheckBoxPeriod; set { _isCheckBoxPeriod = value; OnPropertyChanged(); } }


        public HSoThueDTuHSoKhaiThueCTieuTKhaiChinh BC26Data = new HSoThueDTuHSoKhaiThueCTieuTKhaiChinh();

        public ObservableCollection<HSoThueDTuHSoKhaiThueCTieuTKhaiChinhChiTiet> _hoadonFieldList;
        public ObservableCollection<HSoThueDTuHSoKhaiThueCTieuTKhaiChinhChiTiet> HoadonFieldList { get => _hoadonFieldList; set { _hoadonFieldList = value; OnPropertyChanged(); } }



        #endregion

        #region Binding Object
        public int PeriodType { get => _periodType; set => _periodType = value; }
		public int PeriodValue { get => _periodValue; set => _periodValue = value; }

		private string _Today = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
		public string Today
		{
			get => _Today;
			set
			{
				if (value != _Today)
				{
					_Today = value;				
					OnPropertyChanged();
				}

			}
		}

		private string _TimePeriod = "";
		public string TimePeriod
		{
			get => _TimePeriod;
			set
			{
				if (value != _TimePeriod)
				{
					_TimePeriod = value;
					OnPropertyChanged();
				}

			}
		}
		#endregion

		public B26ViewModel()
		{
            try
            {
                Init();

            ReportPeriodCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (IsCheckBoxPeriod)
                {
                    Period = "Quý:";
                    MonthList = Const.GetPeriod();
                    TextReportLine1 = GetTexReport(SelectedMonth, SelectedYear);

				

				}
                else
                {
                    Period = "Tháng:";
                    MonthList = Const.GetMonth();
                    TextReportLine1 = GetTexReport(SelectedMonth,SelectedYear);

				
				}

				GetTimePeriod();

			});

            ExportB26XMLCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                string message = "Dữ liệu sau khi in sẽ không thể chỉnh sửa! Bạn có chắc chắn muốn in không?";
                string title = Common.Message.MSS_DIALOG_TITLE_ALERT;
                System.Windows.Forms.MessageBoxButtons buttons = System.Windows.Forms.MessageBoxButtons.YesNo;
                System.Windows.Forms.MessageBoxIcon icon = MessageBoxIcon.Warning;
                System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(message, title, buttons, icon);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    ExportB26XML();
                } 
                else
                {
                    return;
                }
            });

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            }
        );
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private string GetTexReport(int month, int year)
        {
            string result = "";
            try
            {
                if (IsCheckBoxPeriod)
                {
                    result = "Quý:" + month + " năm " + year;
                }
                else
                {

                result = "Tháng:"+ month+" năm "+ year;

            }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
            return result;
        }

		private void GetTimePeriod()
		{
            try
            {
                if (IsCheckBoxPeriod)
                {


				var temp = DateTimeConvert.GetQuarterTimeStamp(SelectedYear, SelectedMonth);
				if (temp == null) return;

                DateTime endTimeShow = temp.EndTimeDT.AddDays(-1);
                TimePeriod = "(Từ " + temp.StartTimeDT.ToString("dd/MM/yyyy") + " đến " + endTimeShow.ToString("dd/MM/yyyy") + ")";

			}
			else
			{
				

				var temp = DateTimeConvert.GetMonthTimeStamp(SelectedYear, SelectedMonth);
				if (temp == null) return;
                DateTime endTimeShow = temp.EndTimeDT.AddDays(-1);
                TimePeriod = "(Từ " + temp.StartTimeDT.ToString("dd/MM/yyyy") + " đến " + endTimeShow.ToString("dd/MM/yyyy") + ")";
			}
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}
		public void Init()
		{
            try
            {
                StatusBarString = Const.STATUS_BAR_STRING;
                EnterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");

            HoadonFieldList = new ObservableCollection<HSoThueDTuHSoKhaiThueCTieuTKhaiChinhChiTiet>();
            YearList = Const.GetYear();
            MonthList = Const.GetMonth();
            GetB26List();
            //ExportB26XML();
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        public void GetB26List()
		{
            try
            {
                B26RestApi b26RestApi = new B26RestApi();
                if (IsCheckBoxPeriod)
                {
                    PeriodReport = DateTimeConvert.GetQuarterTimeStamp(SelectedYear, SelectedMonth);
                }
                else
                {
                    PeriodReport = DateTimeConvert.GetMonthTimeStamp(SelectedYear, SelectedMonth);
                }

			if (PeriodReport == null) return;

			var requestListB26 = new B26GetlistRequest
			{
				start_time = PeriodReport.StartTime,
				stop_time = PeriodReport.EndTime
			};

			 GetlistResponse = b26RestApi.GetB26List(requestListB26);
            if(GetlistResponse.code == 50000)
            {
                BC26Data = GetlistResponse.data;
                HoadonFieldList = new ObservableCollection<HSoThueDTuHSoKhaiThueCTieuTKhaiChinhChiTiet>();

                if (BC26Data != null && BC26Data.HoaDon!= null) { 
                    var stt = 1;
                    foreach (HSoThueDTuHSoKhaiThueCTieuTKhaiChinhChiTiet item in BC26Data.HoaDon)
                    {
                        item.Stt = stt;    
                        HoadonFieldList.Add(item);
                        stt++;
                    }

                }
                 
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
		}

        public void ExportB26XML()
        {
            // Save File Dialog
            SaveFileDialog saveFileDialog_export = new SaveFileDialog();
            saveFileDialog_export.InitialDirectory = @"C:\";
            saveFileDialog_export.Title = "Export BC26 file";
            saveFileDialog_export.CheckFileExists = false;
            saveFileDialog_export.CheckPathExists = false;
            saveFileDialog_export.DefaultExt = "xml";
            saveFileDialog_export.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog_export.FilterIndex = 2;
            saveFileDialog_export.RestoreDirectory = true;


            if (saveFileDialog_export.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    string export_excel_path = "";
                    export_excel_path = saveFileDialog_export.FileName;

                    if (IsCheckBoxPeriod)
                    {
                        PeriodType = 1;
                    }
                    else
                    {
                        PeriodType = 0;
                    }
                    var enterpriseData = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");
                    B26RestApi b26RestApi = new B26RestApi();
                    var ReportResponse = b26RestApi.ReportB26(new ReportBC26Request
                    {
                        start_time = PeriodReport.StartTime,
                        stop_time = PeriodReport.EndTime,
                        type_bc = PeriodType,
                        type_value = SelectedMonth
                    });
                    if (ReportResponse.code != 50000)
                    {
                        System.Windows.MessageBox.Show(ReportResponse.message, "Vina invoice thông báo:");
                        return;
                    }

                    BC26Serialization bC26Serialization = new BC26Serialization();
                    int count = 0;

                    if (GetlistResponse.data.HoaDon != null) foreach (var item in GetlistResponse.data.HoaDon)
                        {
                            count++;
                            item.id = "ID_" + count.ToString();
                        }

                    GetlistResponse.data.ngayBCao = DateTime.Now;
                    bC26Serialization.HSoThueDTu.HSoKhaiThue.CTieuTKhaiChinh = GetlistResponse.data;

                    string kieuky = "Q";
                    string kykhai = "1/2019";
                    bC26Serialization.HSoThueDTu.HSoKhaiThue.TTinChung.TTinTKhaiThue.TKhaiThue.KyKKhaiThue =
                        new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueKyKKhaiThue()
                        {
                            kieuKy = kieuky,
                            kyKKhai = kykhai,
                            kyKKhaiTuNgay = PeriodReport.StartTimeDT.ToString("dd/MM/yyyy"),
                            kyKKhaiDenNgay = PeriodReport.EndTimeDT.ToString("dd/MM/yyyy"),
                            kyKKhaiDenThang = "",// not implement
                            kyKKhaiTuThang = "" //not implement
                        };
                    bC26Serialization.HSoThueDTu.HSoKhaiThue.TTinChung.TTinTKhaiThue.TKhaiThue.ngayLapTKhai = DateTime.Now;
                    bC26Serialization.HSoThueDTu.HSoKhaiThue.TTinChung.TTinTKhaiThue.TKhaiThue.nguoiKy = enterpriseData.manage_by;
                    bC26Serialization.HSoThueDTu.HSoKhaiThue.TTinChung.TTinTKhaiThue.TKhaiThue.ngayKy = DateTime.Now;
                    bC26Serialization.HSoThueDTu.HSoKhaiThue.TTinChung.TTinTKhaiThue.NNT = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueNNT()
                    {
                        dchiNNT = "",
                        dthoaiNNT = "",
                        emailNNT = "",
                        faxNNT = "",
                        maHuyenNNT = "",
                        maTinhNNT = "",
                        mst = enterpriseData.tax_code,
                        phuongXa = "",
                        tenHuyenNNT = "",
                        tenNNT = "",
                        tenTinhNNT = ""

                    };

                    bC26Serialization.ToHSoThueDTuXml(export_excel_path);
                    System.Windows.MessageBox.Show(ReportResponse.message, "Vina invoice thông báo:");
                }
                catch(Exception e)
                {
                    InvoiceErrorClient.invoiceErrorClient(e, true);
                }

            }
        

           
		}

	}


}