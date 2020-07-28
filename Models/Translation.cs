using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Models
{
    public class Translation
    {
    }
    public class Words
    {
        [Column(Order = 1)]
        public int Id { get; set; }
        [Column(Order = 2)]
        public string Text { get; set; }
    }
    public class Language
    {
        public int IdLanguage { get; set; }
        public string EnglishName { get; set; }
        public string NativeName { get; set; }
        public string ISOCode { get; set; }
    }


}
