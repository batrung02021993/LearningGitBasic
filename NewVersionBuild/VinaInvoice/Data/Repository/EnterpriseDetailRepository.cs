using VinaInvoice.Data.Interface;
using VinaInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;
using VinaInvoice.Data.DataContext;

namespace VinaInvoice.Data.Repository
{
    public class EnterpriseDetailRepository : Repository<EnterpriseDetail>, IEnterpriseDetailRepository, IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }

        public EnterpriseDetail GetDetail()
        {
            var api = new EnterpriseRestApi();
            EnterpriseDetail enterpriseDetail = new EnterpriseDetail();
            enterpriseDetail = api.GetEnterpriseDetail();
            return enterpriseDetail;
        }

        public bool SaveToCache(EnterpriseDetailData enterpriseDetailData)
        {
            try
            {
                ApplicationCache.Add("enterpriseDetail", enterpriseDetailData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveToLocalDb()
        {
            throw new NotImplementedException();
        }        
    }
}