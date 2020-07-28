using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewOPAL.Models;

namespace NewOPAL.ViewComponents
{
    public class AccordionMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<MenuLink> menuLinks, bool firstCall = true, int padding = 0)
        {
            ViewBag.padding = padding;
            ViewBag.firstCall = firstCall;
            return View("AccordionMenu", menuLinks);
        }
    }
}
