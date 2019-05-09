using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VinaInvoice.Common;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.DataContext
{
    public class CheckPermissionApi
    {

        private BaseRestApi _baseRestApi = new BaseRestApi();
        public int CheckSA(string _serect_password)
        {
			try
			{
				CheckPermissonRequest request = new CheckPermissonRequest
				{
					serect_password = _serect_password
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<CheckPermissionResponse, CheckPermissonRequest>(profile.token, request, "user/check_sa_pass");
				CheckPermissionResponse Response = task.GetAwaiter().GetResult();
				return Response.code;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return 0;
			}


			
        }
    }
}
