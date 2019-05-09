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
	public class ProductRepository : Repository<Product>, IProductRepository, IDisposable
	{
		public void Dispose()
		{
			this.Dispose();
		}
	}

    public class ProductUpdateRepository : Repository<Product>, IProductUpdateRepository, IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }
    }
}
