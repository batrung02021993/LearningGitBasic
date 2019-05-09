using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VinaInvoice.Common;
using VinaInvoice.Data.Repository;
using VinaInvoice.Model;

namespace VinaInvoice.ViewModel
{
    public class UserInfoViewModel:BaseViewModel
    {
        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
       
        private ProfileData _profile ;
        public ProfileData Profile
        {
            get => _profile;
            set {
                _profile = value;
                OnPropertyChanged();
            }
        }
        private ProfileRepository profileRepository = new ProfileRepository();
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

        private string _role;
        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged();
            }

        }
        private bool _active = false;
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                OnPropertyChanged();
            }

        }

        private string _secrectKey;
        public string SecrectKey
        {
            get => _secrectKey;
            set
            {
                _secrectKey = value;
                OnPropertyChanged();
            }

        }


        public UserInfoViewModel()
        {
            try
            {
            EnterPrise = (EnterpriseDetailData)ApplicationCache.GetItem("enterpriseDetail");
            Profile = (ProfileData)ApplicationCache.GetItem("profile");
            Role = Profile.getUserRole(Profile.role);

            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { SecrectKey = p.Password; });


            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { if (p != null) p.Close(); });

            SaveCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                SaveInfo();
            });
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        private void SaveInfo()
        {
            try
        {
            if(SecrectKey == EnterPrise.serect_password)
            {
                Profile.IsAdmin = true;
                profileRepository.SaveToCache(Profile);
                MessageBox.Show(Message.MSS_ALERT_PERMISSON_AD_SUCCESS);
            }
            else
            {
                CheckPermissionRepository permissionRepository = new CheckPermissionRepository();
                var rerultCode = permissionRepository.GetCode(SecrectKey);
                if(rerultCode == Const.Code_Successful)
                {
                    Profile.IsSupperAdmin = true;
                    Profile.keySuperAdin = SecrectKey;
                    profileRepository.SaveToCache(Profile);
                    MessageBox.Show(Message.MSS_ALERT_PERMISSON_SA_SUCCESS);
                }
                // todo call API check Super Admin
                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }
    }
}
