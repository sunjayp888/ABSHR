namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmergencyContact")]
    public partial class EmergencyContact
    {
        public int EmergencyContactId { get; set; }

        public int OrganisationId { get; set; }

        public int PersonnelId { get; set; }

        [Required]
        [StringLength(50)]
        public string Relationship { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Forenames { get; set; }

        [Required]
        [StringLength(50)]
        public string Surname { get; set; }

        public int? CountryId { get; set; }

        [StringLength(100)]
        public string Address1 { get; set; }

        [StringLength(100)]
        public string Address2 { get; set; }

        [StringLength(100)]
        public string Address3 { get; set; }

        [StringLength(100)]
        public string Address4 { get; set; }

        [StringLength(12)]
        public string Postcode { get; set; }

        [Required]
        [StringLength(15)]
        public string Telephone { get; set; }

        [StringLength(15)]
        public string Mobile { get; set; }

        public virtual Country Country { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual Personnel Personnel { get; set; }
    }
}
