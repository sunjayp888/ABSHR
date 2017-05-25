using HR.Entity.Dto;
using System.Collections.Generic;

namespace HR.Interfaces
{
    public interface ITenantsService
    {        
        IEnumerable<TenantOrganisation> TenantOrganisations();
        TenantOrganisation CurrentTenantOrganisation(string hostname);
    }
}
