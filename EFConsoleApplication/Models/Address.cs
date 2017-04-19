using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EFConsoleApplication.Enums;

namespace EFConsoleApplication.Models
{
    public class Address : EntityWithHistoryBase
    {
        [StringLength(250)]
        public string Street { get; set; }

        [Required]
        [StringLength(25)]
        public string Number { get; set; }

        [StringLength(250)]
        public string District { get; set; }

        [Required]
        [StringLength(250)]
        public string City { get; set; }

        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(250)]
        public string County { get; set; }

        [Required]
        [StringLength(250)]
        public string Country { get; set; }

        [Required]
        public AddressType AddressType { get; set; }

        public int? PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }
    }
}