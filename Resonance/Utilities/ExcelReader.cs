using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using OfficeOpenXml;
using ResoClassAPI.Utilities.Interfaces;
using System.Data;

namespace ResoClassAPI.Utilities
{
    public class ExcelReader : IExcelReader
    {
        private IConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ExcelReader(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration; 
            _hostingEnvironment = hostingEnvironment;
        }

        public bool BulkUpload(IFormFile file, string tableName)
        {
            bool isUploaded = false;
            string uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            DataTable dataTable = ReadExcelToDataTable(filePath);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                string connectionString = _config["ConnectionStrings:SqlConnectionString"];
                SaveDataToDatabase(tableName, dataTable, connectionString);
                isUploaded = true;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return isUploaded;
        }

        private DataTable ReadExcelToDataTable(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dataTable = new DataTable();

                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dataTable.Columns.Add(firstRowCell.Text.Trim());
                }

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    DataRow newRow = dataTable.Rows.Add();

                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }
                }

                return dataTable;
            }
        }

        private bool SaveDataToDatabase(string tableName, DataTable dataTable, string connectionString)
        {
            bool isUpdated = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }

                        bulkCopy.WriteToServer(dataTable);
                        isUpdated = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving data to the database: " + ex.Message);
                }
                return isUpdated;
            }
        }
    }
}
