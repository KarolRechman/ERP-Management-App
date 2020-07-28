using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewOPAL.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class Company : Location
    {

    }
}
