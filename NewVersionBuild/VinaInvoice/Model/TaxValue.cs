using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Common;
using VinaInvoice.ViewModel;

namespace VinaInvoice.Model
{
    public class TaxValue : BaseViewModel
    {
        private int _id;
        private int _value;
        private string _name;

        public TaxValue(int dataValue)
        {         
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Value
        {
            get { return _value; }
            set {
                _value = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set {
                _name = value;
            }
        }


    }
    
}
