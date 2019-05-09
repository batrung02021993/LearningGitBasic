using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.RepositoryInterface;
using VinaInvoice.Model;

namespace VinaInvoice.Data.Repository
{

    public class UserRepository : Repository<User>, IUserRespository, IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }  
    
    }
}
