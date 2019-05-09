
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.Interface;
using VinaInvoice.Model;

namespace VinaInvoice.Data.RepositoryInterface
{
	public interface ICustomerRepository : IRepository<Customer>
	{
	}
    public interface ICustomerUpdateRepository : IRepository<Customer>
    {
    }
    public interface ICustomerCreateRepository : IRepository<Invoice>
    {
    }
}
