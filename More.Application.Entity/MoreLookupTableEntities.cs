using System;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace More.Application.Entity
{
    public partial class MoreLookupTableEntities
    {
        public int ExecuteNonQuery(string query)
        {
            using (var connection = new SqlConnection((Connection as EntityConnection).StoreConnection.ConnectionString + ";password=xpress"))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                var result = command.ExecuteNonQuery();
                connection.Close();
                return result;
            }
        }
        public void ExecuteBatchNonQuery(string[] queries, Action<int,string> queryExecuted = null )
        {
          
                foreach (var query in queries)
                {

                    try
                    {
                        var result = ExecuteNonQuery(query);
                        if (queryExecuted != null)
                            queryExecuted(result, query);
                    } catch (Exception ex)
                    {
                        if (queryExecuted != null)
                            queryExecuted(0, ex.Message);
                    }
                    
                }
            
        }
    }
}
