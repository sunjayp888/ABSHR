namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Host")]
    public partial class Host
    {
        public short HostId { get; set; }

        [Required]
        [StringLength(100)]
        public string HostName { get; set; }

        public int OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
