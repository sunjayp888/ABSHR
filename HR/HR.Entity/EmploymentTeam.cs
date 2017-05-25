namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmploymentTeam")]
    public partial class EmploymentTeam
    {
        public int EmploymentTeamId { get; set; }

        public int OrganisationId { get; set; }

        public int EmploymentId { get; set; }

        public int TeamId { get; set; }

        public virtual Employment Employment { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual Team Team { get; set; }
    }
}
