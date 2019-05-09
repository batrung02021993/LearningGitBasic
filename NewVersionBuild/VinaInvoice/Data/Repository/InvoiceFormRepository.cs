using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.RepositoryInterface;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.Repository
{
	public class InvoiceFormRepository : Repository<Form>, IInvoiceFormRepository, IDisposable
	{
		public void Dispose()
		{
			this.Dispose();
		}
	}
}
