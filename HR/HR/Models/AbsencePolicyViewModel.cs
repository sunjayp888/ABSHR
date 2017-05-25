using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Entity;

namespace HR.Models
{
    public class AbsencePolicyViewModel:BaseViewModel
    {
        public AbsencePolicy AbsencePolicy { get; set; }
        public List<WorkingPatternDay> WorkingPatternDays { get; set; }
        public List<AbsenceType> AbsenceTypes { get; set; }
        public List<AbsencePeriod> AbsencePeriods{ get; set; }
        public AbsencePolicyEntitlement AbsencePolicyEntitlement { get; set; } 
        public SelectList Frequencies { get; set; }
        public double SumOfDuration { get; set; }
    }
}