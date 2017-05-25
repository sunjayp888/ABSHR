using System;

namespace HR.Entity.Dto
{
    public class PublicHolidaySlot : Slot
    {
        public override int AbsenceId { get { return 0; } }
        public override string Title => "Public Holiday";

        public override bool IsAbsence => false;

        public override string Details { get; set; }

        public override double Duration => (1);

        public override double Days { get { return Duration; } }

        public override int Begins { get { return Convert.ToInt32(BeginDate.Value.DayOfWeek); } }

        public override bool BeginsPM { get { return false; } }

        public override bool OverflowsBefore { get { return false; } }
        public override bool OverflowsAfter { get { return false; } }

        //public override AbsenceStatus AbsenceStatus { get { return null; } }

        public override ApprovalState ApprovalState { get { return null; } }

        public override DateTime? EndDate { get { return BeginDate; } }
    }
}
