using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.ViewComponents
{
    public class DatePickerViewComponent : ViewComponent
    {
        public DatePickerViewComponent(/*IMenu menu, ITranslationService service, IMemoryCache memoryCache*/)
        {

        }

        public IViewComponentResult Invoke(string identifier, string label = null, bool required = false)
        {
            ViewBag.labelPicker = label;
            ViewBag.identifier = identifier;
            ViewBag.required = required;
            return View("DatePicker");
        }
    }
}
