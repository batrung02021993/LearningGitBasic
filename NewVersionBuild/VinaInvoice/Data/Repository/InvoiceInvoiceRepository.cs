using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.Interface;
using VinaInvoice.Data.RepositoryInterface;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;
using VinaInvoice.Data.DataContext;

namespace VinaInvoice.Data.Repository
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository, IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }
    }
}
