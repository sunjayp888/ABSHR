using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HR.Business.Extensions
{
    public static class WorkingPatternExtensions
    {
        public static IEnumerable<CanBeBookedWorkingPatternDay> ToWorkingPatternNotAbsenceDays(this IEnumerable<WorkingPatternDay> workingPatternDays, DateTime beginDate, DateTime endDate)
        {
            if (workingPatternDays == null)
                return null;

            /// Get Not Absence Days
            var notAbsenceDays = workingPatternDays.Where(w => !w.AM || !w.PM).ToList();

            var days = beginDate.RangeTo(endDate);

            return (from day in days
                    join notAbsenceDay in notAbsenceDays on day.DayOfWeek equals notAbsenceDay.AsDayOfWeek
                    select new CanBeBookedWorkingPatternDay
                    {
                        Date = day,
                        AM = notAbsenceDay.AM,
                        PM = notAbsenceDay.PM
                    }).ToList();

        }
    }
}
