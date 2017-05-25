using System;
using System.Collections.Generic;
using System.Linq;

namespace HR.Entity.Interfaces
{
    public interface INotAbsenceDay
    {
        DateTime Date { get; set; }
        bool AM { get; set; }
        bool PM { get; set; }
        string ValidationReason { get; }
    }

    //create a NotAbsenceDay record by constructing it with a collection of INotAbsenceDay objects
    //the NotAbsenceDay class will return the reason and the booleans

    public class NotAbsenceDay: INotAbsenceDay
    {
        public DateTime Date { get; set; }
        public bool AM
        {
            get { return NotAbsenceDays.All(d => d.AM == true); }
            set { throw new NotImplementedException(); }
        }

        public bool PM
        {
            get { return NotAbsenceDays.All(d => d.PM == true); }
            set { throw new NotImplementedException(); }
        }

        public string ValidationReason
        {
            get
            {
                var amReason = String.Join(",", NotAbsenceDays.Where(e => e.Date == Date).Select(d => d.ValidationReason));
                //var pmReason = String.Join(",", NotAbsenceDays.Where(d => d.PM == false).Select(d => d.ValidationReason));

                return string.Format(amReason);
            }
        }

        public List<INotAbsenceDay> NotAbsenceDays; // store all INotAbsenceDay dates e.g. Public Hols, Non Working NotAbsenceDays, Already Booked NotAbsenceDays
    }

    public class CanBeBookedAbsenceDay : INotAbsenceDay
    {
        public  DateTime Date { get; set; }
        public bool AM { get; set; }
        public bool PM { get; set; }
        public string ValidationReason => !AM && PM ? "AM has already been booked as an absence" : AM && !PM ? "PM has already been booked as an absence" : "Full day has already been booked as an absence";
    }
}
