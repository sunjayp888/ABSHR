using HR.Business.Interfaces;
using HR.Entity.Dto;

namespace HR.Business.Models
{
    public class Authorisation : IAuthorisation
    {
        public int? OrganisationId { get; set; }

        public Role Role => (Role)RoleId;

        public int RoleId { get; set; }
    }
}
