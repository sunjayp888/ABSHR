using FluentAssertions;
using HR.Business.Extensions;
using HR.Entity;
using HR.Entity.Dto;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HR.Business.UnitTests
{
    [TestFixture]
    public class WorkingPatternExtensionsTests
    {

        public static IEnumerable ToWorkingPatternNotAbsenceDays
        {
            get
            {
                yield return new TestCaseData(null, null, null, null)
                    .SetName("ToWorkingPatternNotAbsenceDays: returns null If workingPatternDays is null");

                yield return new TestCaseData(
                    new List<WorkingPatternDay>
                    {
                        new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 0, AM = true, PM = true },
                    }, 
                    null, 
                    null, 
                    new List<CanBeBookedWorkingPatternDay> { })
                    .SetName("ToWorkingPatternNotAbsenceDays: returns empty List If BeginDate is null");

                yield return new TestCaseData(
                    new List<WorkingPatternDay>
                    {
                        new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 0, AM = true, PM = true },
                    },
                    new DateTime(2016, 12, 7),
                    null,
                    new List<CanBeBookedWorkingPatternDay> { })
                    .SetName("ToWorkingPatternNotAbsenceDays: returns empty list If EndDate is null");


                yield return new TestCaseData(
                   new List<WorkingPatternDay>
                   {
                        new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 0, AM = true, PM = true },
                   },
                   new DateTime(2016, 12, 5),
                   new DateTime(2016, 12, 11),
                   new List<CanBeBookedWorkingPatternDay> { })
                   .SetName("ToWorkingPatternNotAbsenceDays: returns empty list If all working pattern days are true");

                yield return new TestCaseData(
                   new List<WorkingPatternDay>
                   {
                        new WorkingPatternDay { DayOfWeek = 1, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 2, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 3, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 4, AM = false, PM = true },
                        new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 0, AM = true, PM = true },
                   },
                   new DateTime(2016, 12, 5),
                   new DateTime(2016, 12, 11),
                   new List<CanBeBookedWorkingPatternDay> {
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 5), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 6), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 7), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 8), AM = false, PM = true }
                   })
                   .SetName("ToWorkingPatternNotAbsenceDays: returns list of CanBeBookedWorkingPatternDay");




                yield return new TestCaseData(
                  new List<WorkingPatternDay>
                  {
                        new WorkingPatternDay { DayOfWeek = 1, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = false },
                        new WorkingPatternDay { DayOfWeek = 3, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 4, AM = false, PM = true },
                        new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                        new WorkingPatternDay { DayOfWeek = 0, AM = true, PM = true },
                  },
                  new DateTime(2016, 12, 5),
                  new DateTime(2016, 12, 24),
                  new List<CanBeBookedWorkingPatternDay> {
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 5), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 6), AM = true, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 7), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 8), AM = false, PM = true },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 12), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 13), AM = true, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 14), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 15), AM = false, PM = true },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 19), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 20), AM = true, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 21), AM = false, PM = false },
                       new CanBeBookedWorkingPatternDay { Date = new DateTime(2016, 12, 22), AM = false, PM = true }
                  })
                  .SetName("ToWorkingPatternNotAbsenceDays: multiple weeks");
            }
        }

        [Test, TestCaseSource(nameof(ToWorkingPatternNotAbsenceDays))]
        public void ToWorkingPatternNotAbsenceDaysTest(IEnumerable<WorkingPatternDay> workingPatternDays, DateTime beginDate, DateTime endDate, IEnumerable<CanBeBookedWorkingPatternDay> expectedWorkingPatternNotAbsenceDays)
        {
            // Arrange

            // Act
            var actualWorkingPatternNotAbsenceDays = workingPatternDays.ToWorkingPatternNotAbsenceDays(beginDate, endDate);


            // Assert
            actualWorkingPatternNotAbsenceDays.ShouldBeEquivalentTo(expectedWorkingPatternNotAbsenceDays);
        }
    }
}
