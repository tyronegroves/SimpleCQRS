using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SimpleCqrs.EventStore.SqlServer
{
    public class SqlCreator
    {
        public string Create(string baseSql, params object[] parameters)
        {
            return string.Format(baseSql, parameters);
        }

    }

    public class SqlRunner
    {
        public void ExecuteCommand(SqlServerConfiguration configuration, string sql) {
            using (var connection = new SqlConnection(configuration.ConnectionString)) {
                connection.Open();
                using (var command = new SqlCommand(sql, connection)) {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public DataRowCollection ExecuteQuery(SqlServerConfiguration configuration, string sql) {
            var dataSet = new DataSet();
            using (var connection = new SqlConnection(configuration.ConnectionString)) {
                connection.Open();

                using (var command = new SqlCommand(sql, connection)) {
                    using (var dataAdapter = new SqlDataAdapter(command)) {
                        dataAdapter.Fill(dataSet);
                    }
                }

                connection.Close();
            }
            return dataSet.Tables[0].Rows;
        }
    }
}
