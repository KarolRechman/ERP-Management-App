using Dapper;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public interface ICacheService
    {
        IEnumerable<TItem> GetData<TItem>(string sql, DatabaseConnectionName DBname, IEnumerable<TItem> item, object parametrs = null);
        TItem GetItem<TItem>(string sql, DatabaseConnectionName DBname, TItem item, object parametrs = null);
        //TItem Get<TItem>(this IMemoryCache cache, object key);
    }
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache cache;
        private IConnectionFactory connectionFactory;
        public CacheService(IConnectionFactory connection, IMemoryCache memoryCache)
        {
            cache = memoryCache;
            connectionFactory = connection;
        }

        public IEnumerable<TItem> GetData<TItem>(string sql, DatabaseConnectionName DBname, IEnumerable<TItem> item, object parametrs = null)
        {
            string cacheKey = SetCacheKey(sql, parametrs);
            if (cache.TryGetValue(cacheKey, out item))
            {
                return item;
            }

            var con = connectionFactory.CreateConnection(DBname);
            using (con)
            {
                con.Open();

                item = con.Query<TItem>(sql, parametrs, commandType: CommandType.StoredProcedure).ToList();
                cache.Set(cacheKey, item, TimeSpan.FromSeconds(60));
                return item;
            }
        }

        public TItem GetItem<TItem>(string sql, DatabaseConnectionName DBname, TItem item, object parametrs = null)
        {
            string cacheKey = SetCacheKey(sql, parametrs);
            if (cache.TryGetValue(cacheKey, out item))
            {
                return item;
            }

            var con = connectionFactory.CreateConnection(DBname);
            using (con)
            {
                con.Open();

                item = con.QuerySingle<TItem>(sql, parametrs, commandType: CommandType.StoredProcedure);
                cache.Set(cacheKey, item, TimeSpan.FromSeconds(60));
                return item;
            }
        }

        private string SetCacheKey(string sql, object parametrs)
        {
            if (parametrs == null)
            {
                return sql;
            }
            else
            {
                return sql + parametrs.ToString();
            }
        }
    }
}
