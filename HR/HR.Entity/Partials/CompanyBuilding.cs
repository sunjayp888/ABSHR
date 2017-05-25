using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using System.ComponentModel;

    [MetadataType(typeof(CompanyBuildingMetadata))]
    public partial class CompanyBuilding : IOrganisationFilterable
    {
      
        private class CompanyBuildingMetadata
        {


        }
    }
}
