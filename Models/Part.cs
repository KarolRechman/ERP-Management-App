using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Models
{
    public class Part
    {
    }

    public class PartCondition
    {
        public int IDPartCondition { get; set; }
        public string PartConditionDescription { get; set; }
        public int? IdProductCategory { get; set; }
    }
}
