using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using NewOPAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Services
{
    public interface IMenu
    {
        List<Company> GetCompanies(int IDUSER, int IDMANDANT);
        List<MenuLink> GetMenu(int IDUSER, int IDMANDANT);
        int? GetMenuCompany();
        void SetMenuCompany(int id);
        int GetIdMenu(string MenuName);
        void SetIdMenu(int id);
        string GetMenuName(string Name);
    }
    public class Menu : IMenu
    {
        private readonly IMemoryCache cache;
        private ICacheService cacheService;
        private IConnectionFactory connectionFactory;
        private SqlConnection con;
        private readonly DatabaseConnectionName DBname = DatabaseConnectionName.OPAL;
        private HttpContext httpContext;
        public Menu(IConnectionFactory connection, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, ICacheService _cache)
        {
            connectionFactory = connection;
            cache = memoryCache;
            httpContext = httpContextAccessor.HttpContext;
            cacheService = _cache;
        }

        public List<Company> GetCompanies(int IDUSER, int IDMANDANT)
        {
            var item = new List<Company>();
            var parameters = new { IDMANDANT, IDUSER };
            return cacheService.GetData("Database_User_Mandant_List", DBname, item, parameters).ToList();
        }

        public List<MenuLink> GetMenu(int IDUSER, int IDMANDANT)
        {
            List<MenuLink> MenuLinks = new List<MenuLink>();
            var item = new List<MenuLink>();
            var parameters = new { IDMANDANT, IDUSER, NewOpal = true };
            MenuLinks = cacheService.GetData("WebMenu", DBname, item, parameters).ToList();

            var MenuTree = GetMenuTree(MenuLinks, 0);
            cache.Set("menuParents", MenuTree, TimeSpan.FromSeconds(1));
            //MenuTree.RemoveRange(1, 12);
            return MenuTree;
        }

        public int? GetMenuCompany()
        {
            return httpContext.Session.GetInt32("MenuCompany");
        }

        public void SetMenuCompany(int id)
        {
            httpContext.Session.SetInt32("MenuCompany", id);
        }

        public int GetIdMenu(string MenuName = null)
        {
            if (MenuName != null)
            {
                MenuName = GetMenuName(MenuName);
                int item = 0;
                var parameters = new { MenuName };
                return cacheService.GetItem("CheckIdMenu", DBname, item, parameters);
            }
            else
            {
                return httpContext.Session.GetInt32("IdMenu").GetValueOrDefault();
            }
        }

        public void SetIdMenu(int id)
        {
            httpContext.Session.SetInt32("IdMenu", id);
        }

        private List<MenuLink> GetMenuTree(List<MenuLink> menuLinks, int? IdParent)
        {
            return menuLinks.Where(m => m.IdParent == IdParent).Select(x => new MenuLink()
            {
                IdMenu = x.IdMenu,
                MenuName = x.MenuName,
                IdParent = x.IdParent,
                UrlLink = x.UrlLink,
                Protocol = x.Protocol,
                Links = GetMenuTree(menuLinks, x.IdMenu)

            }).ToList();
        }

        public string GetMenuName(string Name)
        {
            return string.Concat(Name.Select(c => char.IsUpper(c) ? " " + c.ToString() : c.ToString())).TrimStart();
        }       
    }
}
