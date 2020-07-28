using Dapper;
using Microsoft.EntityFrameworkCore.Internal;
using NewOPAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public interface IPartsService
    {
        List<Words> GetPartConditions(DynamicParameters parameters);
        List<Words> GetWarehouseLocations(int IDMANDANT, int IDMENU, int IDUSER, int? IdBranch);
        List<Words> GetPartConditions(int IDMANDANT, int IDMENU, int IDCUSTOMER);
        List<dynamic> SparepartTransferSelect(DynamicParameters parameters);
        //List<ExpandoObject> SparepartTransferSelect();
    }

    public class PartsService : IPartsService
    {
        private ICacheService cacheService;
        private IConnectionFactory connectionFactory;
        private SqlConnection con;
        private readonly DatabaseConnectionName DBname = DatabaseConnectionName.OPAL;

        public PartsService(IConnectionFactory connection, ICacheService cache)
        {
            connectionFactory = connection;
            cacheService = cache;
        }

        public List<Words> GetPartConditions(int IDMANDANT, int IDMENU, int IDCUSTOMER)
        {
            var item = new List<dynamic>();
            var parameters = new { IDMANDANT, IDMENU, IDCUSTOMER };
            return cacheService.GetData("usp_PartCondition_Select", DBname, item, parameters).ToWords();
        }

        public List<Words> GetPartConditions(DynamicParameters parameters)
        {
            var item = new List<dynamic>();
            return cacheService.GetData("usp_PartCondition_Select", DBname, item, parameters).ToWords();
        }

        public List<Words> GetWarehouseLocations(int IDMANDANT, int IDMENU, int IDUSER, int? IdBranch)
        {
            var item = new List<dynamic>();
            var parameters = new { IDMANDANT, IDMENU, IDUSER, IdBranch };
            return cacheService.GetData("List_LagerOrt", DBname, item, parameters).ToWords();
        }

        public List<dynamic> SparepartTransferSelect(DynamicParameters parameters)
        {
            var sql = "usp_ProdParts_Transfer_Select";
            con = connectionFactory.CreateConnection(DBname);
            using (con)
            {
                con.Open();
                parameters.Add("@NewApp", true, DbType.Boolean);
                return con.Query<dynamic>(sql, parameters, commandType: CommandType.StoredProcedure).ToList(); 
            }
        }
    }
}
