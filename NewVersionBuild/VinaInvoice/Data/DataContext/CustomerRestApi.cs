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

	public class CustomerRestApi
	{
		private BaseRestApi _baseRestApi = new BaseRestApi();

		public IEnumerable<Customer> GetList(int _page) {
			try
			{
				List<Customer> list = new List<Customer>();
				var request = new CustomerListRequest
				{
					page = _page
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<CustomerListResponse, CustomerListRequest>(profile.token, request, "invoice/customer/list");
				var ListResponse = task.GetAwaiter().GetResult();
				if (ListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(ListResponse.message, "Vina invoice thông báo:");
					return null;
				}
				if (ListResponse.data != null)
				{
					if (ListResponse.data.invoice_customer_list.Count() > 0)
					{
						int count = 0;
						foreach (var element in ListResponse.data.invoice_customer_list)
						{
							count++;
							var customer = new Customer
							{
								Address = element.address,
								CompanyName = element.company_name,
								BankName = element.bank_name,
								CompanyTaxCode = element.company_tax_code,
								DisplayName = element.dislay_name,
								Director = element.director,
								STT = count,
								Website = element.website,
								BankNumber = element.bank_number,
								Description = element.description,
								FaxNumber = element.fax_number,
								Id = element.id,
								PersonalTaxCode = element.tax_code,
								PhoneNumber = element.phone_number,
								Email = element.email,
								IsSelected = false
							};
							list.Add(customer);
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

		public string Create(Customer customer)
		{
			try
			{

				var request = new customerCreateRequest
				{
					phone_number = customer.PhoneNumber,
					tax_code = customer.PersonalTaxCode,
					address = customer.Address,
					bank_name = customer.BankName,
					bank_number = customer.BankNumber,
					company_name = customer.CompanyName,
					company_tax_code = customer.CompanyTaxCode,
					description = customer.Description,
					director = customer.Director,
					dislay_name = customer.DisplayName,
					fax_number = customer.FaxNumber,
					website = customer.Website,
					email = customer.Email

				};

				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<CreatecustomerResponse, customerCreateRequest>(profile.token, request, "invoice/customer/add");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response.message;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}



		}

        public string Create(Invoice invoice)
        {
            try
            {

                var request = new customerCreateRequest
                {
                    phone_number = invoice.b_phone_number,
                    tax_code = "",
                    address = invoice.b_address,
                    bank_name = invoice.b_bank_name,
                    bank_number = invoice.b_bank_number,
                    company_name = invoice.b_company,
                    company_tax_code = invoice.b_tax_code,
                    description = "",
                    director = "",
                    dislay_name = invoice.b_name,
                    fax_number = invoice.b_fax_number,
                    website = invoice.b_website,
                    email = invoice.b_email

                };

                var profile = (ProfileData)ApplicationCache.GetItem("profile");
                var task = _baseRestApi.AddAsync<CreatecustomerResponse, customerCreateRequest>(profile.token, request, "invoice/customer/add");
                var Response = task.GetAwaiter().GetResult();
                if (Response.code != Const.Code_Successful)
                {
                    MessageBox.Show(Response.message, "Vina invoice thông báo:");
                    return null;
                }
                return Response.message;
            }
            catch
            {
                MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                return null;

            }
        }

        public void Update(Customer customer)
        {
            try
            {
                var request = new CustomerUpdateRequest
                {
                    id = new[] { customer.Id },
                    email = customer.Email,
                    dislay_name = customer.DisplayName,
                    tax_code = customer.TaxCode,
                    address = customer.Address,
                    phone_number = customer.PhoneNumber,
                    fax_number  = customer.FaxNumber,
                    website = customer.Website,
                    bank_name = customer.BankName,
                    bank_number = customer.BankNumber,
                    company_tax_code = customer.CompanyTaxCode,
                    company_name = customer.CompanyName,
                    director = customer.Director,
                    description = customer.Description
                };

                var profile = (ProfileData)ApplicationCache.GetItem("profile");
                var task = _baseRestApi.AddAsync<CustomerUpdateResponse, CustomerUpdateRequest>(profile.token, request, "invoice/customer/edit");
                var Response = task.GetAwaiter().GetResult();
                if (Response.code != Const.Code_Successful)
                {
                    MessageBox.Show(Response.message, Message.MSS_DIALOG_TITLE_ALERT);
                }
                else
                {
                    MessageBox.Show(Response.message, Message.MSS_DIALOG_TITLE_ALERT);
                }
            }
            catch
            {
                MessageBox.Show(Const.Internet_ERROR, Message.MSS_DIALOG_TITLE_ALERT);
            }
        }

        public string Delete(string[] delete_list)
        {
			try
			{
				var request = new CustomerDeleteRequest
				{
					id = delete_list
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<CustomerDeleteResponse, CustomerDeleteRequest>(profile.token, request, "invoice/customer/delete");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				string result = "";
				if (Response.data.SuccessDelete != null)
				{
					result += "Success delete: \n\n";
					for (int i = 0; i < Response.data.SuccessDelete.Length; i++)
					{
						result += Response.data.SuccessDelete[i].ToString() + "\n";
					}
					result += "\n\n";
				}

				if (Response.data.FailDelete != null)
				{
					result += "Fail delete: \n\n";
					for (int i = 0; i < Response.data.FailDelete.Length; i++)
					{
						result += Response.data.FailDelete[i].ToString() + "\n";
					}
					result += "\n\n";
				}

				return result;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}


			
        }

        public IEnumerable<Customer> Search(int _page, int _type_search, string _key)
        {
			try
			{
				CustomerSearchRequest request = new CustomerSearchRequest
				{
					page = _page,
					type_search = _type_search,
					key = _key
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<CustomerSearchResponse, CustomerSearchRequest>(profile.token, request, "invoice/customer/search");
				CustomerSearchResponse Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				List<Customer> list = new List<Customer>();
				if (Response.data != null)
				{
					if (Response.data.invoice_customer_list.Count() > 0)
					{
						foreach (Invoice_Customer_Search element in Response.data.invoice_customer_list)
						{
							var customer = new Customer
							{
								Id = element.id,
								IsSelected = false,
								DisplayName = element.dislay_name,
								CompanyName = element.company_name,
								CompanyTaxCode = element.company_tax_code,
								Address = element.address,
								PhoneNumber = element.phone_number,
								Website = element.website,
								FaxNumber = element.fax_number,
								BankNumber = element.bank_number,
								BankName = element.bank_name
							};
							list.Add(customer);
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

        public CustomerSearchResponse SearchApiBase(CustomerSearchRequest request)
        {
			try
			{

				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<CustomerSearchResponse, CustomerSearchRequest>(profile.token, request, "invoice/customer/search");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}


        }

        public CustomerAddListResponse AddCustomerList(List<CustomerAddList> import_customer_list)
        {
            var profile = (ProfileData)ApplicationCache.GetItem("profile");
            var task = _baseRestApi.AddAsync<CustomerAddListResponse, CustomerAddList[]>(profile.token, import_customer_list.ToArray(), "invoice/customer/add_list");
            var Response = task.GetAwaiter().GetResult();
            return Response;
        }
    }

	public class CustomerListRequest
	{
		public int page { get; set; }
	}

	public class customerCreateRequest
	{
		public string dislay_name { get; set; }
		public string email { get; set; }
		public string tax_code { get; set; }
		public string address { get; set; }
		public string phone_number { get; set; }
		public string fax_number { get; set; }
		public string website { get; set; }
		public string bank_name { get; set; }
		public string bank_number { get; set; }
		public string company_tax_code { get; set; }
		public string company_name { get; set; }
		public string director { get; set; }
		public string description { get; set; }
	}

    public class CustomerUpdateRequest
    {
        public string[] id = new string[1];

        public string email { get; set; }

        public string dislay_name { get; set; }

        public string tax_code { get; set; }

        public string address { get; set; }

        public string phone_number { get; set; }

        public string fax_number { get; set; }

        public string website { get; set; }

        public string bank_name { get; set; }

        public string bank_number { get; set; }

        public string company_tax_code { get; set; }

        public string company_name { get; set; }

        public string director { get; set; }

        public string description { get; set; }

    }
	public class CreatecustomerResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public object data { get; set; }
	}

    public class CustomerUpdateResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
