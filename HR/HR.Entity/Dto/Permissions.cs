namespace HR.Entity.Dto
{
    public class Permissions
    {
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; }
        public bool CanViewProfile { get; set; }
        public bool CanEditProfile { get; set; }
        public bool CanEditAbsence { get; set; }
        public bool CanCreateAbsence { get; set; }
        public bool CanCancelAbsence { get; set; }
        //public bool CanApproveAbsence { get; set; }
        //public bool CanAlterAbsence => CanEditAbsence || CanCancelAbsence || CanApproveAbsence;
        public bool CanAlterAbsence => CanEditAbsence || CanCancelAbsence;
        public bool CanEditEntitlements { get; set; }
        public bool CanEditEmployments { get; set; }
        public bool CanCreateOvertime { get; set; }
        public bool CanEditOvertime { get; set; }
        public bool CanDeleteOvertime { get; set; }
        public bool CanEditApprover { get; set; }
    }
}
