using HR.Entity.Interfaces;
using System;

namespace HR.Entity.Dto
{
    public class CanBeBookedWorkingPatternDay : INotAbsenceDay
    {
        public DateTime Date { get; set; }
        public bool AM { get; set; }
        public bool PM { get; set; }
        public string ValidationReason
        {
            get
            {
                if (AM)
                    return PM ? "" : "PM not a working day cannot be booked.";

                return PM ? "AM not a working day cannot be booked." : "Not a working day.";
            }
        }
    }
}
