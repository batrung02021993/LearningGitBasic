using Newtonsoft.Json;
using OfficeOpenXml;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Common
{
    public class Excel_Utils
    {
        #region Product Import Related Excel

        public static void ProductImportExcel()
        {
            System.Windows.Forms.OpenFileDialog importProductExcelDialog = new System.Windows.Forms.OpenFileDialog();
            importProductExcelDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            importProductExcelDialog.Title = "Import Excel Invoice";

            if (importProductExcelDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string import_excel_path = importProductExcelDialog.FileName;
                Console.WriteLine(import_excel_path);

                DataTable import_table = ProductExcel2DataTable(import_excel_path);
                List<ProductAddList> import_product_list = DataTable2ProductList(import_table);
                ProductAddListResponse response = ProductList2Server(import_product_list);
                System.Windows.MessageBox.Show(response.message.ToString());
            }
        }

        private static DataTable ProductExcel2DataTable(string import_excel_path)
        {
            DataTable table = new DataTable();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(import_excel_path)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                //Check if worksheet is empty
                if (worksheet.Dimension == null)
                    return table;

                List<string> columnNames = new List<string>();
                int currentColumn = 1;

                foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    string columnName = cell.Text.Trim();

                    //check if the previous header was empty and add it if it was
                    if (cell.Start.Column != currentColumn)
                    {
                        columnNames.Add("Header_" + currentColumn);
                        table.Columns.Add("Header_" + currentColumn);
                        currentColumn++;
                    }

                    //add the column name to the list to count the duplicates
                    columnNames.Add(columnName);

                    //count the duplicate column names and make them unique to avoid the exception
                    //A column named 'Name' already belongs to this DataTable
                    int occurrences = columnNames.Count(x => x.Equals(columnName));
                    if (occurrences > 1)
                    {
                        columnName = columnName + "_" + occurrences;
                    }

                    //add the column to the datatable
                    table.Columns.Add(columnName);

                    currentColumn++;
                }

                //start adding the contents of the excel file to the datatable
                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                    DataRow newRow = table.NewRow();

                    //loop all cells in the row
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }

                    table.Rows.Add(newRow);
                }
            }

            Console.WriteLine("Excel to DataTable done");
            return table;
        }

        private static List<ProductAddList> DataTable2ProductList(DataTable import_table)
        {
            List<ProductAddList> product_add_list = new List<ProductAddList>();

            foreach (DataRow row in import_table.Rows)
            {
                product_add_list.Add(new ProductAddList
                {
                    item_code = row["Mã sản phẩm"].ToString(),
                    item_name = row["Hàng hóa dịch vụ"].ToString(),                    
                    unit_name = row["Đơn vị tính"].ToString(),
                    unit_price = (row["Đơn giá"].ToString() == "") ? 0 : double.Parse(row["Đơn giá"].ToString()),
                    quantity = (row["Số lượng"].ToString() == "") ? 0 : double.Parse(row["Số lượng"].ToString()),
                    item_total_amount_without_vat = (row["Tiền trước thuế"].ToString() == "") ? 0 : double.Parse(row["Tiền trước thuế"].ToString()),
                    vat_percentage = (row["% VAT"].ToString() == "") ? 0 : int.Parse(row["% VAT"].ToString()),
                    vat_amount = (row["Tiền thuế VAT"].ToString() == "") ? 0 : double.Parse(row["Tiền thuế VAT"].ToString()),
                    discount_percentage = (row["% Giảm giá"].ToString() == "") ? 0 : int.Parse(row["% Giảm giá"].ToString()),
                    discount_amount = (row["Tiền giảm giá"].ToString() == "") ? 0 : double.Parse(row["Tiền giảm giá"].ToString()),
                    total_amount = (row["Tổng tiền"].ToString() == "") ? 0 : double.Parse(row["Tổng tiền"].ToString()),
                    current_code = row["Đơn vị tiền tệ"].ToString()
                });
            }
            return product_add_list;
        }

        private static ProductAddListResponse ProductList2Server(List<ProductAddList> import_product_list)
        {
            var api = new ProductRestApi();
            ProductAddListResponse response = api.AddProductList(import_product_list);
            return response;
        }

        #endregion

        #region Customer Import Related Excel

        public static void CustomerImportExcel()
        {
            System.Windows.Forms.OpenFileDialog importCustomerExcelDialog = new System.Windows.Forms.OpenFileDialog();
            importCustomerExcelDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            importCustomerExcelDialog.Title = "Import Excel Invoice";

            if (importCustomerExcelDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string import_excel_path = importCustomerExcelDialog.FileName;
                Console.WriteLine(import_excel_path);

                DataTable import_table = CustomerExcel2DataTable(import_excel_path);
                List<CustomerAddList> import_product_list = DataTable2CustomerList(import_table);
                CustomerAddListResponse response = CustomerList2Server(import_product_list);
                System.Windows.MessageBox.Show(response.message.ToString());
            }
        }

        private static DataTable CustomerExcel2DataTable(string import_excel_path)
        {
            DataTable table = new DataTable();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(import_excel_path)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                //Check if worksheet is empty
                if (worksheet.Dimension == null)
                    return table;

                List<string> columnNames = new List<string>();
                int currentColumn = 1;

                foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    string columnName = cell.Text.Trim();

                    //check if the previous header was empty and add it if it was
                    if (cell.Start.Column != currentColumn)
                    {
                        columnNames.Add("Header_" + currentColumn);
                        table.Columns.Add("Header_" + currentColumn);
                        currentColumn++;
                    }

                    //add the column name to the list to count the duplicates
                    columnNames.Add(columnName);

                    //count the duplicate column names and make them unique to avoid the exception
                    //A column named 'Name' already belongs to this DataTable
                    int occurrences = columnNames.Count(x => x.Equals(columnName));
                    if (occurrences > 1)
                    {
                        columnName = columnName + "_" + occurrences;
                    }

                    //add the column to the datatable
                    table.Columns.Add(columnName);

                    currentColumn++;
                }

                //start adding the contents of the excel file to the datatable
                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                    DataRow newRow = table.NewRow();

                    //loop all cells in the row
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }

                    table.Rows.Add(newRow);
                }
            }

            Console.WriteLine("Excel to DataTable done");
            return table;
        }

        private static List<CustomerAddList> DataTable2CustomerList(DataTable import_table)
        {
            List<CustomerAddList> customer_add_list = new List<CustomerAddList>();

            foreach (DataRow row in import_table.Rows)
            {
                customer_add_list.Add(new CustomerAddList
                {
                    dislay_name = row["Tên khách hàng"].ToString(),
                    company_name = row["Tên công ty"].ToString(),
                    company_tax_code = row["Mã số thuế"].ToString(),
                    address = row["Địa chỉ"].ToString(),
                    phone_number = row["Số điện thoại"].ToString(),
                    fax_number = row["Số Fax"].ToString(),
                    email = row["Email"].ToString(),                
                    website = row["Website"].ToString(),
                    bank_name = row["Tên ngân hàng"].ToString(),
                    bank_number = row["Số TK ngân hàng"].ToString(),
                });
            }
            return customer_add_list;
        }

        private static CustomerAddListResponse CustomerList2Server(List<CustomerAddList> import_customer_list)
        {
            var api = new CustomerRestApi();
            CustomerAddListResponse response = api.AddCustomerList(import_customer_list);
            return response;
        }

        #endregion

        #region Invoice Import Related Excel

        public static void InvoiceImportExcelDraft()
        {
            OpenFileDialog importExcelDialog = new OpenFileDialog();
            importExcelDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            importExcelDialog.Title = "Import Excel Invoice";

            if (importExcelDialog.ShowDialog() == DialogResult.OK)
            {
                string import_excel_path = importExcelDialog.FileName;
                Console.WriteLine(import_excel_path);

                DataTable import_table = InvoiceExcel2DataTable(import_excel_path);
                try
                {
                    List<InvoiceAddListDraft> invoice_list = DataTable2ServerDraft(import_table);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(VinaInvoice.Common.Message.MSS_IMPORT_WRONG_FORMAT);
                    System.Windows.MessageBox.Show(e.ToString());
                }
            }
        }

        private static DataTable InvoiceExcel2DataTable(string excel_path)
        {
            DataTable table = new DataTable();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(excel_path)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                //Check if worksheet is empty
                if (worksheet.Dimension == null)
                    return table;

                List<string> columnNames = new List<string>();
                int currentColumn = 1;

                foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    string columnName = cell.Text.Trim();

                    //check if the previous header was empty and add it if it was
                    if (cell.Start.Column != currentColumn)
                    {
                        columnNames.Add("Header_" + currentColumn);
                        table.Columns.Add("Header_" + currentColumn);
                        currentColumn++;
                    }

                    //add the column name to the list to count the duplicates
                    columnNames.Add(columnName);

                    //count the duplicate column names and make them unique to avoid the exception
                    //A column named 'Name' already belongs to this DataTable
                    int occurrences = columnNames.Count(x => x.Equals(columnName));
                    if (occurrences > 1)
                    {
                        columnName = columnName + "_" + occurrences;
                    }

                    //add the column to the datatable
                    table.Columns.Add(columnName);

                    currentColumn++;
                }

                //start adding the contents of the excel file to the datatable
                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                    DataRow newRow = table.NewRow();

                    //loop all cells in the row
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }

                    table.Rows.Add(newRow);
                }
            }

            Console.WriteLine("Excel to DataTable done");
            return table;
        }

 public static DataTable MisaExcel2DataTable(string excel_path, int Id)
        {
            DataTable tableTemp = new DataTable();
            try
            {
                DataTable table = new DataTable();

                tableTemp = new DataTable();

                Workbook workbook = new Workbook();
                workbook.LoadFromFile(excel_path);
                Worksheet sheet = workbook.Worksheets[0];

                table = sheet.ExportDataTable();


                //var convertedList = (from rw in table.AsEnumerable()
                //                     select new MisaObjectModel()
                //                     {
                //                         Column1 = rw["Column1"].ToString(),
                //                         Column2 = rw["Column2"].ToString(),
                //                         Column3 = rw["Column3"].ToString(),
                //                         Column4 = rw["Column4"].ToString(),
                //                         Column5 = rw["Column5"].ToString(),
                //                         Column6 = rw["Column6"].ToString(),
                //                         Column7 = rw["Column7"].ToString(),
                //                         Column8 = rw["Column8"].ToString(),
                //                         Column9 = rw["Column9"].ToString(),
                //                         Column10 = rw["Column10"].ToString(),
                //                         Column11 = rw["Column11"].ToString(),
                //                         Column12 = rw["Column12"].ToString(),
                //                         Column13 = rw["Column13"].ToString(),
                //                         Column14 = rw["Column14"].ToString(),
                //                         Column15 = rw["Column15"].ToString(),
                //                         Column16 = rw["Column16"].ToString(),
                //                         Column17 = rw["Column17"].ToString(),
                //                         Column18 = rw["Column18"].ToString(),
                //                         Column19 = rw["Column19"].ToString(),
                //                         Column20 = rw["Column20"].ToString(),
                //                         Column21 = rw["Column21"].ToString(),
                //                         Column22 = rw["Column22"].ToString(),
                //                         Column23 = rw["Column23"].ToString(),
                //                         Column24 = rw["Column24"].ToString(),
                //                         Column25 = rw["Column25"].ToString(),
                //                         Column26 = rw["Column26"].ToString(),
                //                         Column27 = rw["Column27"].ToString(),
                //                         Column28 = rw["Column28"].ToString(),
                //                         Column29 = rw["Column29"].ToString(),
                //                         Column30 = rw["Column30"].ToString(),
                //                         Column31 = rw["Column31"].ToString(),
                //                         Column32 = rw["Column32"].ToString(),
                //                         Column33 = rw["Column33"].ToString(),
                //                         Column34 = rw["Column34"].ToString(),
                //                         Column35 = rw["Column35"].ToString(),
                //                         Column36 = rw["Column36"].ToString(),
                //                         Column37 = rw["Column37"].ToString(),
                //                         Column38 = rw["Column38"].ToString(),
                //                         Column39 = rw["Column39"].ToString(),
                //                         Column40 = rw["Column40"].ToString(),
                //                         Column41 = rw["Column41"].ToString(),
                //                         Column42 = rw["Column42"].ToString(),
                //                         Column43 = rw["Column43"].ToString(),
                //                         Column44 = rw["Column44"].ToString()
                //                     }).ToList();


                string HDGTGT = string.Empty;
                string MauSoTitle = string.Empty;
                string KyHieuTitle = string.Empty;
                string NumberTitle = string.Empty;
                string DayTitle = string.Empty;
                string MonthTitle = string.Empty;
                string YearTitle = string.Empty;
                string DonViBHTitle = string.Empty;
                string MSTTitle = string.Empty;
                string DiaChiTitle = string.Empty;
                string DienThoaiTitle = string.Empty;
                string STKTitle = string.Empty;
                string CustomerNameTitle = string.Empty;
                string TenDonViCustomerTitle = string.Empty;
                string MSTCustomerTitle = string.Empty;
                string DiaChiCustomerTitle = string.Empty;
                string HTTTCustomerTitle = string.Empty;
                string STKCustomerTitle = string.Empty;

                string MauSo = string.Empty;
                string KyHieu = string.Empty;
                string Number = string.Empty;
                string Day = string.Empty;
                string Month = string.Empty;
                string Year = string.Empty;
                string DonViBH = string.Empty;
                string MST = string.Empty;
                string DiaChi = string.Empty;
                string DienThoai = string.Empty;
                string STK = string.Empty;
                string CustomerName = string.Empty;
                string TenDonViCustomer = string.Empty;
                string MSTCustomer = string.Empty;
                string DiaChiCustomer = string.Empty;
                string HTTTCustomer = string.Empty;
                string STKCustomer = string.Empty;

                List<MisaProduct> misaList = new List<MisaProduct>();

                string CK = string.Empty;
                string CKAmount = string.Empty;

                string TotalThanhTien = string.Empty;
                string ThueGTGT = string.Empty;
                string TotalGTGT = string.Empty;
                string Total = string.Empty;

                string indexOf = string.Empty;

                IList<int> indexRow = new List<int>();
                //foreach (DataRow dtRow in table.Rows)
                //{
                //    int index = table.Rows.IndexOf(dtRow);

                //    if (index == 42)
                //    {
                //        break;
                //    }
                //    switch (index)
                //    {
                //        case 1:
                //            MauSo = dtRow["Column37"].ToString();
                //            continue;
                //        case 2:
                //            KyHieu = dtRow["Column38"].ToString();
                //            continue;
                //        case 4:
                //            Number = dtRow["Column36"].ToString();
                //            continue;
                //        case 5:
                //            Day = dtRow["Column18"].ToString();
                //            Month = dtRow["Column23"].ToString();
                //            Year = dtRow["Column25"].ToString();
                //            continue;
                //        case 9:
                //            DonViBH = dtRow["Column10"].ToString();
                //            continue;
                //        case 10:
                //            MST = dtRow["Column8"].ToString();
                //            continue;
                //        case 11:
                //            DiaChi = dtRow["Column6"].ToString();
                //            continue;
                //        case 12:
                //            DienThoai = dtRow["Column7"].ToString();
                //            STK = dtRow["Column7"].ToString();
                //            continue;
                //        case 14:
                //            CustomerName = dtRow["Column13"].ToString();
                //            continue;
                //        case 15:
                //            indexOf = dtRow["Column3"].ToString();
                //            if (string.IsNullOrEmpty(indexOf) == false)
                //            {
                //                TenDonViCustomer = dtRow["Column8"].ToString();
                //            }
                //            continue;
                //        case 16:
                //            if (string.IsNullOrEmpty(indexOf) == true)
                //            {
                //                TenDonViCustomer = dtRow["Column8"].ToString();
                //            }
                //            else
                //            {
                //                MSTCustomer = dtRow["Column8"].ToString();
                //            }
                //            continue;
                //        case 17:
                //            if (string.IsNullOrEmpty(indexOf) == true)
                //            {
                //                MSTCustomer = dtRow["Column8"].ToString();
                //            }
                //            else
                //            {
                //                DiaChiCustomer = dtRow["Column6"].ToString();
                //            }
                //            continue;
                //        case 18:
                //            if (string.IsNullOrEmpty(indexOf) == true)
                //            {
                //                DiaChiCustomer = dtRow["Column6"].ToString();
                //            }
                //            else
                //            {
                //                HTTTCustomer = dtRow["Column12"].ToString();
                //                STKCustomer = dtRow["Column28"].ToString();
                //            }
                //            continue;
                //        case 19:
                //            if (string.IsNullOrEmpty(indexOf) == true)
                //            {
                //                HTTTCustomer = dtRow["Column12"].ToString();
                //                STKCustomer = dtRow["Column28"].ToString();
                //            }
                //            continue;
                //        case 23:
                //        case 24:
                //        case 25:
                //        case 26:
                //        case 27:
                //        case 28:
                //        case 29:
                //        case 30:
                //        case 31:
                //        case 32:
                //        case 33:
                //        case 34:
                //        case 35:
                //        case 36:
                //        case 37:
                //        case 38:
                //            string stt = dtRow["Column3"].ToString();
                //            if (string.IsNullOrEmpty(stt))
                //            {
                //                break;
                //            }
                //            MisaProduct misaProduct = new MisaProduct();
                //            misaProduct.ID = Id.ToString();
                //            misaProduct.STT = dtRow["Column3"].ToString();
                //            misaProduct.MaHH = dtRow["Column5"].ToString();
                //            misaProduct.TenHH = dtRow["Column12"].ToString();
                //            misaProduct.DVT = dtRow["Column22"].ToString();
                //            misaProduct.SL = dtRow["Column27"].ToString();
                //            misaProduct.DG = dtRow["Column35"].ToString();
                //            misaProduct.ThanhTien = dtRow["Column39"].ToString();
                //            misaList.Add(misaProduct);
                //            continue;
                //        case 39:
                //            TotalThanhTien = dtRow["Column30"].ToString();
                //            continue;
                //        case 40:
                //            ThueGTGT = dtRow["Column9"].ToString();
                //            TotalGTGT = dtRow["Column25"].ToString();
                //            continue;
                //        case 41:
                //            Total = dtRow["Column26"].ToString();
                //            continue;
                //        case 42:
                //            break;
                //    }
                //}

                //foreach (DataRow dtRow in table.Rows)
                //{
                //    int index = table.Rows.IndexOf(dtRow);
                //    if (dtRow[0,34].ToString() == "Mẫu số")
                //    {
                //        MauSo = dtRow["Column37"].ToString();
                //    }
                //}
                bool stop = false;

                for (int i = 0; i <= table.Rows.Count - 1; i++)
                {
                    if (stop == true)
                    {
                        break;
                    }
                    for (int j = 0; j <= table.Columns.Count - 1; j++)
                    {
                        var cellValue = table.Rows[i][j];
                        if (cellValue.ToString() == "HÓA ĐƠN GIÁ TRỊ GIA TĂNG" && i !=1)
                        {
                            stop = true;
                            break;
                        }
                        if (cellValue.ToString() == "Mẫu số:")
                        {
                            MauSo = table.Rows[i][j + 3].ToString();
                        }
                        if (cellValue.ToString() == "Ký hiệu:")
                        {
                            KyHieu = table.Rows[i][j + 4].ToString(); break;
                        }
                        if (cellValue.ToString() == "Số:")
                        {
                            Number = table.Rows[i][j + 2].ToString(); break;
                        }

                        if (cellValue.ToString() == "Ngày")
                        {
                            Day = table.Rows[i][j + 2].ToString();
                        }
                        if (cellValue.ToString() == "tháng")
                        {
                            Month = table.Rows[i][j + 3].ToString();
                        }
                        if (cellValue.ToString() == "năm")
                        {
                            Year = table.Rows[i][j + 1].ToString(); break;
                        }

                        if (cellValue.ToString() == "Đơn vị bán hàng:")
                        {
                            DonViBH = table.Rows[i][j + 8].ToString(); break;
                        }
                        if (cellValue.ToString() == "Mã số thuế:" && i == 10)
                        {
                            MST = table.Rows[i][j + 6].ToString(); break;
                        }

                        if (cellValue.ToString() == "Địa chỉ:" && i == 11)
                        {
                            DiaChi = table.Rows[i][j + 3].ToString(); break;
                        }
                        if (cellValue.ToString() == "Điện thoại:" && i == 12)
                        {
                            DienThoai = table.Rows[i][j + 5].ToString();
                        }
                        if (cellValue.ToString() == "Số tài khoản:" && i == 12)
                        {
                            STK = table.Rows[i][j + 6].ToString(); break;
                        }
                        if (cellValue.ToString() == "Họ tên người mua hàng:")
                        {
                            CustomerName = table.Rows[i][j + 11].ToString();
                            if (string.IsNullOrEmpty(CustomerName) == true)
                            {
                                CustomerName = table.Rows[i][j + 10].ToString();
                            }
                            break;
                        }
                        if (cellValue.ToString() == "Tên đơn vị:")
                        {
                            TenDonViCustomer = table.Rows[i][j + 6].ToString();
                            if (string.IsNullOrEmpty(TenDonViCustomer) == true)
                            {
                                TenDonViCustomer = table.Rows[i][j + 5].ToString(); 
                            }
                            break;
                        }
                        if (cellValue.ToString() == "Mã số thuế:")
                        {
                            MSTCustomer = table.Rows[i][j + 6].ToString();
                            if (string.IsNullOrEmpty(MSTCustomer) == true)
                            {
                                MSTCustomer = table.Rows[i][j + 5].ToString();
                            }
                            break;
                        }
                        if (cellValue.ToString() == "Địa chỉ:")
                        {
                            DiaChiCustomer = table.Rows[i][j + 3].ToString(); 
                            if (string.IsNullOrEmpty(DiaChiCustomer) == true)
                            {
                                DiaChiCustomer = table.Rows[i][j + 4].ToString();
                            }
                            break;
                        }
                        if (cellValue.ToString() == "Hình thức thanh toán:")
                        {
                            HTTTCustomer = table.Rows[i][j + 10].ToString();
                            if (string.IsNullOrEmpty(HTTTCustomer) == true)
                            {
                                HTTTCustomer = table.Rows[i][j + 9].ToString();
                            }
                        }
                        if (cellValue.ToString() == "Số tài khoản:")
                        {
                            STKCustomer = table.Rows[i][j + 6].ToString();
                            if (string.IsNullOrEmpty(STKCustomer) == true)
                            {
                                STKCustomer = table.Rows[i][j + 5].ToString();
                            }
                            break;
                        }


                        if (cellValue.ToString() == "STT")
                        {
                            for (int k = i + 2; k <= 44; k++)
                            {
                                int x = 0;
                                MisaProduct misaProduct = new MisaProduct();
                                misaProduct.ID = Id.ToString();
                                misaProduct.STT = table.Rows[k][x + 2].ToString();
                                misaProduct.MaHH = table.Rows[k][x + 4].ToString();
                                misaProduct.TenHH = table.Rows[k][x + 12].ToString();
                                if (string.IsNullOrEmpty(misaProduct.TenHH) == true)
                                {
                                    misaProduct.TenHH = table.Rows[k][x + 11].ToString();
                                }
                                misaProduct.DVT = table.Rows[k][x + 22].ToString();
                                if (string.IsNullOrEmpty(misaProduct.DVT) == true)
                                {
                                    misaProduct.DVT = table.Rows[k][x + 21].ToString();
                                }
                                misaProduct.SL = table.Rows[k][x + 28].ToString();
                                if (string.IsNullOrEmpty(misaProduct.SL) == true)
                                {
                                    misaProduct.SL = table.Rows[k][x + 26].ToString();
                                }
                                misaProduct.DG = table.Rows[k][x + 36].ToString();
                                if (string.IsNullOrEmpty(misaProduct.DG) == true)
                                {
                                    misaProduct.DG = table.Rows[k][x + 34].ToString();
                                }
                                misaProduct.ThanhTien = table.Rows[k][x + 40].ToString();
                                if (string.IsNullOrEmpty(misaProduct.ThanhTien) == true)
                                {
                                    misaProduct.ThanhTien = table.Rows[k][x + 38].ToString();
                                }

                                if (table.Rows[k][x + 2].ToString() == "Tỷ lệ CK:" || string.IsNullOrEmpty(table.Rows[k][x + 2].ToString()))
                                {
                                    break;
                                }
                                else
                                {
                                    misaList.Add(misaProduct);

                                }
                            }
                        }

                        if (cellValue.ToString() == "Tỷ lệ CK:")
                        {
                            CK = table.Rows[i][j + 4].ToString(); 
                        }
                        if (cellValue.ToString() == "Số tiền chiết khấu:")
                        {
                            CKAmount = table.Rows[i][j + 7].ToString(); break;
                        }
                        if (cellValue.ToString() == "Cộng tiền hàng (Đã trừ CK):")
                        {
                            TotalThanhTien = table.Rows[i][j + 12].ToString();
                            break;
                        }
                        if (cellValue.ToString() == "Cộng tiền hàng:")
                        {
                            TotalThanhTien = table.Rows[i][j + 11].ToString();
                            break;
                        }
                        if (cellValue.ToString() == "Thuế suất GTGT:")
                        {
                            ThueGTGT = table.Rows[i][j + 7].ToString();
                            if (string.IsNullOrEmpty(ThueGTGT) == true)
                            {
                                ThueGTGT = table.Rows[i][j + 6].ToString();
                            }
                        }
                        if (cellValue.ToString() == "Tiền thuế GTGT:")
                        {
                            TotalGTGT = table.Rows[i][j + 6].ToString(); break;
                        }
                        if (cellValue.ToString() == "Tổng tiền thanh toán:")
                        {
                            Total = table.Rows[i][j + 8].ToString();
                            if (string.IsNullOrEmpty(Total) == true)
                            {
                                Total = table.Rows[i][j + 7].ToString();
                            }
                            break;
                        }
                    }
                }


                tableTemp.TableName = "MISA DATATABLE";

                tableTemp.Columns.Add("ID");
                tableTemp.Columns.Add("Tên khách hàng");
                tableTemp.Columns.Add("Tên công ty");
                tableTemp.Columns.Add("Mã số thuế");
                tableTemp.Columns.Add("Địa chỉ");
                tableTemp.Columns.Add("Số điện thoại");
                tableTemp.Columns.Add("Số Fax");
                tableTemp.Columns.Add("Email");
                tableTemp.Columns.Add("Website");
                tableTemp.Columns.Add("Số TK ngân hàng");
                tableTemp.Columns.Add("Tên ngân hàng");
                tableTemp.Columns.Add("Mẫu HĐ");
                tableTemp.Columns.Add("Ký hiệu HĐ");
                tableTemp.Columns.Add("Số HĐ");
                tableTemp.Columns.Add("Ngày HĐ");
                tableTemp.Columns.Add("Trạng thái");
                tableTemp.Columns.Add("Loại HĐ");
                tableTemp.Columns.Add("Đơn vị tiền tệ");
                tableTemp.Columns.Add("Tỉ giá");
                tableTemp.Columns.Add("Hình thức thanh toán");
                tableTemp.Columns.Add("Số chứng từ");
                tableTemp.Columns.Add("Hóa đơn gốc");
                tableTemp.Columns.Add("Tiền trước thuế");
                tableTemp.Columns.Add("% VAT");
                tableTemp.Columns.Add("Tiền thuế VAT");
                tableTemp.Columns.Add("Giảm giá");
                tableTemp.Columns.Add("% Phí DV");
                tableTemp.Columns.Add("Phí dịch vụ");
                tableTemp.Columns.Add("Tổng tiền");
                tableTemp.Columns.Add("STT");
                tableTemp.Columns.Add("Mã sản phẩm");
                tableTemp.Columns.Add("Hàng hóa dịch vụ");
                tableTemp.Columns.Add("Đơn vị tính");
                tableTemp.Columns.Add("Đơn giá");
                tableTemp.Columns.Add("Số lượng");
                tableTemp.Columns.Add("Tiền chưa thuế");
                tableTemp.Columns.Add("% Thuế VAT");
                tableTemp.Columns.Add("Thuế VAT");
                tableTemp.Columns.Add("% Giảm giá");
                tableTemp.Columns.Add("Tiền giảm giá");
                tableTemp.Columns.Add("Tổng cộng");

                DataRow dataRow = tableTemp.NewRow();

                dataRow["Tên khách hàng"] = CustomerName;
                dataRow["Tên công ty"] = TenDonViCustomer;
                dataRow["Mã số thuế"] = MSTCustomer;
                dataRow["Địa chỉ"] = DiaChiCustomer;
                dataRow["Số điện thoại"] = "";
                dataRow["Số Fax"] = "";
                dataRow["Email"] = "";
                dataRow["Website"] = "";
                dataRow["Số TK ngân hàng"] = STKCustomer;
                dataRow["Tên ngân hàng"] = "";
                //dataRow["Mẫu HĐ"] = MauSo;
                //dataRow["Ký hiệu HĐ"] = KyHieu;
                dataRow["Mẫu HĐ"] = "01GTKT0/001";
                dataRow["Ký hiệu HĐ"] = "DH/18E";
                dataRow["Số HĐ"] = Number;
                dataRow["Ngày HĐ"] = Day + "/" + Month + "/" + Year;
                dataRow["Trạng thái"] = "";
                dataRow["Loại HĐ"] = "";
                dataRow["Đơn vị tiền tệ"] = "";
                dataRow["Tỉ giá"] = "1";
                dataRow["Hình thức thanh toán"] = HTTTCustomer;
                dataRow["Số chứng từ"] = "";
                dataRow["Hóa đơn gốc"] = "";
                dataRow["Tiền trước thuế"] = TotalThanhTien;
                dataRow["% VAT"] = (double.Parse(ThueGTGT) * 100).ToString();
                dataRow["Tiền thuế VAT"] = TotalGTGT;
                dataRow["Giảm giá"] = "";
                dataRow["% Phí DV"] = "";
                dataRow["Phí dịch vụ"] = "";
                dataRow["Tổng tiền"] = Total;

                string moneyNoVAT = string.Empty;
                string vatPercent = string.Empty;
                string vat = string.Empty;

                foreach (MisaProduct p in misaList)
                {

                    dataRow["ID"] = p.ID;

                    dataRow["STT"] = p.STT;
                    dataRow["Mã sản phẩm"] = p.MaHH;
                    dataRow["Hàng hóa dịch vụ"] = p.TenHH;
                    dataRow["Đơn vị tính"] = p.DVT;
                    dataRow["Đơn giá"] = p.DG;
                    dataRow["Số lượng"] = p.SL;

                    dataRow["Tiền chưa thuế"] = (convertStringToDouble(p.DG) * convertStringToDouble(p.SL)).ToString();
                    moneyNoVAT = dataRow["Tiền chưa thuế"].ToString();

                    dataRow["% Thuế VAT"] = (double.Parse(ThueGTGT) * 100).ToString();
                    vatPercent = dataRow["% Thuế VAT"].ToString();

                    dataRow["Thuế VAT"] = (convertStringToDouble(moneyNoVAT) * convertStringToDouble(vatPercent)).ToString();
                    vat = dataRow["Thuế VAT"].ToString();

                    dataRow["% Giảm giá"] = "";
                    dataRow["Tiền giảm giá"] = "";
                    dataRow["Tổng cộng"] = (convertStringToDouble(moneyNoVAT) + convertStringToDouble(vat)).ToString();

                    tableTemp.Rows.Add(dataRow);

                    dataRow = tableTemp.NewRow();

                }
            }
            catch (Exception e)
            {
                InvoiceErrorClient.invoiceErrorClient(e, true);
            }

            return tableTemp;
        }
        public static double convertStringToDouble(string item)
        {
            double value = 0.0D;
            value = double.Parse(item);
            return value;
        }
        public static List<InvoiceAddListDraft> DataTable2ServerDraft(DataTable import_table)
        {
            List<InvoiceAddListDraft> invoice_detail_list = new List<InvoiceAddListDraft>();
            List<ProductAddList> product_list = new List<ProductAddList>();
            List<CustomerAddList> customer_list = new List<CustomerAddList>();

            #region Invoice List Processing

            int invoice_count = 0;
            int item_count = 1;
            InvoiceAddListDraft tmp_invoice = new InvoiceAddListDraft();
            List<InvoiceAddListDraftData> tmp_item_list = new List<InvoiceAddListDraftData>();

            foreach (DataRow row in import_table.Rows)
            {
                //New Invoice
                if (int.Parse(row["ID"].ToString()) > invoice_count)
                {
                    //Add invoice to Invoice List
                    if (invoice_count > 0)
                    {
                        tmp_invoice.invoice_item_list = Clone(tmp_item_list);
                        invoice_detail_list.Add((InvoiceAddListDraft)tmp_invoice.Clone());
                        tmp_item_list.Clear();
                    }

                    invoice_count = int.Parse(row["ID"].ToString());
                    item_count = 1;
                    item_count++;

                    //Invoice Data Mapping
                    tmp_invoice.b_name = row["Tên khách hàng"].ToString();
                    tmp_invoice.b_company = row["Tên công ty"].ToString();
                    tmp_invoice.b_tax_code = row["Mã số thuế"].ToString();
                    tmp_invoice.b_address = row["Địa chỉ"].ToString();
                    tmp_invoice.b_phone_number = row["Số điện thoại"].ToString();
                    tmp_invoice.b_fax_number = row["Số Fax"].ToString();
                    tmp_invoice.b_email = row["Email"].ToString();
                    tmp_invoice.b_website = row["Website"].ToString();
                    tmp_invoice.b_bank_number = row["Số TK ngân hàng"].ToString();
                    tmp_invoice.b_bank_name = row["Tên ngân hàng"].ToString();

                    //tmp_invoice.invoice_form_name = row["Mẫu HĐ"].ToString();
                    //tmp_invoice.b_bank_name = row["Ký hiệu HĐ"].ToString();
                    //tmp_invoice.invoice_number = int.Parse(row["Số HĐ"].ToString());
                    //tmp_invoice.invoice_sign_date = row["Ngày HĐ"].ToString();

                    switch (row["Loại HĐ"].ToString())
                    {
                        case "NORMAL":
                            tmp_invoice.state_of_bill = 0;
                            break;
                        case "CHANGED":
                            tmp_invoice.state_of_bill = 1;
                            break;
                        case "CONVERTED":
                            tmp_invoice.state_of_bill = 2;
                            break;
                        case "ADJUSTED":
                            tmp_invoice.state_of_bill = 3;
                            break;
                        case "ADJUSTED_INC":
                            tmp_invoice.state_of_bill = 4;
                            break;
                    }
                    tmp_invoice.currency_code = row["Đơn vị tiền tệ"].ToString();
                    tmp_invoice.exchange_rate = tmp_invoice.sub_total = (row["Tỉ giá"].ToString() == "") ? 0 : double.Parse(row["Tỉ giá"].ToString());
                    switch (row["Hình thức thanh toán"].ToString())
                    {
                        case "Tiền mặt":
                            tmp_invoice.method_of_payment = 0;
                            break;
                        case "Chuyển khoản":
                            tmp_invoice.method_of_payment = 1;
                            break;
                        case "Tiền mặt / Chuyển khoản":
                            tmp_invoice.method_of_payment = 2;
                            break;
                    }

                    tmp_invoice.invoice_ref = row["Số chứng từ"].ToString();
                    tmp_invoice.original_invoice = row["Hóa đơn gốc"].ToString();
                    tmp_invoice.sub_total = (row["Tiền trước thuế"].ToString() == "") ? 0 : double.Parse(row["Tiền trước thuế"].ToString());
                    tmp_invoice.vat_rate = int.Parse(row["% VAT"].ToString());
                    tmp_invoice.vat_amount = (row["Tiền thuế VAT"].ToString() == "") ? 0 : double.Parse(row["Tiền thuế VAT"].ToString());
                    tmp_invoice.total_discount = (row["Giảm giá"].ToString() == "") ? 0 : double.Parse(row["Giảm giá"].ToString());
                    tmp_invoice.service_charge_percent = (row["% Phí DV"].ToString() == "") ? 0 : int.Parse(row["% Phí DV"].ToString());
                    tmp_invoice.total_service_charge = (row["Phí dịch vụ"].ToString() == "") ? 0 : double.Parse(row["Phí dịch vụ"].ToString());
                    tmp_invoice.total_amount = (row["Tổng tiền"].ToString() == "") ? 0 : float.Parse(row["Tổng tiền"].ToString());

                    //Item Data Mapping
                    tmp_item_list.Add(new InvoiceAddListDraftData
                    {
                        item_code = (row["Mã sản phẩm"] == null) ? "" : row["Mã sản phẩm"].ToString(),                        
                        item_name = row["Hàng hóa dịch vụ"].ToString(),
                        unit_name = row["Đơn vị tính"].ToString(),
                        unit_price = (row["Đơn giá"].ToString() == "") ? 0 : double.Parse(row["Đơn giá"].ToString()),
                        quantity = (row["Số lượng"].ToString() == "") ? 0 : double.Parse(row["Số lượng"].ToString()),
                        item_total_amount_without_vat = (row["Tiền chưa thuế"].ToString() == "") ? 0 : double.Parse(row["Tiền chưa thuế"].ToString()),
                        vat_percentage = (row["% Thuế VAT"].ToString() == "") ? 0 : int.Parse(row["% VAT"].ToString()),
                        vat_amount = (row["Thuế VAT"].ToString() == "") ? 0 : double.Parse(row["Thuế VAT"].ToString()),
                        //discount_percentage = (row["% Giảm giá"].ToString() == "") ? 0 : int.Parse(row["% Giảm giá"].ToString()),
                        discount_amount = (row["Tiền giảm giá"].ToString() == "") ? 0 : double.Parse(row["Tiền giảm giá"].ToString()),
                        total_amount = (row["Tổng cộng"].ToString() == "") ? 0 : double.Parse(row["Tổng cộng"].ToString()),
                        //current_code = row["Đơn vị tiền tệ"].ToString()
                    });

                    continue;
                }

                //Item Data Mapping
                tmp_item_list.Add(new InvoiceAddListDraftData
                {
                    item_code = row["Mã sản phẩm"].ToString(),
                    item_name = row["Hàng hóa dịch vụ"].ToString(),
                    unit_name = row["Đơn vị tính"].ToString(),
                    unit_price = (row["Đơn giá"].ToString() == "") ? 0 : double.Parse(row["Đơn giá"].ToString()),
                    quantity = (row["Số lượng"].ToString() == "") ? 0 : double.Parse(row["Số lượng"].ToString()),
                    item_total_amount_without_vat = (row["Tiền chưa thuế"].ToString() == "") ? 0 : double.Parse(row["Tiền chưa thuế"].ToString()),
                    vat_percentage = (row["% Thuế VAT"].ToString() == "") ? 0 : int.Parse(row["% Thuế VAT"].ToString()),
                    vat_amount = (row["Thuế VAT"].ToString() == "") ? 0 : double.Parse(row["Thuế VAT"].ToString()),
                    //discount_percentage = (row["% Giảm giá"].ToString() == "") ? 0 : int.Parse(row["% Giảm giá"].ToString()),
                    discount_amount = (row["Tiền giảm giá"].ToString() == "") ? 0 : double.Parse(row["Tiền giảm giá"].ToString()),
                    total_amount = (row["Tổng cộng"].ToString() == "") ? 0 : double.Parse(row["Tổng cộng"].ToString()),
                    //current_code = row["Đơn vị tiền tệ"].ToString()
                });

                //Console.WriteLine("Item num: " + item_count.ToString());
                item_count++;

                //End Row
                if (row.Table.Rows.IndexOf(row) == (import_table.Rows.Count - 1))
                {
                    tmp_invoice.invoice_item_list = Clone(tmp_item_list);
                    invoice_detail_list.Add((InvoiceAddListDraft)tmp_invoice.Clone());
                    //Console.WriteLine("End row of file");
                }
            }

            #endregion

            #region Customer and Product List Processing

            foreach (InvoiceAddListDraft invoice in invoice_detail_list)
            {
                Console.WriteLine(invoice.b_name);
                customer_list.Add(new CustomerAddList
                {
                    email = invoice.b_email,
                    dislay_name = invoice.b_name,
                    tax_code = invoice.b_tax_code,
                    address = invoice.b_address,
                    phone_number = invoice.b_phone_number,
                    fax_number = invoice.b_fax_number,
                    website = invoice.b_website,
                    bank_name = invoice.b_bank_name,
                    bank_number = invoice.b_bank_number,
                    company_name = invoice.b_company,
                });
                foreach (InvoiceAddListDraftData item in invoice.invoice_item_list)
                {
                    product_list.Add(new ProductAddList
                    {
                        item_name = item.item_name,
                        unit_name = item.unit_name,
                        unit_price = item.unit_price,
                        quantity = item.quantity,
                        item_total_amount_without_vat = item.item_total_amount_without_vat,
                        vat_percentage = item.vat_percentage,
                        vat_amount = item.vat_amount,
                        current_code = item.current_code,
                        item_type = item.item_type,
                        discount_amount = item.discount_amount,
                        total_amount = item.total_amount,
                        item_code = item.item_code
                    });
                }
            }

            #endregion

            #region API Import

            InvoiceAddListDraftResponse response_1 = InvoiceList2ServerDraft(invoice_detail_list);

            ProductAddListResponse response_2 = ProductList2Server(product_list);

            CustomerAddListResponse response_3 = CustomerList2Server(customer_list);

            System.Windows.MessageBox.Show(response_3.message.ToString());

            #endregion

            return invoice_detail_list;
        }

        private static InvoiceAddListDraftResponse InvoiceList2ServerDraft(List<InvoiceAddListDraft> invoice_list)
        {
            var api = new InvoiceRestApi();
            InvoiceAddListDraftResponse response = api.AddListDraft(invoice_list);
            return response;
        }

        public static void InvoiceImportExcelKeep()
        {
            throw new NotImplementedException();
        }

        private static List<InvoiceAddListDraft> DataTable2ServerKeep(DataTable import_table)
        {
            throw new NotImplementedException();
        }

        private static InvoiceAddListDraftResponse InvoiceList2ServerKeep(List<InvoiceAddListDraft> invoice_list)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Invoice Export Related Excel

        public static DataTable InvoiceListToDataTableDetail(Invoice_Detail_List[] list)
        {
            DataTable table = new DataTable();

            //Table type mapping
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Tên khách hàng", typeof(string));
            table.Columns.Add("Tên công ty", typeof(string));
            table.Columns.Add("Mã số thuế", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Số điện thoại", typeof(string));
            table.Columns.Add("Số Fax", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Website", typeof(string));
            table.Columns.Add("Số TK ngân hàng", typeof(string));
            table.Columns.Add("Tên ngân hàng", typeof(string));

            table.Columns.Add("Mẫu HĐ", typeof(string));
            table.Columns.Add("Ký hiệu HĐ", typeof(string));
            table.Columns.Add("Số HĐ", typeof(int));
            table.Columns.Add("Ngày HĐ", typeof(string));
            table.Columns.Add("Trạng thái", typeof(string));
            table.Columns.Add("Loại HĐ", typeof(string));
            table.Columns.Add("Đơn vị tiền tệ", typeof(string));
            table.Columns.Add("Tỉ giá", typeof(double));
            table.Columns.Add("Hình thức thanh toán", typeof(string));
            table.Columns.Add("Số chứng từ", typeof(string));
            table.Columns.Add("Hóa đơn gốc", typeof(string));

            table.Columns.Add("Tiền trước thuế", typeof(double));
            table.Columns.Add("% VAT", typeof(int));            
            table.Columns.Add("Tiền thuế VAT", typeof(double));
            table.Columns.Add("Giảm giá", typeof(double));
            table.Columns.Add("% Phí DV", typeof(int));
            table.Columns.Add("Phí dịch vụ", typeof(double));
            table.Columns.Add("Tổng tiền", typeof(double));

            table.Columns.Add("STT", typeof(int));
            table.Columns.Add("Mã sản phẩm", typeof(string));
            table.Columns.Add("Hàng hóa dịch vụ", typeof(string));
            table.Columns.Add("Đơn vị tính", typeof(string));
            table.Columns.Add("Đơn giá", typeof(double));
            table.Columns.Add("Số lượng", typeof(double));
            table.Columns.Add("Tiền chưa thuế", typeof(double));
            table.Columns.Add("% Thuế VAT", typeof(int));
            table.Columns.Add("Thuế VAT", typeof(double));
            table.Columns.Add("% Giảm giá", typeof(int));
            table.Columns.Add("Tiền giảm giá", typeof(double));
            table.Columns.Add("Tổng cộng", typeof(double));

            //int cnt_row = 1;
            for (int i = 0; i < list.Length; i++)
            {
                DataRow row = table.NewRow();
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 7, 0, 0, 0, System.DateTimeKind.Utc);
                //Table Mapping
                row["ID"] = i + 1;
                row["Tên khách hàng"] = list[i].b_name;
                row["Tên công ty"] = list[i].b_company;
                row["Mã số thuế"] = list[i].b_tax_code;
                row["Địa chỉ"] = list[i].b_address;
                row["Số điện thoại"] = list[i].b_phone_number;
                row["Số Fax"] = list[i].b_fax_number;
                row["Email"] = list[i].b_email;
                row["Website"] = list[i].b_website;
                row["Số TK ngân hàng"] = list[i].b_bank_number;
                row["Tên ngân hàng"] = list[i].b_bank_number;

                row["Mẫu HĐ"] = list[i].invoice_form_name;
                row["Ký hiệu HĐ"] = list[i].invoice_serial_name;
                row["Số HĐ"] = list[i].invoice_number;
                row["Ngày HĐ"] = dtDateTime.AddSeconds(list[i].invoice_sign_date).ToLocalTime().ToString().Substring(0, 10);
                if (list[i].status == 1)
                    row["Trạng thái"] = "SIGNED";
                switch (list[i].state_of_bill)
                {
                    case 0:
                        row["Loại HĐ"] = "NORMAL";
                        break;
                    case 1:
                        row["Loại HĐ"] = "CHANGED";
                        break;
                    case 2:
                        row["Loại HĐ"] = "CONVERTED";
                        break;
                    case 3:
                        row["Loại HĐ"] = "ADJUSTED";
                        break;
                    case 4:
                        row["Loại HĐ"] = "ADJUSTED_INC";
                        break;
                }
                row["Đơn vị tiền tệ"] = list[i].currency_code;
                row["Tỉ giá"] = list[i].exchange_rate;
                switch (list[i].method_of_payment)
                {
                    case 0:
                        row["Hình thức thanh toán"] = "Tiền mặt";
                        break;
                    case 1:
                        row["Hình thức thanh toán"] = "Chuyển khoản";
                        break;
                    case 2:
                        row["Hình thức thanh toán"] = "Tiền mặt / Chuyển khoản";
                        break;
                }
                row["Số chứng từ"] = list[i].invoice_ref;
                row["Hóa đơn gốc"] = list[i].original_invoice;

                row["Tiền trước thuế"] = list[i].sub_total;
                row["% VAT"] = list[i].vat_rate;                
                row["Tiền thuế VAT"] = list[i].vat_amount;
                row["Giảm giá"] = list[i].total_discount;
                row["% Phí DV"] = list[i].service_charge_percent;
                row["Phí dịch vụ"] = list[i].total_service_charge;
                row["Tổng tiền"] = list[i].total_amount;        

                row["STT"] = 1;
                if (list[i].invoice_item_list != null && list[i].invoice_item_list.Length > 0)
                {
                    row["Mã sản phẩm"] = (list[i].invoice_item_list[0].item_code == null) ? "" : list[i].invoice_item_list[0].item_code.ToString();
                    row["Hàng hóa dịch vụ"] = list[i].invoice_item_list[0].item_name;
                    row["Đơn vị tính"] = list[i].invoice_item_list[0].unit_name;
                    row["Đơn giá"] = list[i].invoice_item_list[0].unit_price;
                    row["Số lượng"] = list[i].invoice_item_list[0].quantity;
                    row["Tiền chưa thuế"] = list[i].invoice_item_list[0].item_total_amount_without_vat;
                    row["% Thuế VAT"] = list[i].invoice_item_list[0].vat_percentage;
                    row["Thuế VAT"] = list[i].invoice_item_list[0].vat_amount;
                    //row["% Giảm giá"] = list[i].invoice_item_list[0];
                    row["Tiền giảm giá"] = list[i].invoice_item_list[0].discount_amount;
                    row["Tổng cộng"] = list[i].invoice_item_list[0].total_amount;
                }

                table.Rows.Add(row);

                //Item Mapping
                for (int j = 1; j < list[i].invoice_item_list.Length; j++)
                {
                    DataRow rw = table.NewRow();
                    rw["ID"] = i + 1;
                    rw["STT"] = j + 1;
                    rw["Mã sản phẩm"] = (list[i].invoice_item_list[j].item_code == null) ? "" : list[i].invoice_item_list[j].item_code.ToString();
                    rw["Hàng hóa dịch vụ"] = list[i].invoice_item_list[j].item_name.ToString();
                    rw["Đơn vị tính"] = list[i].invoice_item_list[j].unit_name;
                    rw["Đơn giá"] = list[i].invoice_item_list[j].unit_price;
                    rw["Số lượng"] = list[i].invoice_item_list[j].quantity;
                    rw["Tiền chưa thuế"] = list[i].invoice_item_list[j].item_total_amount_without_vat;
                    rw["% Thuế VAT"] = list[i].invoice_item_list[j].vat_percentage;
                    rw["Thuế VAT"] = list[i].invoice_item_list[j].vat_amount;
                    //rw["% Giảm giá"] = list[i].invoice_item_list[0];
                    rw["Tiền giảm giá"] = list[i].invoice_item_list[j].discount_amount;
                    rw["Tổng cộng"] = list[i].invoice_item_list[j].total_amount;
                    table.Rows.Add(rw);
                }
            }

            return table;
        }

        public static DataTable InvoiceListToDataTableNormal(Invoice_Detail_List[] list)
        {
            DataTable table = new DataTable();

            //Table type mapping
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Tên khách hàng", typeof(string));
            table.Columns.Add("Tên công ty", typeof(string));
            table.Columns.Add("Mã số thuế", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Số điện thoại", typeof(string));
            table.Columns.Add("Số Fax", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Website", typeof(string));
            table.Columns.Add("Số TK ngân hàng", typeof(string));
            table.Columns.Add("Tên ngân hàng", typeof(string));

            table.Columns.Add("Mẫu HĐ", typeof(string));
            table.Columns.Add("Ký hiệu HĐ", typeof(string));
            table.Columns.Add("Số HĐ", typeof(int));
            table.Columns.Add("Ngày HĐ", typeof(string));
            table.Columns.Add("Trạng thái", typeof(string));
            table.Columns.Add("Loại HĐ", typeof(string));
            table.Columns.Add("Đơn vị tiền tệ", typeof(string));
            table.Columns.Add("Tỉ giá", typeof(double));
            table.Columns.Add("Hình thức thanh toán", typeof(string));
            table.Columns.Add("Số chứng từ", typeof(string));
            table.Columns.Add("Hóa đơn gốc", typeof(string));

            table.Columns.Add("Tiền trước thuế", typeof(double));
            table.Columns.Add("% VAT", typeof(int));
            table.Columns.Add("Tiền thuế VAT", typeof(double));
            table.Columns.Add("Giảm giá", typeof(double));
            table.Columns.Add("% Phí DV", typeof(int));
            table.Columns.Add("Phí dịch vụ", typeof(double));
            table.Columns.Add("Tổng tiền", typeof(double));

            //int cnt_row = 1;
            for (int i = 0; i < list.Length; i++)
            {
                DataRow row = table.NewRow();
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 7, 0, 0, 0, System.DateTimeKind.Utc);
                //Table Mapping
                row["ID"] = i + 1;
                row["Tên khách hàng"] = list[i].b_name;
                row["Tên công ty"] = list[i].b_company;
                row["Mã số thuế"] = list[i].b_tax_code;
                row["Địa chỉ"] = list[i].b_address;
                row["Số điện thoại"] = list[i].b_phone_number;
                row["Số Fax"] = list[i].b_fax_number;
                row["Email"] = list[i].b_email;
                row["Website"] = list[i].b_website;
                row["Số TK ngân hàng"] = list[i].b_bank_number;
                row["Tên ngân hàng"] = list[i].b_bank_number;

                row["Mẫu HĐ"] = list[i].invoice_form_name;
                row["Ký hiệu HĐ"] = list[i].invoice_serial_name;
                row["Số HĐ"] = list[i].invoice_number;
                row["Ngày HĐ"] = dtDateTime.AddSeconds(list[i].invoice_sign_date).ToLocalTime().ToString().Substring(0, 10);
                if (list[i].status == 1)
                    row["Trạng thái"] = "SIGNED";
                switch (list[i].state_of_bill)
                {
                    case 0:
                        row["Loại HĐ"] = "NORMAL";
                        break;
                    case 1:
                        row["Loại HĐ"] = "CHANGED";
                        break;
                    case 2:
                        row["Loại HĐ"] = "CONVERTED";
                        break;
                    case 3:
                        row["Loại HĐ"] = "ADJUSTED";
                        break;
                    case 4:
                        row["Loại HĐ"] = "ADJUSTED_INC";
                        break;
                }
                row["Đơn vị tiền tệ"] = list[i].currency_code;
                row["Tỉ giá"] = list[i].exchange_rate;
                switch (list[i].method_of_payment)
                {
                    case 0:
                        row["Hình thức thanh toán"] = "Tiền mặt";
                        break;
                    case 1:
                        row["Hình thức thanh toán"] = "Chuyển khoản";
                        break;
                    case 2:
                        row["Hình thức thanh toán"] = "Tiền mặt / Chuyển khoản";
                        break;
                }
                row["Số chứng từ"] = list[i].invoice_ref;
                row["Hóa đơn gốc"] = list[i].original_invoice;

                row["Tiền trước thuế"] = list[i].sub_total;
                row["% VAT"] = list[i].vat_rate;
                row["Tiền thuế VAT"] = list[i].vat_amount;
                row["Giảm giá"] = list[i].total_discount;
                row["% Phí DV"] = list[i].service_charge_percent;
                row["Phí dịch vụ"] = list[i].total_service_charge;
                row["Tổng tiền"] = list[i].total_amount;

                table.Rows.Add(row);
            }

            return table;
        }



        #endregion

        #region Others

        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        #endregion
    }
}
