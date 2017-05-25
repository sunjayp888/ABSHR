namespace HR.Entity
{
    using Dto;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Overtime")]
    public partial class Overtime
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Overtime()
        {
            CreatedDateUtc = DateTime.UtcNow;
            ApprovalStateId = (int)ApprovalStates.Requested;
        }
        
        public int OvertimeId { get; set; }
        
        public int OrganisationId { get; set; }

        public int PersonnelId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        
        public double Hours { get; set; }

        public int OvertimePreferenceId { get; set; }

        public int ApprovalStateId { get; set; }

        [Required]
        public string Reason { get; set; }
        
        public string Comment { get; set; }

        public DateTime? CreatedDateUtc { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDateUtc { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public virtual ApprovalState ApprovalState { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual OvertimePreference OvertimePreference { get; set; }

        public virtual Personnel Personnel { get; set; }
    }
}
