using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public enum DatabaseConnectionName
    {
        Identity,
        OPAL,
        Test
    }

    public interface IConnectionFactory
    {
        SqlConnection CreateConnection(DatabaseConnectionName connectionName);
    }
    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly IDictionary<DatabaseConnectionName, string> _connectionDict;
   
        public SqlConnectionFactory(IDictionary<DatabaseConnectionName, string> connectionDict)
        {
            _connectionDict = connectionDict;
        }
        public SqlConnection CreateConnection(DatabaseConnectionName connectionName)
        {
            string connectionString = null;
            if (_connectionDict.TryGetValue(connectionName, out connectionString))
            {
                return new SqlConnection(connectionString);
            }

            throw new ArgumentNullException();
        }
    }
}
