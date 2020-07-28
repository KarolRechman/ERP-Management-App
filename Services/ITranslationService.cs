using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
    public interface ITranslationService
    {
        List<Words> Translate(int id, List<Words> words, int? LastLanguageId);
        string TranslateWord(int? id, string word, int LastLanguageId = 1);
        List<Language> GetLanguages();
        string GetTxtById(int IdTxt, int? IdLanguage = null);
        int? GetLanguageId();
        //IEnumerable<TItem> GetData<TItem>(string sql, DatabaseConnectionName DBname, IEnumerable<TItem> item, object parametrs = null);
    }
    public class TranslationService : ITranslationService
    {
        private ICacheService cacheService;
        private IConnectionFactory connectionFactory;
        private SqlConnection con;
        private readonly DatabaseConnectionName DBname = DatabaseConnectionName.OPAL;
        private HttpContext httpContext;
        private IHelpService helpService;
        public TranslationService(IConnectionFactory connection, ICacheService cache, IHttpContextAccessor httpContextAccessor, IHelpService help)
        {
            connectionFactory = connection;
            cacheService = cache;
            httpContext = httpContextAccessor.HttpContext;
            helpService = help;
        }

        public List<Words> Translate(int id, List<Words> words, int? LastLanguageId = 1)
        {
            var sql = "GetTranslations";

            DataTable OrginalWords = helpService.CreateTableValuedParameter(words);

            con = connectionFactory.CreateConnection(DBname);
            using (con)
            {
                con.Open();
                var parameters = new { IdLanguage = id, Words = OrginalWords, LastLanguageId };

                return con.Query<Words>(sql, parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public string TranslateWord(int? id, string word, int LastLanguageId = 1)
        {
            if ((id == 1) && (LastLanguageId == 1) || (word == ""))
            {
                return word;
            }
            else
            {
                String item = null;
                var parameters = new { IdLanguage = id, Word = word, LastLanguageId };
                return cacheService.GetItem("GetWordTranslation", DBname, item, parameters);
            }
        }

        public List<Language> GetLanguages()
        {
            var item = new List<Language>();
            return cacheService.GetData("GetLanguages", DBname, item).ToList();
        }
        public string GetTxtById(int IdTxt, int? IdLanguage = null)
        {
            if (IdLanguage == null)
            {
                IdLanguage = GetLanguageId();
            }

            String item = null;
            var parameters = new { IdTxt, IdLanguage };
            return cacheService.GetItem("GetTxtById", DBname, item, parameters);
        }

        public int? GetLanguageId()
        {
            int? LanguageId = httpContext.Session.GetInt32("language");

            if (LanguageId == null)
            {
                LanguageId = 1;
                httpContext.Session.SetInt32("language", Convert.ToInt32(LanguageId));
            }
            return LanguageId;
        }
    }
}
