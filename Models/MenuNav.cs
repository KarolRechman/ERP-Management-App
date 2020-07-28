using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Models
{
    public class MenuNav
    {
        public List<MenuLink> MenuParents { get; set; }
        public List<Company> Companies { get; set; }
        public List<Language> Languages { get; set; }
    }

    public class MenuLink
    {
        public int IdMenu { get; set; }
        public int IdParent { get; set; }
        public string MenuName { get; set; }
        [Column("urllink")]
        public string UrlLink { get; set; }
        public string Protocol { get; set; }
        public List<MenuLink> Links { get; set; }
    }
}
