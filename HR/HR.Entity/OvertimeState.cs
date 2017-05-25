namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OvertimeState")]
    public partial class OvertimeState
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OvertimeState()
        {
        }

        public int OvertimeStateId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
    }
}
