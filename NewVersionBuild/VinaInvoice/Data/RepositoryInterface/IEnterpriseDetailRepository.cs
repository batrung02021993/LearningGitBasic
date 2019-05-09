using VinaInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.Interface
{
    public interface IEnterpriseDetailRepository : IRepository<EnterpriseDetail>
    {
        bool SaveToCache(EnterpriseDetailData enterpriseDetailData);
        bool SaveToLocalDb();
        EnterpriseDetail GetDetail();
    }
}
