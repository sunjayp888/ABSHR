namespace HR.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("JobGrade")]
    public partial class JobGrade
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JobGrade()
        {
            JobTitleJobGrades = new HashSet<JobTitleJobGrade>();
            Employments = new HashSet<Employment>();
        }

        public int JobGradeId { get; set; }

        public int OrganisationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual Organisation Organisation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobTitleJobGrade> JobTitleJobGrades { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employment> Employments { get; set; }

    }
}
