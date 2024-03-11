using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Data;

namespace ResoClassAPI.Services
{
    public class CommonService : ICommonService
    {
        private IConfiguration config;
        private readonly ResoClassContext dbContext;
        public CommonService(ResoClassContext _dbContext, IConfiguration configuration)
        {
            config = configuration;
            this.dbContext = _dbContext;
       }

        public async Task<bool> SaveDataToDatabase(string tableName, DataTable dataTable, List<string> foreignKeyColumns)
        {
            bool isUpdated = false;
            using (SqlConnection connection = new SqlConnection(config["ConnectionStrings:SqlConnectionString"]))
            {
                try
                {
                    connection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;

                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (foreignKeyColumns != null && foreignKeyColumns.Count > 0)
                            {
                                foreach (string item in foreignKeyColumns)
                                {
                                    if (row[item] != null)
                                    {
                                        long foreignKeyId = GetForeignKeyId(connection, item.Substring(0, item.Length - 2), row[item].ToString());

                                        if (tableName == "Video")
                                        {
                                            if(foreignKeyId > 0)
                                                row[item] = foreignKeyId;
                                            else
                                                row[item] = null;
                                        }
                                        else
                                        {
                                            if (foreignKeyId > 0)
                                                row[item] = foreignKeyId;
                                            else
                                                throw new Exception($"Foreign key value '{row[item]}' not found in the parent table.");
                                        }
                                    }
                                }
                            }
                        }

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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();

                    throw new Exception("Error saving data to the database: " + ex.Message);
                }
                return isUpdated;
            }
        }
       
        public async Task<List<string>> GetForeignKeyColumns(string tableName)
        {
            List<string> foreignKeyColumns = new List<string>();
            string connectionString = config["ConnectionStrings:SqlConnectionString"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query to fetch foreign key columns for the specified table
                string query = @"
                SELECT COLUMN_NAME 
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsForeignKey') = 1 
                AND TABLE_NAME = @TableName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader.GetString(0);
                            foreignKeyColumns.Add(columnName);
                        }
                    }
                }
            }

            return foreignKeyColumns;
        }

        public int GetTotalQUestionsCountBySubjectId(long  subjectId)
        {
            int count = 0;
            string connectionString = config["ConnectionStrings:SqlConnectionString"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"Select COUNT(AQ.ID) from AssessmentSession_Questions AQ
                                Inner Join Chapter C ON AQ.ChapterId = C.Id
                                Inner Join Subject S ON C.SubjectId = S.Id
                                Where S.Id = " + subjectId;

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            count = reader.GetInt32(0);
                    }
                }
            }

            return count;
        }

        public int GetCorrectAnswersCountBySubjectId(long subjectId)
        {
            int count = 0;
            string connectionString = config["ConnectionStrings:SqlConnectionString"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"Select Count(AQ.ID) from AssessmentSession_Questions AQ
                            Inner Join Chapter C ON AQ.ChapterId = C.Id
                            Inner Join Subject S ON C.SubjectId = S.Id
                            Where AQ.Result IS not NUll and AQ.Result = 1 And S.Id = " + subjectId;

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            count = reader.GetInt32(0);
                    }
                }
            }

            return count;
        }

        private long GetForeignKeyId(SqlConnection connection, string tableName, string value)
        {
            using (SqlCommand command = new SqlCommand($"SELECT Id FROM {tableName} WHERE Name = @Value", connection))
            {
                command.Parameters.AddWithValue("@Value", value);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt64(result) : -1; // Assuming IDs are integers and -1 represents not found
            }
        }

        public async Task<List<ListItemDto>> GetListItems(string tableName, string parentName, long? parentId)
        {
            List<ListItemDto> listItems = new List<ListItemDto>();
            string connectionString = config["ConnectionStrings:SqlConnectionString"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = string.Empty;

                if (!string.IsNullOrEmpty(parentName) && parentId > 0)
                {
                    query = "Select Id, Name from " + tableName + " Where " + parentName + "Id" + " = " + parentId;
                }
                else
                {
                    query = "Select Id, Name from " + tableName;
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //command.Parameters.AddWithValue("@TableName", tableName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListItemDto item = new ListItemDto()
                            {
                                Id = reader.GetInt64(0),
                                Name = reader.GetString(1)
                            };
                            listItems.Add(item);
                        }
                    }
                }
            }

            return listItems;
        }

        public async Task<string> LogError(Type entityType, string message, string stackTrace, string exceptionType)
        {
            string referenceNumber = string.Empty;
            try
            {
                referenceNumber = Guid.NewGuid().ToString();
                Logger log = new Logger();
                log.ReferenceNumber = referenceNumber;
                log.Message = message;
                log.LogType = "Error";
                log.StackTrace = stackTrace;
                log.EntityName = "";
                log.ExceptionType = exceptionType;
                log.CreateOn = DateTime.Now;

                dbContext.Loggers.Add(log);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return referenceNumber;
        }

    }
}
