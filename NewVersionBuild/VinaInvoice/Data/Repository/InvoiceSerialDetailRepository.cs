using VinaInvoice.Data.Interface;
using VinaInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;
using VinaInvoice.Data.DataContext;

namespace VinaInvoice.Data.Repository
{
    class InvoiceSerialDetailRepository : Repository<SerialDetail>, IInvoiceSerialDetailRepository, IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }

        public SerialDetail GetDetail(string _id)
        {
            var api = new InvoiceRestApi();
            SerialDetail invoiceSerialDetail = new SerialDetail();
            invoiceSerialDetail = api.GetInvoiceSerialDetail(_id);
            return invoiceSerialDetail;
        }
    }
}
