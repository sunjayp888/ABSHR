namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CompanyBuilding")]
    public partial class CompanyBuilding
    {
        public int CompanyBuildingId { get; set; }

        public int BuildingId { get; set; }

        public int CompanyId { get; set; }

        public int OrganisationId { get; set; }

        public virtual Building Building { get; set; }

        public virtual Company Company { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
