using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewOPAL.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewOPAL.Models;
using Westwind.AspNetCore.LiveReload;
using NewOPAL.Services;

namespace NewOPAL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("Identity")));
            services.AddDefaultIdentity<OpalUser>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
 
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            services.AddSession();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(30);
            });
            services.AddMemoryCache();
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                //options.AllowMappingHeadRequestsToGetHandler = false;
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
                //options.Conventions.AuthorizeFolder("/Main");
                options.Conventions.AuthorizeFolder("/");
                options.Conventions.AuthorizeAreaFolder("Main", "/");
                options.Conventions.AuthorizeAreaFolder("General", "/");
                options.Conventions.AuthorizeAreaFolder("SpareParts", "/");
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account");
                //options.Conventions.AllowAnonymousToPage("/Account/Login");
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddLiveReload(config =>
            //{
            //    //optional - use config instead
            //    config.LiveReloadEnabled = true;
            //    config.FolderToMonitor = Path.GetFullname(Path.Combine(Env.ContentRootPath, ".."));
            //});
            //services.AddResponseCaching();
            services.AddLiveReload();
            services.AddSingleton<IConfiguration>(Configuration);

            var connectionDict = new Dictionary<DatabaseConnectionName, string>
            {
                { DatabaseConnectionName.Identity, this.Configuration.GetConnectionString("Identity") },
                { DatabaseConnectionName.OPAL, this.Configuration.GetConnectionString("OPAL") },
                { DatabaseConnectionName.Test, this.Configuration.GetConnectionString("Test") }
            };

            // Inject this dict
            services.AddSingleton<IDictionary<DatabaseConnectionName, string>>(connectionDict);

            // Inject the factory
            services.AddTransient<IConnectionFactory, SqlConnectionFactory>();

            //// Register your regular repositories
            //services.AddScoped<IDiameterRepository, DiameterRepository>();
            ////////////////////////////////////
            //var connString = Configuration.GetConnectionString("Identity");
            //if (connString == null)
            //    throw new ArgumentNullException("Connection string cannot be null");

            //services.AddSingleton<IConnectionFactory>(s => new SqlConnectionFactory(connString));

            services.RegisterMyServices();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseLiveReload();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            //app.UseCookiePolicy();

            app.UseAuthentication();
            //app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = false,
                        MaxAge = TimeSpan.FromSeconds(-10),
                        MustRevalidate = true,
                        NoCache = true,
                        NoStore = true,
                        
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });

            app.UseMvc();

        }
    }
}
