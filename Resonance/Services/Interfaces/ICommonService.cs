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

        Task<string> LogError(Type entityType, string message, string stackTrace, string exceptionType);
        int GetCorrectAnswersCountBySubjectId(long subjectId);
        int GetTotalQUestionsCountBySubjectId(long subjectId);
    }
}
