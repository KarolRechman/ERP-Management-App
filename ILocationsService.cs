using Dapper;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public interface ILocationsService
    {
        string GetBranchIdentity(int IdMandant, int IdUser, string IpAddress = null);
    }
    public class LocationsService : ILocationsService
    {
        private readonly IMemoryCache MemoryCache;
        private ICacheService cacheService;
        private IConnectionFactory connectionFactory;
        private SqlConnection con;
        private readonly DatabaseConnectionName DBname = DatabaseConnectionName.OPAL;

        public LocationsService(IConnectionFactory connection, ICacheService cache, IMemoryCache _memoryCache)
        {
            connectionFactory = connection;
            cacheService = cache;
            MemoryCache = _memoryCache;
        }

        public string GetBranchIdentity(int IdMandant, int IdUser, string IpAddress = null)
        {
            if (MemoryCache.TryGetValue("BranchIdentity_BranchName", out string BranchNameCache))
            {
                return BranchNameCache;
            }

            var sql = "BranchIdentity";
            con = connectionFactory.CreateConnection(DBname);
            using (con)
            {
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@IDMANDANT", IdMandant, DbType.Int32);
                _params.Add("@IDUSER", IdUser, DbType.Int32);
                _params.Add("@BranchName", " ", DbType.String, direction: ParameterDirection.Output);
                _params.Add("@IdBranch", SqlDbType.Int, direction: ParameterDirection.Output);
                _params.Add("@IdServiceCenter", SqlDbType.Int, direction: ParameterDirection.Output);
                _params.Add("@IdMandantForBranch", SqlDbType.Int, direction: ParameterDirection.Output);
                _params.Add("@HostIpAddress", IpAddress);
                con.Execute(sql, _params, commandType: CommandType.StoredProcedure);
                var result = _params.Get<string>("BranchName");
                MemoryCache.Set("BranchIdentity_BranchName", result, TimeSpan.FromMinutes(60));
                return result;
            }
        }
    }
}