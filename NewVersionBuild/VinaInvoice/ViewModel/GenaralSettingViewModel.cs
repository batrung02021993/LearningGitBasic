using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VinaInvoice.Model;
using VinaInvoice.Common;
using Newtonsoft.Json;

namespace VinaInvoice.ViewModel
{
    public class GenaralSettingViewModel : BaseViewModel
    {
        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand EditMoneyAmountCheckCommand { get; set; }
        private string Path = AppDomain.CurrentDomain.BaseDirectory;

		private string _num = "0";
        public string num
        {
            get => _num;
            set
            {
                _num = value;
                OnPropertyChanged();
            }
        }
		
		private bool _showCheckBoxMoneyAmount = false;
        public bool ShowCheckBoxMoneyAmount
        {
            get => _showCheckBoxMoneyAmount;
            set
            {
                _showCheckBoxMoneyAmount = value;
                OnPropertyChanged();
            }
        }

        private EnterpriseDetailData _enterPrise;
        public EnterpriseDetailData EnterPrise
        {
            get => _enterPrise;
            set
            {
                _enterPrise = value;
                OnPropertyChanged();
            }
        }

        private bool editMoneyAmountCheck = false;
        public bool EditMoneyAmountCheck
        {
            get => editMoneyAmountCheck;
            set
            {
                editMoneyAmountCheck = value;
                OnPropertyChanged();
            }
        }

        public GenaralSettingViewModel()
        {
            try
            {
                Init();

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { if (p != null) p.Close(); });

            SaveCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                SaveInvoice();
            });

            EditMoneyAmountCheckCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                EditMoneyAmount();
            });
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
        private void Init()
        {
            try
            {
                EnterPrise = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");
                var profile = (ProfileData)ApplicationCache.GetItem("profile");
				// get value decimal from file setting in xml
                num = StringFormat.decimalPlaces.ToString();
				// check show or hidden checkbox total moeny
                if (profile.IsSupperAdmin == true)
                {
                    ShowCheckBoxMoneyAmount = true;
                }
                else
                {
                    ShowCheckBoxMoneyAmount = false;
                }
            EditMoneyAmount();
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
                ConverterVariable.NUMBER_BEHIND_DOT = num;
                StringFormat.decimalPlaces = Int32.Parse(ConverterVariable.NUMBER_BEHIND_DOT.Trim());
                // save num to local 
                string jsonstring = JsonConvert.SerializeObject(num);
                System.IO.File.WriteAllText(Path + "dotnumfloat.txt", jsonstring);

			MessageBox.Show(Message.MSS_SAVE_SETTING_CONFIG_SUCCESS);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void EditMoneyAmount()
        {
            try
            {
                string jsonstring = JsonConvert.SerializeObject(EditMoneyAmountCheck);
                System.IO.File.WriteAllText(Path + "EditMonyAmount.txt", jsonstring);
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
    }
}
