using System;

namespace HR.Entity.Dto
{
    public class AbsenceForApproval : ForApproval
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int AbsenceId { get; set; }
        public int AbsenceTypeId { get; set; }
        public int Conflicts { get; set; }
        public double NumberOfDays { get; set; }
        public int PersonnelAbsenceEntitlementId { get; set; }
        public string AbsenceType { get; set; }
        public string Description { get; set; }
        public string Forenames { get; set; }
        public string Surname { get; set; }
    }
}
