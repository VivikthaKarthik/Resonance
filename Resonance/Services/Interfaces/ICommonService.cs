using Microsoft.Data.SqlClient;
using System.Data;

namespace ResoClassAPI.Services.Interfaces
{
    public interface ICommonService
    {
        Task<bool> SaveDataToDatabase(string tableName, DataTable dataTable, List<string> foreignKeyColumns);
        Task<List<string>> GetForeignKeyColumns(string tableName);
    }
}
