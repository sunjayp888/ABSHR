using HR.Entity;
using HR.Entity.Dto;
using HR.Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HR.Extensions
{
    public static class AbsenceExtensions
    {
        /// <summary>
        /// Method takes absence days and already booked days and either returns a summary of the booking or returns only the days that will be booked.
        /// </summary>
        /// <param name="absenceDays"></param>
        /// <param name="cannotBeBookedDays"></param>
        /// <param name="returnUnbookableDays"></param>
        /// <returns></returns>

        public static IEnumerable<AbsenceDay> ToAbsenceDaysSummary(this IEnumerable<AbsenceDay> absenceDays, IEnumerable<INotAbsenceDay> cannotBeBookedDays, bool returnUnbookableDays = false)
        {
            if (cannotBeBookedDays == null)
                return absenceDays;

            var filteredAbsenceDays = new List<AbsenceDay>();

            foreach (var absenceDay in absenceDays)
            {
                var notAbsenceDay = cannotBeBookedDays.FirstOrDefault(w => w.Date.Date == absenceDay.Date.Date);
                absenceDay.AM = absenceDay.AM && notAbsenceDay != null ? notAbsenceDay.AM : absenceDay.AM;
                absenceDay.PM = absenceDay.PM && notAbsenceDay != null ? notAbsenceDay.PM : absenceDay.PM;

                if (notAbsenceDay != null)
                {
                    absenceDay.CanBeBookedAsAbsence = absenceDay.AM || absenceDay.PM;
                    absenceDay.Validation = notAbsenceDay.ValidationReason;

                    if (!returnUnbookableDays && !absenceDay.CanBeBookedAsAbsence)
                        continue;
                }
                else
                    absenceDay.CanBeBookedAsAbsence = true;

                absenceDay.Duration = CalculateAbsenceDayDuration(absenceDay.AM, absenceDay.PM);
                filteredAbsenceDays.Add(absenceDay);
            }
            return filteredAbsenceDays;
        }

        public static IEnumerable<AbsenceDay> ToAbsenceDayList(this IEnumerable<DateTime> dates, AbsenceRange absenceRange)
        {
            if (absenceRange == null)
                return null;

            return dates.Select(d =>
            {
                var IsBeginDate = absenceRange.BeginDateUtc.Date == d.Date;
                var IsEndDate = absenceRange.EndDateUtc.Date == d.Date;
                var IsAM = IsBeginDate
                        ? absenceRange.BeginAbsencePart != AbsencePart.HalfDayPM
                        : IsEndDate
                            ? absenceRange.EndAbsencePart != AbsencePart.HalfDayPM
                            : true;
                var IsPM = IsBeginDate
                        ? absenceRange.BeginAbsencePart != AbsencePart.HalfDayAM
                        : IsEndDate
                            ? absenceRange.EndAbsencePart != AbsencePart.HalfDayAM
                            : true;

                return new AbsenceDay
                {
                    AbsenceId = absenceRange.AbsenceId.HasValue ? absenceRange.AbsenceId.Value : 0,
                    Date = d,
                    AM = IsAM,
                    PM = IsPM,
                    Duration = CalculateAbsenceDayDuration(IsAM, IsPM),
                };
            });

        }

        private static double CalculateAbsenceDayDuration(bool am, bool pm)
        {
            return (am ? 0.5 : 0) + (pm ? 0.5 : 0);
        }
    }
}