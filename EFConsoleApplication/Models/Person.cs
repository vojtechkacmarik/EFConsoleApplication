using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFConsoleApplication.Models
{
    public class Person : EntityWithHistoryBase
    {
        [Required]
        [StringLength(250)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(250)]
        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
    }
}