using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewOPAL.Models
{
    public class OpalUser : IdentityUser
    {
        [Required]
        [Column("IDUSER")]
        public int Iduser { get; set; }
        [Required]
        [Column("IDMandant")]
        public int IdMandant { get; set; }
        public string PersonalNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int IdLanguage { get; set; }
    }
}
