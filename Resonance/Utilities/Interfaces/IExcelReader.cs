namespace ResoClassAPI.Utilities.Interfaces
{
    public interface IExcelReader
    {
        Task<bool> BulkUpload(IFormFile file, string tableName);
    }
}
