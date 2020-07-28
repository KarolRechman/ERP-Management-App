using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewOPAL.Services;
using Microsoft.AspNetCore.Identity;
using NewOPAL.Models;
using System.Dynamic;
using Microsoft.Extensions.Caching.Memory;

namespace NewOPAL.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMemoryCache cache;
        private IMenu menuNavbar;
        private ITranslationService translationService;
        public MenuViewComponent(IMenu menu, ITranslationService service,IMemoryCache memoryCache)
        {
            menuNavbar = menu;
            translationService = service;
            cache = memoryCache;
        }

        public IViewComponentResult Invoke(OpalUser user = null)
        {
            if (cache.TryGetValue("menuNav", out MenuNav menuNavCache))
            {
                return View("Menu", menuNavCache);
            }
            else
            {
                MenuNav menuNav = new MenuNav()
                {
                    Languages = translationService.GetLanguages(),
                    Companies = menuNavbar.GetCompanies(user.Iduser, user.IdMandant),
                    MenuParents = menuNavbar.GetMenu(user.Iduser, user.IdMandant)
                };
                cache.Set("menuNav", menuNav, TimeSpan.FromSeconds(60));

                return View("Menu", menuNav);
            }

        }
    }
}
