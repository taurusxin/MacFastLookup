using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;

namespace MacFastLookup
{
    public class SQLiteHelper
    {
        private readonly string connectionString;
        private readonly SQLiteConnection connection;

        public SQLiteHelper(string dbFilePath)
        {
            connectionString = $"Data Source={dbFilePath};Version=3;";
            connection = new SQLiteConnection(connectionString);
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public MacAddress QueryMacAddressByPrefix(string prefix)
        {
            string TableName = "MacAddresses";
            string query = $"SELECT * FROM {TableName} WHERE Prefix = @prefix";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@prefix", prefix);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        MacAddress macAddress = new MacAddress();
                        macAddress.Id = reader["Id"].ToString();
                        macAddress.Prefix = reader["Prefix"].ToString();
                        macAddress.VendorName = reader["VendorName"].ToString();
                        macAddress.Private = Convert.ToBoolean(reader["Private"]);
                        macAddress.BlockType = reader["BlockType"].ToString();
                        macAddress.LastUpdate = reader["LastUpdate"].ToString();
                        return macAddress;
                    }
                }
            }
            return null;
        }

        private string QuerySingleColumn(string tableName, string columnName)
        {
            string result = null;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = $"SELECT {columnName} FROM {tableName} LIMIT 1";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = reader[columnName].ToString();
                        }
                    }
                }
            }

            return result;
        }

        public string QueryVersion()
        {
            return QuerySingleColumn("Config", "Version");
        }
    }
}
