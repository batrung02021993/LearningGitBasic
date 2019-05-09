using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.RepositoryInterface;
using VinaInvoice.Model;
using VinaInvoice.Data.Interface;
using VinaInvoice.Model.JsonObjectModel;
using VinaInvoice.Data.DataContext;

namespace VinaInvoice.Data.Repository
{

    class CheckPermissionRepository : Repository<CheckPermissionResponse> 
    {
        public int GetCode(string serect_password)
        {
            var api = new CheckPermissionApi();

            return  api.CheckSA(serect_password);

        }
    }
   
}
