
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.Interface;
using VinaInvoice.Data.RepositoryInterface;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.Repository
{
	public class CustomerRepository : Repository<Customer>, ICustomerRepository, IDisposable
	{
		public void Dispose()
		{
			this.Dispose();
		}
	}

    public class CustomerUpdateRepository : Repository<Customer>, ICustomerUpdateRepository, IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }
    }

    public class CustomerCreateRepository : Repository<Invoice>, ICustomerCreateRepository, IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }
    }
}
