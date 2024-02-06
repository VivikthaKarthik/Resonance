namespace ResoClassAPI.Utilities.Interfaces
{
    public interface IExcelReader
    {
        bool BulkUpload(IFormFile file, string tableName);
    }
}
