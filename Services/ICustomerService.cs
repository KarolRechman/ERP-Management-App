using NewOPAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public interface ICustomerService
    {
        List<Words> GetHerstellers(int IDMANDANT, int IDUSER, int? IDMENU);
    }

    public class CustomerService : ICustomerService
    {
        private ICacheService cacheService;
        private IConnectionFactory connectionFactory;
        private SqlConnection con;
        private readonly DatabaseConnectionName DBname = DatabaseConnectionName.OPAL;

        public CustomerService(IConnectionFactory connection, ICacheService cache)
        {
            connectionFactory = connection;
            cacheService = cache;
        }

        public List<Words> GetHerstellers(int IDMANDANT, int IDUSER, int? IDMENU)
        {
            var item = new List<Words>();
            var parameters = new { IDMANDANT, IDUSER, IDMENU, NewApp = true };
            return cacheService.GetData("LIST_HERSTELLER", DBname, item, parameters).ToList();
        }

    }
}


