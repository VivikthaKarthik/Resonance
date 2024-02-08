using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ResoClassAPI.Services.Interfaces;
using System.Data;

namespace ResoClassAPI.Services
{
    public class CommonService : ICommonService
    {
        private IConfiguration config;
        public CommonService(IConfiguration configuration)
        {
            config = configuration;
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
                                        if (foreignKeyId > 0)
                                            row[item] = foreignKeyId;
                                        else
                                            throw new Exception($"Foreign key value '{row[item]}' not found in the parent table.");
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

        private long GetForeignKeyId(SqlConnection connection, string tableName, string value)
        {
            using (SqlCommand command = new SqlCommand($"SELECT Id FROM {tableName} WHERE Name = @Value", connection))
            {
                command.Parameters.AddWithValue("@Value", value);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt64(result) : -1; // Assuming IDs are integers and -1 represents not found
            }
        }

    }
}
