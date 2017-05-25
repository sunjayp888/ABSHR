namespace HR.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PublicHoliday")]
    public partial class PublicHoliday
    {
        public int PublicHolidayId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public int OrganisationId { get; set; }

        public int PublicHolidayPolicyId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual PublicHolidayPolicy PublicHolidayPolicy { get; set; }
    }
}
