namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AbsencePolicyEntitlement")]
    public partial class AbsencePolicyEntitlement
    {
        public int AbsencePolicyEntitlementId { get; set; }

        public int OrganisationId { get; set; }

        public int AbsenceTypeId { get; set; }

        public int FrequencyId { get; set; }

        public double Entitlement { get; set; }

        public double MaximumCarryForward { get; set; }

        public bool IsUnplanned { get; set; }

        public bool IsPaid { get; set; }

        public int AbsencePolicyId { get; set; }

        public bool HasEntitlement { get; set; }

        public virtual AbsencePolicy AbsencePolicy { get; set; }

        public virtual AbsenceType AbsenceType { get; set; }

        public virtual Frequency Frequency { get; set; }
        
        public virtual Organisation Organisation { get; set; }
    }
}
