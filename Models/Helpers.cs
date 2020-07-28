using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Models
{
    public class Helpers
    {
    }

    public class Coupons
    {
        public int IdCoupon { get; set; }
        public string Logo { get; set; }
        public string Value { get; set; }
        public string Txt { get; set; }
        public string ExpiryDate { get; set; }
        public string InfoTxt { get; set; }
        //[Display(Name = "")]
        //public bool Check { get; set; }
    }

    public class Modal
    {
        public string BodyTitle { get; set; }
        public string BodyInfo { get; set; }
    }

    public class UniversalId
    {
        public int Id { get; set; }
    }
}
