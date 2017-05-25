namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Building")]
    public partial class Building
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Building()
        {
            CompanyBuildings = new HashSet<CompanyBuilding>();
            Employments = new HashSet<Employment>();
        }

        public int BuildingId { get; set; }

    
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        public int OrganisationId { get; set; }

        public int SiteId { get; set; }

        [StringLength(100)]
        public string Address1 { get; set; }

        public virtual Site Site { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyBuilding> CompanyBuildings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employment> Employments { get; set; }
    }
}
