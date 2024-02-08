using OfficeOpenXml;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities.Interfaces;
using System.Data;

namespace ResoClassAPI.Utilities
{
    public class ExcelReader : IExcelReader
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IAuthService authService;
        private readonly ICommonService commonService;

        public ExcelReader(IWebHostEnvironment _hostingEnvironment, IAuthService _authService
            ,ICommonService _commonService)
        {
            hostingEnvironment = _hostingEnvironment;
            authService = _authService;
            commonService = _commonService;
        }

        public async Task<bool> BulkUpload(IFormFile file, string tableName)
        {
            bool isUploaded = false;
            string filePath = string.Empty;
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var foreignKeyColumns = await commonService.GetForeignKeyColumns(tableName);
                    DataTable dataTable = ReadExcelToDataTable(stream, tableName, foreignKeyColumns);
                    if (dataTable != null && dataTable.Rows.Count > 0)
                        isUploaded = await commonService.SaveDataToDatabase(tableName, dataTable, foreignKeyColumns);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return isUploaded;
        }

        private DataTable ReadExcelToDataTable(Stream stream, string tableName, List<string> foreignKeyColumns)
        {
            var currentUser = authService.GetCurrentUser();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dataTable = new DataTable();

                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    if (foreignKeyColumns != null && foreignKeyColumns.Contains(firstRowCell.Text.Trim() + "Id"))
                        dataTable.Columns.Add(firstRowCell.Text.Trim() + "Id");
                    else
                        dataTable.Columns.Add(firstRowCell.Text.Trim());
                }
                if (tableName == SqlTableName.User)
                {
                    dataTable.Columns.Add("IsActive");
                    dataTable.Columns.Add("CreatedBy");
                    dataTable.Columns.Add("CreatedOn");
                    dataTable.Columns.Add("ModifiedBy");
                    dataTable.Columns.Add("ModifiedOn");
                }

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    DataRow newRow = dataTable.Rows.Add();

                    foreach (var cell in row)
                    {                        
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }

                    if (tableName == SqlTableName.User)
                    {
                        newRow["IsActive"] = true;
                        newRow["CreatedBy"] = currentUser.Name;
                        newRow["CreatedOn"] = DateTime.Now.ToString();
                        newRow["ModifiedBy"] = currentUser.Name;
                        newRow["ModifiedOn"] = DateTime.Now.ToString();
                    }
                }

                return dataTable;
            }
        }

    }
}
