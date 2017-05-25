namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(AbsenceMetadata))]
    public partial class Absence : IOrganisationFilterable
    {

        private class AbsenceMetadata
        {
            [Display(Name = "Type")]
            public int AbsenceTypeId { get; set; }

        }
    }
}