using Microsoft.AspNetCore.Mvc;
using NewOPAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.ViewComponents
{
    public class SelectViewComponent : ViewComponent
    {
        public SelectViewComponent(/*IMenu menu, ITranslationService service, IMemoryCache memoryCache*/)
        {

        }

        public IViewComponentResult Invoke(string label, List<Words> words = null)
        {
            //if (label.Contains("-"))
            //{
            //    var split = label.Split("-");
            //    ViewBag.OptionalLabel 
            //}

            ViewBag.labelSelect = label;
            return View("Select",words);
        }
    }
}
