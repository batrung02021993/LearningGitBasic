using VinaInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;
using System.Windows;
using VinaInvoice.Common;

namespace VinaInvoice.Data.DataContext
{

	public class UserRestApi
	{
		BaseRestApi _baseRestApi = new BaseRestApi();
	
		public Profile GetProfile(object infor)
		{
			try
			{
				var loginInfo = (LoginInfo)infor;
				var task = _baseRestApi.AddAsync<Profile, LoginInfo>(loginInfo, "user/login");
				var loginresponse = task.GetAwaiter().GetResult();
				var profile = loginresponse;
				return profile;
			}
			catch(Exception e)
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}
		
		}


        public IEnumerable<User> GetListUser(int _page, string _role)
        {
			try
			{
				List<User> list = new List<User>();
				var request = new UserListRequest
				{
					role = _role,
					page = _page
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<UserListResponse, UserListRequest>(profile.token, request, "user/list");
				var userListResponse = task.GetAwaiter().GetResult();
				if (userListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(userListResponse.message, "Vina invoice thông báo:");
					return null;
				}
				if (userListResponse.data.user_list != null)
				{
					if (userListResponse.data.user_list.Count() > 0)
					{
						foreach (User_List element in userListResponse.data.user_list)
						{
							var user = new User
							{
								Id = element.id,
								Email = element.email,
								Role = element.role,
								Status = element.active,

							};
							list.Add(user);
						}
					}
				}
				return list;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}
           
        }


    }
}
