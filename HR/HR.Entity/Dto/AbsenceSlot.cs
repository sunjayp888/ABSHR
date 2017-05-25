using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HR.Entity.Dto
{
    public class AbsenceSlot : Slot
    {
        private IEnumerable<AbsenceDay> _weekAbsenceDays;

        [JsonIgnore]
        public AbsenceType AbsenceType { get; set; }

        [JsonIgnore]
        public IEnumerable<AbsenceDay> AbsenceDays { get; set; }

        //[JsonIgnore]
        //public bool DayBeforeAbsenceIsAmOnly { get; set; }

        //[JsonIgnore]
        //public bool DayAfterAbsenceIsPmOnly { get; set; }

        public override string Title => string.Format("{0} {1}: {2}{3} - {4}{5}",
            //AbsenceStatus?.Name,
            ApprovalState?.Name,
            AbsenceType?.Name,
            BeginAbsenceDay.AbsencePart == AbsencePart.HalfDayPM ? "PM " : string.Empty,
            BeginAbsenceDay.Date.ToLongDateString(),
            EndAbsenceDay.AbsencePart == AbsencePart.HalfDayPM ? "PM " : string.Empty,
            EndAbsenceDay.Date.ToLongDateString());
        
        public override bool IsAbsence => true;

        public override string Details => string.Format("{0}{1}",
            AbsenceType?.Name,
            Duration > 1 ? string.Format(" ({0} days)", Duration) : string.Empty);

        public IEnumerable<AbsenceDay> SlotAbsenceDays
        {
            get
            {
                if (!BeginDate.HasValue || !EndDate.HasValue)
                    return null;

                if (_weekAbsenceDays == null)
                    _weekAbsenceDays = AbsenceDays.Where(d => d.Date.Date >= BeginDate.Value.Date && d.Date.Date <= EndDate.Value.Date);

                return _weekAbsenceDays;
            }
        }

        public override DateTime? EndDate { get; set; }

        //public override double Duration => AbsenceDays.Where(day => day.AbsenceDayId != 0)?.Sum(day => day.Duration) ?? 0;

        //public override double Days => ((SlotAbsenceDays?.Sum(day => day.Duration) ?? 0) + (DayBeforeAbsenceIsAmOnly ? 0.5 : 0) + (DayAfterAbsenceIsPmOnly ? 0.5 : 0));

        public override double Duration => AbsenceDays?.Sum(day => day.Duration) ?? 0;

        public override double Days => SlotAbsenceDays?.Sum(day => day.Duration) ?? 0;

        [JsonIgnore]
        public AbsenceDay BeginAbsenceDay => AbsenceDays?.OrderBy(day => day.Date).FirstOrDefault();

        [JsonIgnore]
        public AbsenceDay SlotBeginAbsenceDay => SlotAbsenceDays?.OrderBy(day => day.Date).FirstOrDefault();

        [JsonIgnore]
        public AbsenceDay EndAbsenceDay => AbsenceDays?.OrderByDescending(day => day.Date).FirstOrDefault();
        
        public override int Begins => SlotBeginAbsenceDay?.DayOfWeek ?? 0;


        public override bool BeginsPM => SlotBeginAbsenceDay?.AbsencePart == AbsencePart.HalfDayPM;


        public override bool OverflowsBefore => BeginAbsenceDay.Date < BeginDate.Value.Date;

        public override bool OverflowsAfter => EndAbsenceDay.Date > EndDate;

        //public override AbsenceStatus AbsenceStatus { get; set; }
        public override ApprovalState ApprovalState { get; set; }

        public bool? CanApprove { get; set; }
    }
}
