using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.Interface;
using VinaInvoice.Model.JsonObjectModel;
using VinaInvoice.Model;

namespace VinaInvoice.Data.Interface
{
    interface IInvoiceRepository : IRepository<Invoice>
    {
    }
}
