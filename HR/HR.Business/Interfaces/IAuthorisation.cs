using HR.Entity;
using HR.Entity.Dto;

namespace HR.Business.Interfaces
{
    public interface IAuthorisation
    {
        int? OrganisationId { get; set; }        
        int RoleId { get; set; }
        Role Role { get; }
    }
}
