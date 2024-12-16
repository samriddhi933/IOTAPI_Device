using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Text;




public class GlobalModel
{
    public static bool IsCronJobRunning { get; set; } = false;
    public static string ConnectionString { get; set; }
}

namespace IOTapi.dbcontext
{
    public class DataContextDB
    {


        public static string ExecuteRowSqlCommand(string query, object[] parameters = null)
        {
            try
            {
                using (var connection = new MySqlConnection(GlobalModel.ConnectionString))
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                command.Parameters.AddWithValue($"@p{i}", parameters[i]);
                            }
                        }

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            var result = new StringBuilder();
                            while (reader.Read())
                            {
                                // Process each row here
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Append(reader[i].ToString() + ", ");
                                }
                                result.AppendLine();
                            }
                            return result.ToString(); // Return your result here
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static string ExecuteQueryDynamicDataset(string spQuery)
        {
            string output = string.Empty;

            using (MySqlConnection con = new MySqlConnection(GlobalModel.ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(spQuery, con))
                {
                   
                    //cmd.CommandTimeout = Int32.MaxValue;
                    cmd.CommandType = CommandType.Text;

                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        DataSet dt = new DataSet();
                        sda.Fill(dt);

                        output = DataSetToJSONWithJSONNet(dt);
                    }
                }
            }

            return output;
        }

        public static string DataSetToJSONWithJSONNet(DataSet table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }

    }
}
