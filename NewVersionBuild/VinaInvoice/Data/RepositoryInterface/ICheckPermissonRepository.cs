using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.Interface;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.Interface
{
    public interface ICheckPermissonRepository : IRepository<CheckPermissionResponse>
    {
        int GetCode();
    }
}
