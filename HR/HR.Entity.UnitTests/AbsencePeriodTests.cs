using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HR.Entity.UnitTests
{
    [TestFixture]
    public class AbsencePeriodTests

    {
        public static IEnumerable<AbsencePeriod> AbsencePeriodsUnassigned;
        public static IEnumerable<AbsencePeriod> AbsencePeriodsAssigned;

        public static IEnumerable OverlappingAbsencePeriod
        {
            get
            {
                yield return new TestCaseData(
                    new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-01-2014"), EndDate = DateTime.Parse("01-01-2015") } }.AsQueryable(),
                    new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-01-2014"), EndDate = DateTime.Parse("01-01-2015") } }.AsQueryable(),
                    new List<AbsencePeriod>()
            ).SetName("Current AbsencePeriod is overlapping");

                yield return new TestCaseData(
                   new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-01-2014"), EndDate = DateTime.Parse("01-01-2015") } }.AsQueryable(),
                   new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-10-2018"), EndDate = DateTime.Parse("01-04-2018") } }.AsQueryable(),
                   new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-10-2018"), EndDate = DateTime.Parse("01-04-2018") } }
           ).SetName("Current AbsencePeriod not Overlapping");

                yield return new TestCaseData(
                   new List<AbsencePeriod>()
                   {
                       new AbsencePeriod { StartDate = DateTime.Parse("01-02-2015"), EndDate = DateTime.Parse("01-01-2015")}
                   }.AsQueryable(),
                   new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("31-12-2014"), EndDate = DateTime.Parse("01-11-2014") } }.AsQueryable(),
                   new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("31-12-2014"), EndDate = DateTime.Parse("01-11-2014") } }
           ).SetName("Current AbsencePeriod is not overlapping and date range lies before assigned absence period");

                yield return new TestCaseData(
                  new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-02-2015"), EndDate = DateTime.Parse("01-01-2015") } }.AsQueryable(),
                  new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("03-03-2015"), EndDate = DateTime.Parse("02-02-2015") } }.AsQueryable(),
                  new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("03-03-2015"), EndDate = DateTime.Parse("02-02-2015") } }
          ).SetName("Current AbsencePeriod is not overlapping and date range lies after assigned absence period");

                yield return new TestCaseData(
                    new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-01-2015"), EndDate = DateTime.Parse("31-01-2015") } }.AsQueryable(),
                    new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("31-03-2014"), EndDate = DateTime.Parse("01-01-2015") } }.AsQueryable(),
                     new List<AbsencePeriod>()
        ).SetName("start date is equal to end date of assigned absence period");

                yield return new TestCaseData(
                    new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-01-2015"), EndDate = DateTime.Parse("31-01-2015") } }.AsQueryable(),
                    new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-12-2014"), EndDate = DateTime.Parse("01-01-2015") } }.AsQueryable(),
                    new List<AbsencePeriod>()
        ).SetName("end date is equal to start date of assigned absence period");

                yield return new TestCaseData(
                   new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("01-01-2015"), EndDate = DateTime.Parse("28-02-2015") } }.AsQueryable(),
                   new List<AbsencePeriod> { new AbsencePeriod { StartDate = DateTime.Parse("21-01-2015"), EndDate = DateTime.Parse("15-02-2015") } }.AsQueryable(),
                   new List<AbsencePeriod>()
       ).SetName("start date and end date is in between of assigned absence period");

            }

        }


        [Test, TestCaseSource(nameof(OverlappingAbsencePeriod))]
        public void OverlapsWithTests(IQueryable<AbsencePeriod> assignedAbsencePeriods,
            IQueryable<AbsencePeriod> unAssignedAbsencePeriod, List<AbsencePeriod> expectedAbsencePeriods)
        {

            //Act
            var actual = unAssignedAbsencePeriod.Where(ua => !ua.OverlapsWithAny(assignedAbsencePeriods)).ToList();
            //Assert
            actual.ShouldBeEquivalentTo(expectedAbsencePeriods);
        }


        public static IEnumerable IsCurrentPeriodTestCases
        {
            get
            {
                yield return new TestCaseData(new AbsencePeriod { StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(1) }, true)
                    .SetName("IsCurrentPeriod returns true when current date is in AbsencePeriod");

                yield return new TestCaseData(new AbsencePeriod { StartDate = DateTime.Today.AddYears(-2), EndDate = DateTime.Today.AddYears(-1) }, false)
                    .SetName("IsCurrentPeriod returns false when current date is not in AbsencePeriod");

                yield return new TestCaseData(new AbsencePeriod { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1) }, true)
                    .SetName("IsCurrentPeriod returns true when current date equals AbsencePeriod.StartDate");

                yield return new TestCaseData(new AbsencePeriod { StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today }, true)
                    .SetName("IsCurrentPeriod returns true when current date equals AbsencePeriod.EndDate");

                yield return new TestCaseData(new AbsencePeriod { StartDate = DateTime.Today.AddDays(-2), EndDate = DateTime.Today.AddDays(-1) }, false)
                    .SetName("IsCurrentPeriod returns false when current date is greater then start date but is also greater than end date");

            }

        }

        [Test, TestCaseSource(nameof(IsCurrentPeriodTestCases))]
        public void IsCurrentPeriodTests(AbsencePeriod absencePeriod, bool expectedResult)
        {
            //Arrange

            //Act
            var actual = absencePeriod.IsCurrentPeriod();

            //Assert
            actual.Should().Be(expectedResult);

        }
    }
}
