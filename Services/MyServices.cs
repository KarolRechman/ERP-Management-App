using Microsoft.Extensions.DependencyInjection;
using NewOPAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public static class MyServices
    {
        public static IServiceCollection RegisterMyServices(this IServiceCollection services)
        {
            return services
                    .AddTransient<IDataAccess, DataAccess>()
                    .AddTransient<ITranslationService, TranslationService>()
                    .AddTransient<IMenu, Menu>()
                    .AddTransient<IHelpService, HelpService>()
                    .AddTransient<ILocationsService, LocationsService>()
                    .AddTransient<ICacheService, CacheService>()
                    .AddTransient<ICustomerService, CustomerService>()
                    .AddTransient<IPartsService, PartsService>()
                    .AddTransient<IConnectionFactory, SqlConnectionFactory>();
        }
    }
}
