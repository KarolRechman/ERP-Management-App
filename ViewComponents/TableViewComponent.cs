using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using NewOPAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.ViewComponents
{
    public class TableViewComponent : ViewComponent
    {
        public TableViewComponent(/*IMenu menu, ITranslationService service, IMemoryCache memoryCache*/)
        {

        }

        public IViewComponentResult Invoke(string label, Table table)
        {
            if (table.Columns != null && table.Items != null)
            {
                if (table.TableOptions.HasCheckboxes == true)
                {
                    table.Columns.Add(new Column
                    {
                        Id = table.Columns.Count + 1,
                        Hidden = false,
                        Name = ""
                    });

                    foreach (var item in table.Items)
                    {
                        item.TryAdd("CheckBox", false);
                    }
                }
            }



            ViewBag.labelTable = label;
            return View("Table", table);
        }


    }
}
