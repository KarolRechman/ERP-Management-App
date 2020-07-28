using Dapper;
using Microsoft.Extensions.Configuration;
using NewOPAL.Models;
using NewOPAL.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public interface IDataAccess
    {
        //IDbConnection GetCon();
        List<OpalUser> ImportUsers();
        void UpdateUsers();
        //List<Words> Translate(int id, List<Words> words);
        //List<Language> GetLanguages();
    }

    public class DataAccess : IDataAccess
    {
        IConnectionFactory _connectionFactory;
        private SqlConnection con;
        public DataAccess(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Retrieves connection string from "appsettings.json" file
        /// </summary>
        /// <returns>SqlConnection object</returns>
        //public IDbConnection GetCon()
        //{
        //    return _connectionFactory.CreateConnection();
        //}

        public List<OpalUser> ImportUsers()
        {
            var sql = "Import_Users";
            List<OpalUser> users = new List<OpalUser>();

            con = _connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            using (con)
            {
                con.Open();

                users = con.Query<OpalUser>(sql, commandType: CommandType.StoredProcedure).ToList();

                return users;
            }
        }

        public void UpdateUsers()
        {

            var sql = "Import_Users";
            var users = new List<OpalUser>();


            con = _connectionFactory.CreateConnection(DatabaseConnectionName.Test);
            using (con)
            {
                con.Open();

                users = con.Query<OpalUser>(sql, commandType: CommandType.StoredProcedure).ToList();
            }



            sql = "UpdateUsers";
            foreach (var user in users)
            {
                con = _connectionFactory.CreateConnection(DatabaseConnectionName.Identity);
                using (con)
                {
                    con.Open();
                    var parameters = new { IDUSER = user.Iduser, IDMandant = user.IdMandant, user.PersonalNumber, user.FirstName, user.LastName };
                    con.Execute(sql, parameters, commandType: CommandType.StoredProcedure);
                }
            }          
        }
    }
}
