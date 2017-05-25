namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OvertimePreference")]
    public partial class OvertimePreference
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OvertimePreference()
        {
            Overtimes = new HashSet<Overtime>();
        }

        public int OvertimePreferenceId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Overtime> Overtimes { get; set; }
    }
}
