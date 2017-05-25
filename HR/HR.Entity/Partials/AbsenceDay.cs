namespace HR.Entity
{
    using Dto;
    using Interfaces;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(AbsenceDayMetadata))]
    public partial class AbsenceDay : IOrganisationFilterable, INotAbsenceDay
    {
        [NotMapped]
        public bool FullDay => AM && PM;

        [NotMapped]
        public AbsencePart AbsencePart => FullDay ? AbsencePart.FullDay : AM ? AbsencePart.HalfDayAM : AbsencePart.HalfDayPM;

        [NotMapped]
        public bool CanBeBookedAsAbsence { get; set; }

        [NotMapped]
        public string ValidationReason => AM && !PM?"AM has already been booked as an absence": !AM && PM? "PM has already been booked as an absence" : "Full day has already been booked as an absence";

        [NotMapped]
        public string Validation { get; set; }

        [NotMapped]
        public int DayOfWeek => Date.DayOfWeek == System.DayOfWeek.Sunday ? 7 : Date.DayOfWeek.GetHashCode();

        private class AbsenceDayMetadata
        {
        }
    }
}