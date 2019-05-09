using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.Interface;
using VinaInvoice.Model;

namespace VinaInvoice.Data.RepositoryInterface
{
	public interface IProductRepository : IRepository<Product>
	{
	}

    public interface IProductUpdateRepository : IRepository<Product>
    {
    }
}
