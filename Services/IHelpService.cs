using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using NewOPAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public interface IHelpService
    {
        Tuple<string, List<Coupons>> GetCoupons(int IdMandant, int IdUser, string IpAddress = null, string Action = "SelectCoupons");
        string SendCoupons(int IdMandant, int IdUser, string IpAddress, dynamic coupons, string Action = "SendCoupon");
        bool DontShow(int IdMandant, int IdUser, string Action = "DoNotShowCoupon");
        bool UserInfo(int IdUser, string Info);
        bool UserInfoConfirmation(int IdUser, string Info);
        Modal GetModalTxt(string Action, int IdLanguage);
        Table GetHolidays(int IdMandant, int IdUser, string Action, int Year, int IdCustomer);
        Table GetHolidays(DynamicParameters parameters);
        string InsertHoliday(DynamicParameters parameters);
        string DeleteHolidays(DynamicParameters parameters);
        string UpdateHoliday(DynamicParameters parameters);
        DynamicParameters SetParameters(ExpandoObject dynamic, bool idsTable = false);
        DataTable CreateTableValuedParameter(object value, bool idsTable = false);

        //string GetBranchIdentity(int IdMandant, int IdUser, string IpAddress = null);
    }
    public class HelpService : IHelpService
    {
        private IConnectionFactory connectionFactory;
        private IDbConnection con;
        private HttpContext httpContext;
        public HelpService(IConnectionFactory connection, IHttpContextAccessor httpContextAccessor)
        {
            connectionFactory = connection;
            httpContext = httpContextAccessor.HttpContext;
        }

        public Tuple<string, List<Coupons>> GetCoupons(int IdMandant, int IdUser, string IpAddress = null, string Action = "SelectCoupons")
        {
            var coupons = new List<Coupons>();
            string coupontxt = "";
            var LastLanguageId = httpContext.Session.GetInt32("language");
            var sql = "usp_Coupon";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            using (con)
            {
                con.Open();

                DynamicParameters _params = new DynamicParameters();
                _params.Add("@Status", SqlDbType.VarChar, direction: ParameterDirection.InputOutput);
                _params.Add("@IdMandant", IdMandant);
                _params.Add("@IdUser", IdUser);
                _params.Add("@Action", Action);
                _params.Add("@IpAddress", IpAddress);
                _params.Add("@IdLanguage", LastLanguageId);
                var result = con.QueryMultiple(sql, _params, commandType: CommandType.StoredProcedure);

                coupontxt = result.Read<string>().FirstOrDefault();
                coupons = result.Read<Coupons>().ToList();
            }
            return Tuple.Create(coupontxt, coupons);
        }

        public string SendCoupons(int IdMandant, int IdUser, string IpAddress, dynamic coupons, string Action = "SendCoupon")
        {
            DataTable CouponsToSend = new DataTable();
            CouponsToSend.Columns.Add("Id", typeof(int));

            DataRow row;

            for (int i = 0; i < coupons.Count; i++)
            {
                row = CouponsToSend.NewRow();
                row["Id"] = Convert.ToInt32(coupons[i].IdCoupon);
                CouponsToSend.Rows.Add(row);
            }

            var LastLanguageId = httpContext.Session.GetInt32("language");
            var sql = "usp_Coupon";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            using (con)
            {
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@Status", " ", DbType.String, direction: ParameterDirection.InputOutput);
                _params.Add("@IdMandant", IdMandant, DbType.Int32);
                _params.Add("@IdUser", IdUser);
                _params.Add("@Action", Action);
                _params.Add("@IpAddress", IpAddress);
                _params.Add("@IdCoupon", 1);
                _params.Add("@MailTo", "test@test.com");
                _params.Add("@IdLanguage", LastLanguageId);
                _params.Add("@CouponsToSend", CouponsToSend.AsTableValuedParameter("dbo.UniversalIdTable"));
                con.Execute(sql, _params, commandType: CommandType.StoredProcedure);

                return _params.Get<string>("Status");
            }
        }
        //add cache @!!!!!!!!!!!!!
        public bool DontShow(int IdMandant, int IdUser, string Action = "DoNotShowCoupon")
        {
            var sql = "usp_Coupon";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            using (con)
            {
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@Status", SqlDbType.VarChar, direction: ParameterDirection.InputOutput);
                _params.Add("@IdMandant", IdMandant);
                _params.Add("@IdUser", IdUser);
                _params.Add("@Action", Action);
                _params.Add("@IpAddress", null);
                var result = con.QuerySingleOrDefault<bool>(sql, _params, commandType: CommandType.StoredProcedure);

                return result;
            }
        }
        public bool UserInfo(int IdUser, string Info)
        {
            var sql = "CheckUserInfo";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);

            using (con)
            {
                con.Open();
                var parameters = new { IdUser, Info };
                return con.QuerySingleOrDefault<bool>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public bool UserInfoConfirmation(int IdUser, string Info)
        {
            var sql = "UserInfoConfirmation";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);

            using (con)
            {
                con.Open();
                var parameters = new { IdUser, Info };
                return con.QuerySingleOrDefault<bool>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public Modal GetModalTxt(string Action, int IdLanguage)
        {
            var sql = "SetInfoTxt";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);

            using (con)
            {
                con.Open();
                var parameters = new { Action, IdLanguage };
                return con.Query<Modal>(sql, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public Table GetHolidays(int IdMandant, int IdUser, string Action, int Year, int IdCustomer)
        {
            var sql = "usp_Holidays";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            dynamic dataResult;
            using (con)
            {
                con.Open();
                var parameters = new { IdMandant, IdUser, Action, Year, IdCustomer };
                dataResult = con.Query<dynamic>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
            return CreateTable(dataResult);
        }

        public Table GetHolidays(DynamicParameters parameters)
        {
            var sql = "usp_Holidays";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            dynamic dataResult;
            using (con)
            {
                con.Open();
                dataResult = con.Query<dynamic>(sql, parameters, commandType: CommandType.StoredProcedure);
            }

            return CreateTable(dataResult);
        }

        public string InsertHoliday(DynamicParameters parameters)
        {
            var sql = "usp_Holidays";
            var LastLanguageId = httpContext.Session.GetInt32("language");
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            using (con)
            {
                con.Open();
                parameters.Add("@NewApp", true, DbType.Boolean);
                parameters.Add("@IdLanguage", LastLanguageId);
                return con.Query<string>(sql, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public string DeleteHolidays(DynamicParameters parameters)
        {
            var sql = "usp_Holidays";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            using (con)
            {
                con.Open();
                parameters.Add("@NewApp", true, DbType.Boolean);
                return con.Query<string>(sql, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public string UpdateHoliday(DynamicParameters parameters)
        {
            var sql = "usp_Holidays";
            con = connectionFactory.CreateConnection(DatabaseConnectionName.OPAL);
            using (con)
            {
                con.Open();
                parameters.Add("@NewApp", true, DbType.Boolean);
                var r = con.Query<string>(sql, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return r;
            }

        }

        public DynamicParameters SetParameters(ExpandoObject dynamic, bool idsTable = false)
        {
            DynamicParameters parameters = new DynamicParameters();

            foreach (KeyValuePair<string, object> pair in dynamic)
            {
                if (pair.Value.GetType().FullName.Contains("List"))
                {
                    parameters.Add("@" + pair.Key, CreateTableValuedParameter(pair.Value, idsTable).AsTableValuedParameter($"dbo.{pair.Key}"));
                }
                else
                {
                    DbType dbt = (DbType)Enum.Parse(typeof(DbType), pair.Value.GetType().Name);
                    parameters.Add("@" + pair.Key, pair.Value, dbt);
                }
            }

            return parameters;
        }

        private Table CreateTable(dynamic dataResult)
        {
            Table table = new Table();
            if (dataResult != null && dataResult.Count > 0)
            {
                table = new Table(dataResult);
            }

            return table;
        }

        public DataTable CreateTableValuedParameter(object value, bool idsTable = false)
        {
            DataTable TableValuedParameter = new DataTable();
            TableValuedParameter.Columns.Add("Id", typeof(int));
            DataRow row;

            if (idsTable != true)
            {
                var list = (IList)value;

                Type type = list.HeuristicallyDetermineType();

                if (type.Namespace == "NewOPAL.Models")
                {
                    TableValuedParameter = CreateDefaultTable(TableValuedParameter, list);
                }
                else
                {
                    TableValuedParameter.Columns.Add("Value", type);
                    int number = 1;

                    foreach (var item in list)
                    {
                        row = TableValuedParameter.NewRow();
                        row["Id"] = number;
                        row["Value"] = item;
                        TableValuedParameter.Rows.Add(row);

                        number = number + 1;
                    }
                }
            }
            else
            {
                foreach (var item in (List<UniversalId>)value)
                {
                    row = TableValuedParameter.NewRow();
                    row["Id"] = item.Id;
                    TableValuedParameter.Rows.Add(row);
                }
            }

            return TableValuedParameter;
        }

        private DataTable CreateDefaultTable(DataTable dataTable, IList items)
        {
            Type type = items[0].GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo propertyInfo in props)
            {
                if (propertyInfo.Name != "Id")
                {
                    dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                }
            }

            DataRow row;
            foreach (var item in items)
            {
                row = dataTable.NewRow();
                foreach (var prop in props)
                {                    
                    if (prop.GetIndexParameters().Length == 0)
                    {
                        row[prop.Name] = prop.GetValue(item);
                    }                    
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}
