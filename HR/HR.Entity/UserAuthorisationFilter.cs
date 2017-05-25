namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserAuthorisationFilter")]
    public partial class UserAuthorisationFilter
    {
        public long? Id { get; set; }

        public int? PersonnelId { get; set; }

        [Key]
        [Column(Order = 0)]
        public string AspNetUsersId { get; set; }
        
        public int? OrganisationId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string RoleId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(256)]
        public string RoleName { get; set; }
    }
}
