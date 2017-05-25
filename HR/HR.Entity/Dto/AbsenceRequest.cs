using System.Collections.Generic;
using System.Web.Mvc;

namespace HR.Entity.Dto
{
    public class AbsenceRequest
    {
        public IEnumerable<AbsenceDay> AbsenceDays { get; set; }
        public AbsenceType AbsenceType { get; set; }
        public PersonnelAbsenceEntitlement PersonnelAbsenceEntitlement { get; set; }
        public double Duration { get; set; }
        public IEnumerable<AbsenceType> AbsenceTypes { get; set; }

    }
}
