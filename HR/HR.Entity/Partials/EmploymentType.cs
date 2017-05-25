using System.ComponentModel;

namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(EmploymentTypeMetadata))]
    public partial class EmploymentType : IOrganisationFilterable
    {
        private class EmploymentTypeMetadata
        {
            [DisplayName("Employment Type")]
            [Required]
            public int EmploymentTypeId { get; set; }
        }
    }

}

