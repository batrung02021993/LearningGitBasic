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
    public class ProductRestApi
    {
        private BaseRestApi _baseRestApi = new BaseRestApi();

        public IEnumerable<Product> GetListProduct(int _page)
        {
           
			try
			{
				List<Product> list = new List<Product>();
				var request = new ProductListRequest
				{
					page = _page
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<ProductListResponse, ProductListRequest>(profile.token, request, "invoice/product/list");
				var productListResponse = task.GetAwaiter().GetResult();
				if (productListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(productListResponse.message, "Vina invoice thông báo:");
					return null;
				}
				if (productListResponse.data != null)
				{
					if (productListResponse.data.invoice_product_list.Count() > 0)
					{
						return productListResponse.data.invoice_product_list;
					}
					else return null;
				}
				else return null;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}


		}

		public string Create(Product product)
        {		          
			try
			{
				var request = product;
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<CreateProductResponse, Product>(profile.token, request, "invoice/product/add");
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

        public void Update(Product product)
        {
            try
            {
                var request = new ProductUpdateRequest
                {
                    id = new[] { product.id },
                    item_name = product.item_name,
                    unit_name = product.unit_name,
                    unit_price = product.unit_price,
                    quantity = product.quantity,
                    item_total_amount_without_vat = product.item_total_amount_without_vat,
                    vat_percentage = product.vat_percentage,
                    vat_amount = product.vat_amount,
                    current_code = product.current_code,
                    item_note = product.item_note,
                    item_type = product.item_type,
                    discount_percentage = product.discount_percentage,
                    discount_amount = product.discount_amount,
                    total_amount = product.total_amount,
                    item_code = product.item_code
                };

                var profile = (ProfileData)ApplicationCache.GetItem("profile");
                var task = _baseRestApi.AddAsync<CreateProductResponse, ProductUpdateRequest>(profile.token, request, "invoice/product/edit");
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
				var request = new ProductDeleteRequest
				{
					id = delete_list
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<ProductDeleteResponse, ProductDeleteRequest>(profile.token, request, "invoice/product/delete");
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

		public IEnumerable<Product> Search(int _page, int _type_search, string _key)
		{
           
			try
			{
				ProductSearchRequest request = new ProductSearchRequest
				{
					page = _page,
					type_search = _type_search,
					key = _key
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<ProductSearchResponse, ProductSearchRequest>(profile.token, request, "invoice/product/search");
				ProductSearchResponse Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				List<Product> list = new List<Product>();
				if (Response.data != null)
				{
					if (Response.data.invoice_product_list.Count() > 0)
					{
						list = Response.data.invoice_product_list;

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

		public ProductSearchResponse SearchApiBase(ProductSearchRequest request)
        {
			try
			{
            var profile = (ProfileData)ApplicationCache.GetItem("profile");
            var task = _baseRestApi.AddAsync<ProductSearchResponse, ProductSearchRequest>(profile.token, request, "invoice/product/search");
            var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
            return Response;
        	}
			catch{
					MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
					return null;
			}
		}
        internal ProductAddListResponse AddProductList(List<ProductAddList> import_product_list)
        {
            var profile = (ProfileData)ApplicationCache.GetItem("profile");
            var task = _baseRestApi.AddAsync<ProductAddListResponse, ProductAddList[]>(profile.token, import_product_list.ToArray(), "invoice/product/add_list");
            var Response = task.GetAwaiter().GetResult();
            return Response;
        }
    }


    public class CreateProductResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class ProductUpdateRequest
    {
        public string[] id = new string[1];

        public string item_name { get; set; }

        public string unit_name { get; set; }

        public double unit_price { get; set; }

        public double quantity { get; set; }

        public double item_total_amount_without_vat { get; set; }

        public int vat_percentage { get; set; }

        public double vat_amount { get; set; }

        public string current_code { get; set; }

        public string item_note { get; set; }

        public string item_type { get; set; }

        public int discount_percentage { get; set; }

        public double discount_amount { get; set; }

        public double total_amount { get; set; }

        public string item_code { get; set; }

    }
}
