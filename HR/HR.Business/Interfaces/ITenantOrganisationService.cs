using HR.Entity.Dto;
using System.Collections.Generic;

namespace HR.Business.Interfaces
{
    public interface ITenantOrganisationService
    {
        IEnumerable<TenantOrganisation> RetrieveTenantOrganisations();
    }
}
