using FluentAssertions;
using HR.Entity;
using HR.Entity.Dto;
using HR.Entity.Interfaces;
using HR.Extensions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HR.Business.UnitTests
{
    [TestFixture]
    public class AbsenceExtensionTests
    {

        public static IEnumerable ToAbsenceDayListTestCases
        {
            get
            {
                yield return new TestCaseData(DateTime.Today.RangeTo(DateTime.Today.AddDays(5)), null, null)
                    .SetName("ToAbsenceDayList: returns null If absenceRange is null");

                yield return new TestCaseData(
                    DateTime.Today.RangeTo(DateTime.Today.AddDays(5)),
                    new AbsenceRange { BeginDateUtc = DateTime.Today, BeginAbsencePart = AbsencePart.FullDay, EndDateUtc = DateTime.Today.AddDays(5), EndAbsencePart = AbsencePart.FullDay },
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = DateTime.Today, AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = DateTime.Today.AddDays(2), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = DateTime.Today.AddDays(3), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = DateTime.Today.AddDays(4), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = DateTime.Today.AddDays(5), AM = true, PM = true, Duration = 1 },
                    })
                    .SetName("ToAbsenceDayList: from new Absence returns FullDay IEnumerable<AbsenceDay>");

                yield return new TestCaseData(
                    DateTime.Today.RangeTo(DateTime.Today.AddDays(5)),
                    new AbsenceRange { AbsenceId = 1, BeginDateUtc = DateTime.Today, BeginAbsencePart = AbsencePart.FullDay, EndDateUtc = DateTime.Today.AddDays(5), EndAbsencePart = AbsencePart.FullDay },
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(2), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(3), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(4), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(5), AM = true, PM = true, Duration = 1 },
                    })
                    .SetName("ToAbsenceDayList: from existing Absence returns FullDay IEnumerable<AbsenceDay>");


                yield return new TestCaseData(
                    DateTime.Today.RangeTo(DateTime.Today.AddDays(5)),
                    new AbsenceRange { AbsenceId = 1, BeginDateUtc = DateTime.Today, BeginAbsencePart = AbsencePart.HalfDayPM, EndDateUtc = DateTime.Today.AddDays(5), EndAbsencePart = AbsencePart.HalfDayAM },
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = false, PM = true, Duration = 0.5 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(2), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(3), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(4), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(5), AM = true, PM = false, Duration = 0.5 },
                    })
                    .SetName("ToAbsenceDayList: multiple days with full day and half day am and half day pm");


                yield return new TestCaseData(
                    DateTime.Today.RangeTo(DateTime.Today),
                    new AbsenceRange { AbsenceId = 1, BeginDateUtc = DateTime.Today, BeginAbsencePart = AbsencePart.HalfDayAM, EndDateUtc = DateTime.Today, EndAbsencePart = AbsencePart.HalfDayAM },
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = true, PM = false, Duration = 0.5 },
                    })
                    .SetName("ToAbsenceDayList: single half day am");

                yield return new TestCaseData(
                   DateTime.Today.RangeTo(DateTime.Today),
                   new AbsenceRange { AbsenceId = 1, BeginDateUtc = DateTime.Today, BeginAbsencePart = AbsencePart.HalfDayPM, EndDateUtc = DateTime.Today, EndAbsencePart = AbsencePart.HalfDayPM },
                   new List<AbsenceDay>
                   {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = false, PM = true, Duration = 0.5 },
                   })
                   .SetName("ToAbsenceDayList: single half day pm");
            }
        }

        [Test, TestCaseSource(nameof(ToAbsenceDayListTestCases))]
        public void ToAbsenceDayListTests(IEnumerable<DateTime> dates, AbsenceRange absenceRange, IEnumerable<AbsenceDay> expectedAbsenceDays)
        {
            // Arrange

            // Act
            var actualAbsenceDays = dates.ToAbsenceDayList(absenceRange);


            // Assert
            actualAbsenceDays.ShouldBeEquivalentTo(expectedAbsenceDays);
        }


        public static IEnumerable FilterNotAbsenceDaysTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = true, PM = true, Duration = 1 }
                    },
                    null,
                    false,
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = true, PM = true, Duration = 1 }
                    })
                    .SetName("ToAbsenceDaysSummary: returns absenceDays if notAbsenceDays is null");


                yield return new TestCaseData(
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(2), AM = true, PM = true, Duration = 1 },
                    },
                    new List<INotAbsenceDay>
                    {
                        new CanBeBookedWorkingPatternDay { Date = DateTime.Today, AM = false, PM = false }
                    },
                    false,
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(2), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                    })
                    .SetName("ToAbsenceDaysSummary: notAbsenceDays removed from absenceDays");


                yield return new TestCaseData(
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(2), AM = true, PM = true, Duration = 1 },
                    },
                    new List<INotAbsenceDay>
                    {
                        new CanBeBookedWorkingPatternDay { Date = DateTime.Today, AM = false, PM = false }
                    },
                    true,
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = false, PM = false, Duration = 0, CanBeBookedAsAbsence = false, Validation = "Not a working day." },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(2), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                    })
                    .SetName("ToAbsenceDaysSummary: include notAbsenceDays with validation from absenceDays");


                yield return new TestCaseData(
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(2), AM = true, PM = true, Duration = 1 },
                    },
                    new List<INotAbsenceDay>
                    {
                        new CanBeBookedWorkingPatternDay { Date = DateTime.Today, AM = false, PM = true },
                        new CanBeBookedWorkingPatternDay { Date = DateTime.Today.AddDays(1), AM = true, PM = true },
                        new CanBeBookedWorkingPatternDay { Date = DateTime.Today.AddDays(2), AM = true, PM = false },
                    },
                    false,
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today, AM = false, PM = true, Duration = 0.5, CanBeBookedAsAbsence = true,Validation = "AM not a working day cannot be booked." },
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true ,Validation = string.Empty},
                        new AbsenceDay { AbsenceId = 1, Date = DateTime.Today.AddDays(2), AM = true, PM = false, Duration = 0.5, CanBeBookedAsAbsence = true ,Validation = "PM not a working day cannot be booked."},
                    })
                    .SetName("ToAbsenceDaysSummary: notAbsenceDays half days from absenceDays");
            }
        }


        [Test, TestCaseSource(nameof(FilterNotAbsenceDaysTestCases))]
        public void FilterNotAbsenceDaysTests(IEnumerable<AbsenceDay> absenceDays, IEnumerable<INotAbsenceDay> notAbsenceDays, bool returnUnbookableDays, IEnumerable<AbsenceDay> expectedAbsenceDays)
        {
            // Arrange

            // Act
            var actualAbsenceDays = absenceDays.ToAbsenceDaysSummary(notAbsenceDays, returnUnbookableDays);


            // Assert
            actualAbsenceDays.ShouldBeEquivalentTo(expectedAbsenceDays);
        }


    }
}
