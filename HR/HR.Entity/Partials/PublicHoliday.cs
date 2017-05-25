using HR.Entity.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Entity
{
    [MetadataType(typeof(PublicHolidayMetadata))]
    public partial class PublicHoliday : INotAbsenceDay
    {
        [NotMapped]
        public bool AM { get { return false; } set { } }
        [NotMapped]
        public bool PM { get { return false; } set { } }
        [NotMapped]
        public string ValidationReason => Name;

        private class PublicHolidayMetadata
        {
           
        }
    }
}
