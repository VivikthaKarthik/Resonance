using Microsoft.Data.SqlClient;
using ResoClassAPI.DTOs;
using System.Data;

namespace ResoClassAPI.Services.Interfaces
{
    public interface ICommonService
    {
        Task<bool> SaveDataToDatabase(string tableName, DataTable dataTable, List<string> foreignKeyColumns);
        Task<List<string>> GetForeignKeyColumns(string tableName);
        Task<List<ListItemDto>> GetListItems(string tableName, string parentName, long? parentId);
    }
}
