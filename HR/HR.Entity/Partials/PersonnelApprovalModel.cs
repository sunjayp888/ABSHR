namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(PersonnelApprovalModelMetadata))]
    public partial class PersonnelApprovalModel : IOrganisationFilterable
    {
        private class PersonnelApprovalModelMetadata
        {
        }
    }

}

