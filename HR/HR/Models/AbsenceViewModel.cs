using HR.Entity;
using HR.Entity.Dto;
using System;
using System.Web.Mvc;

namespace HR.Models
{
    public class AbsenceViewModel : BaseViewModel
    {
        public DateTime BeginDate { get; set; }
        public DateTime EmploymentBeginDate { get; set; }
        public DateTime EmploymentEndDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Period { get; set; }
        public SelectList AbsenceTypes { get; set; }
        public AbsenceRange Absence { get; set; }
        public PersonnelAbsenceEntitlement PersonnelAbsenceEntitlement { get; set; }

    }
}