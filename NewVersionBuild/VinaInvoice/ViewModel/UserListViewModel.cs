using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VinaInvoice.Model;
using System.Collections.ObjectModel;
using VinaInvoice.Data.Repository;
using VinaInvoice.Common;

namespace VinaInvoice.ViewModel
{
    public class UserListViewModel : BaseViewModel
    {
        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand SearchButtonCommand { get; set; }
        public ICommand NextButtonCommand { get; set; }
        public ICommand BackButtonCommand { get; set; }

        EnterpriseRepository _EnterpriseRepository = new EnterpriseRepository();


        private ObservableCollection<Member> _List;
        public ObservableCollection<Member> MemberList { get => _List; set { _List = value; OnPropertyChanged(); } }



        private User _SelectedItem;
        public User SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                   // DisplayName = SelectedItem.DisplayName;
                }
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }



        // mọi thứ xử lý sẽ nằm trong này
        public UserListViewModel()
        {
            try
            {
            StatusBarString = Const.STATUS_BAR_STRING;

            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                Isloaded = true;

                this.getUserList();



            }
           );



            //Button Onclick
            SearchButtonCommand = new RelayCommand<object>((p) => { return true; }, (p) => { this.Search(); });
            NextButtonCommand = new RelayCommand<object>((p) => { return true; }, (p) => { this.Next(); });
            SearchButtonCommand = new RelayCommand<object>((p) => { return true; }, (p) => { this.Search(); });
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        // Call API to get UserList
        private void getUserList(){
            try
            {
            //todo
            MemberList = new ObservableCollection<Member>();
     

            IEnumerable<Member> list;

            list = _EnterpriseRepository.Find(product => true);

            if (list != null)
            {
                int count = 0;
                foreach (Member p in list)
                {
                    count++;
                    p.STT = count;
                    MemberList.Add(p);
                }
            }
        }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }
        }

        // Function Search
        private void Search() {
            this.getUserList();

        }

        //Function Next
        private void Next()
        {
           //todo

        }

        //Function Back
        private void Back()
        {
           //todo

        }



    }
}
