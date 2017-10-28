using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
//using DocumentService.API.RESTClient.Interfaces;
using FluentAssertions;
using FluentAssertions.Equivalency;
using HR.Business.Interfaces;
using HR.Data.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Entity.Interfaces;
using Moq;
using NUnit.Framework;

namespace HR.Business.UnitTests
{
    [TestFixture]
    public class AbsenceCannotBookTests
    {
        private Mock<IHRDataService> _mockHRDataService;
        private Mock<ICacheProvider> _mockCacheProvider;
        private Mock<ITemplateService> _mockTemplateService;
        private Mock<IEmailService> _mockEmailService;
        //private Mock<IDocumentServiceRestClient> _mockDocumentServiceRestClient;
        private static WorkingPattern _workingPattern => new WorkingPattern()
        {
            WorkingPatternDays = new List<WorkingPatternDay>()
                {
                    new WorkingPatternDay {DayOfWeek = 1, AM = true, PM = false},   //Monday 30-01-2017
                    new WorkingPatternDay {DayOfWeek = 2, AM = false, PM = true},   //Tuesday 31-01-2017  
                    new WorkingPatternDay {DayOfWeek = 3, AM = false, PM = false},  //Wednesday 01-02-2017
                    new WorkingPatternDay {DayOfWeek = 4, AM = true, PM = true},    //Thursday 02-02-2017
                    new WorkingPatternDay {DayOfWeek = 5, AM = false, PM = true},   //Friday 03-02-2017
                    new WorkingPatternDay {DayOfWeek = 6, AM = false, PM = true},   //Saturday 04-02-2017
                    new WorkingPatternDay {DayOfWeek = 0, AM = false, PM = true},   //sunday 05-02-2017
                }
        };

        private IHRBusinessService _hrBusinessService;

        [SetUp]
        public void Setup()
        {
            _mockHRDataService = new Mock<IHRDataService>();
            _mockCacheProvider = new Mock<ICacheProvider>();
            _mockTemplateService = new Mock<ITemplateService>();
            _mockEmailService = new Mock<IEmailService>();
            //_mockDocumentServiceRestClient = new Mock<IDocumentServiceRestClient>();

            //_hrBusinessService = new HRBusinessService(_mockHRDataService.Object, _mockCacheProvider.Object
            //_mockTemplateService.Object, _mockEmailService.Object, _mockDocumentServiceRestClient.Object);

        }

        public static IEnumerable CannotBookAbsenceDaysTestCases
        {
            get
            {
                //Cannot Book Half Day PM as non working day 
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 01, 30), EndDateUtc = new DateTime(2017, 01, 30), BeginAbsencePart = AbsencePart.HalfDayPM },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>(),
                    new List<INotAbsenceDay>
                    {
                        new CanBeBookedWorkingPatternDay { Date = new DateTime(2017, 01, 30), AM = true, PM = false },
                    })
                    .SetName("RetrieveCannotBeBookedDays: cannot book half day PM absence as non working day , AM can be booked");

                //Cannot Book Half Day AM as non working day 
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 01, 31), EndDateUtc = new DateTime(2017, 01, 31), BeginAbsencePart = AbsencePart.HalfDayAM },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>(),
                    new List<INotAbsenceDay>
                    {
                        new CanBeBookedWorkingPatternDay { Date = new DateTime(2017, 01, 31), AM = false, PM = true },
                    })
                  .SetName("RetrieveCannotBeBookedDays: cannot book half day AM absence as non working day , PM can be booked");

                //Cannot Book Full Day
                yield return new TestCaseData(
              new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 01), EndDateUtc = new DateTime(2017, 02, 01), BeginAbsencePart = AbsencePart.FullDay },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>(),
                    new List<INotAbsenceDay> { new CanBeBookedWorkingPatternDay { Date = new DateTime(2017, 02, 01), AM = false, PM = false } })
                 .SetName("RetrieveCannotBeBookedDays: cannot book full day absence as a non working day");

                //Full Day Can Be Booked
                yield return new TestCaseData(
              new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 02), EndDateUtc = new DateTime(2017, 02, 02), BeginAbsencePart = AbsencePart.FullDay },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>(),
                     new List<INotAbsenceDay>())
                 .SetName("RetrieveCannotBeBookedDays: full day absence can be booked");

                // PM Already Booked,AM Cannot Be Booked  (Tuesday 31-01-2017)
                yield return new TestCaseData(
              new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 03), EndDateUtc = new DateTime(2017, 02, 03), BeginAbsencePart = AbsencePart.HalfDayAM },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>() { new AbsenceDay() { PM = true, Date = new DateTime(2017, 02, 03) } },
                     new List<INotAbsenceDay>
                    {
                        new NotAbsenceDay { Date = new DateTime(2017, 02, 03),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedWorkingPatternDay { Date = new DateTime(2017, 02, 03), AM = false, PM = true },
                            new CanBeBookedAbsenceDay() { Date = new DateTime(2017, 02, 03), AM = true, PM = false },
                        } }
                    })
                 .SetName("RetrieveCannotBeBookedDays: cannot book half day AM absence as non working day , PM Already Booked");

                //AM Already Booked,PM Cannot Be Booked   (Monday 30-01-2017)
                yield return new TestCaseData(
              new AbsenceRange { BeginDateUtc = new DateTime(2017, 01, 30), EndDateUtc = new DateTime(2017, 01, 30), BeginAbsencePart = AbsencePart.HalfDayAM },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay> { new AbsenceDay() { AM = true, Date = new DateTime(2017, 01, 30) } },
                    new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 01, 30),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedWorkingPatternDay() {AM = true,PM = false,Date = new DateTime(2017, 01, 30)},
                            new CanBeBookedAbsenceDay(){ AM = false, PM = true, Date = new DateTime(2017, 01, 30) }
                        } }
                    })
                 .SetName("RetrieveCannotBeBookedDays: cannot book half day PM absence as non working day, AM Already Booked");

                //Full Day Already Booked (Thursday 02-02-2017)
                yield return new TestCaseData(
              new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 02), EndDateUtc = new DateTime(2017, 02, 02), BeginAbsencePart = AbsencePart.FullDay },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>() { new AbsenceDay() { AM = true, PM = true, Date = new DateTime(2017, 02, 02) } },
                      new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 02, 02),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedAbsenceDay(){ AM = false, PM = false, Date = new DateTime(2017, 02, 02) }
                        } }
                    })
                 .SetName("RetrieveCannotBeBookedDays: full day already booked");

                //AM Already Booked, PM Can Be Booked (Thursday 02-02-2017)
                yield return new TestCaseData(
              new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 02), EndDateUtc = new DateTime(2017, 02, 02), BeginAbsencePart = AbsencePart.HalfDayPM },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>() { new AbsenceDay() { AM = true, Date = new DateTime(2017, 02, 02) } },
                    new List<CanBeBookedAbsenceDay>() { new CanBeBookedAbsenceDay() { PM = true, Date = new DateTime(2017, 02, 02) } })
                 .SetName("RetrieveCannotBeBookedDays: cannot book half day AM absence as already booked, PM can be booked");

                //PM Already Booked, AM Can Be Booked (Thursday 02-02-2017)
                yield return new TestCaseData(
              new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 02), EndDateUtc = new DateTime(2017, 02, 02), BeginAbsencePart = AbsencePart.HalfDayAM },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>() { new AbsenceDay() { PM = true, Date = new DateTime(2017, 02, 02) } },
                     new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 02, 02),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedAbsenceDay(){ AM = true, PM = false, Date = new DateTime(2017, 02, 02) }
                        } }
                    })
                .SetName("RetrieveCannotBeBookedDays: cannot book half day PM absence as already booked, AM can be booked");

                //Public Holiday Test Cases

                //Cannot Book full day (Working Day) On Public Holiday
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 02), EndDateUtc = new DateTime(2017, 02, 02), BeginAbsencePart = AbsencePart.FullDay },
                    _workingPattern,
                    new List<PublicHoliday>() { new PublicHoliday() { Date = new DateTime(2017, 02, 02), Name = "Public Holiday" } },
                    new List<AbsenceDay>(),
                    new List<INotAbsenceDay> { new PublicHoliday() { Date = new DateTime(2017, 02, 02), AM = false, PM = false, Name = "Public Holiday" } })
                    .SetName("RetrieveCannotBeBookedDays: cannot book full day absence on public holiday.");

                //Cannot Book Half Day AM (Half Day Working Day) On Public Holiday
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 01, 30), EndDateUtc = new DateTime(2017, 01, 30), BeginAbsencePart = AbsencePart.HalfDayAM },
                    _workingPattern,
                    new List<PublicHoliday>() { new PublicHoliday() { Date = new DateTime(2017, 01, 30), Name = "Public Holiday" } },
                    new List<AbsenceDay>(),
                   new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 01, 30),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedWorkingPatternDay() {AM = true,PM = false,Date = new DateTime(2017, 01, 30)},
                            new PublicHoliday(){ AM = false, PM = false, Date = new DateTime(2017, 01, 30),Name = "Public Holiday" }
                        } }
                    })
                  .SetName("RetrieveCannotBeBookedDays: cannot book half day AM absence as on public holiday");

                //Cannot Book Half Day PM (Half Day Working Day) On Public Holiday and AM is not working
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 01, 31), EndDateUtc = new DateTime(2017, 01, 31), BeginAbsencePart = AbsencePart.HalfDayPM },
                    _workingPattern,
                    new List<PublicHoliday>() { new PublicHoliday() { Date = new DateTime(2017, 01, 31), Name = "Public Holiday" } },
                    new List<AbsenceDay>(),
                     new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 01, 31),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedWorkingPatternDay() {AM = false,PM = true,Date = new DateTime(2017, 01, 31)},
                            new PublicHoliday(){ AM = false, PM = false, Date = new DateTime(2017, 01, 31),Name = "Public Holiday" }
                        } }
                    })
                 .SetName("RetrieveCannotBeBookedDays: cannot book half day PM absence as on public holiday");

                //Cannot Book Full Day On Non Working Day And Public Holiday (01-02-2017)
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 01), EndDateUtc = new DateTime(2017, 02, 02), BeginAbsencePart = AbsencePart.FullDay },
                    _workingPattern,
                    new List<PublicHoliday>() { new PublicHoliday()
                    {
                        Date = new DateTime(2017, 02, 01), Name = "Public Holiday" ,
                    } , new PublicHoliday()
                    {
                        Date = new DateTime(2017, 02, 02), Name = "Public Holiday" ,
                    } },
                    new List<AbsenceDay>(),
                    new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 02, 01),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedWorkingPatternDay() {AM = false,PM = false,Date = new DateTime(2017, 02, 01)},
                            new PublicHoliday(){ AM = false, PM = false, Date = new DateTime(2017, 02, 01),Name = "Public Holiday" }
                        } },
                         new NotAbsenceDay() { Date = new DateTime(2017, 02, 02),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new PublicHoliday(){ AM = false, PM = false, Date = new DateTime(2017, 02, 02) ,Name = "Public Holiday"}
                        } }
                    })
                 .SetName("RetrieveCannotBeBookedDays: cannot book full day absence on same day having non working and public holiday");

                //Cannot Book Half Day AM On Non Working Day And Public Holiday (01-02-2017)
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 01), EndDateUtc = new DateTime(2017, 02, 02), BeginAbsencePart = AbsencePart.HalfDayAM },
                    _workingPattern,
                    new List<PublicHoliday>() { new PublicHoliday()
                    {
                        Date = new DateTime(2017, 02, 01), Name = "Public Holiday" ,

                    } , new PublicHoliday()
                    {
                        Date = new DateTime(2017, 02, 02), Name = "Public Holiday" ,
                    } },
                    new List<AbsenceDay>(),
                    new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 02, 01),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedWorkingPatternDay() {AM = false,PM = false,Date = new DateTime(2017, 02, 01)},
                            new PublicHoliday(){ AM = false, PM = false, Date = new DateTime(2017, 02, 01),Name = "Public Holiday" }
                        } },
                         new NotAbsenceDay() { Date = new DateTime(2017, 02, 02),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new PublicHoliday(){ AM = false, PM = false, Date = new DateTime(2017, 02, 02) ,Name = "Public Holiday"}
                        } }
                    })
                 .SetName("RetrieveCannotBeBookedDays: cannot book full day absence on same day having non working and public holiday");

                //Cannot Book Half Day PM On Non Working Day And Public Holiday (01-02-2017)
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 01), EndDateUtc = new DateTime(2017, 02, 02), BeginAbsencePart = AbsencePart.HalfDayPM },
                    _workingPattern,
                    new List<PublicHoliday>() { new PublicHoliday()
                    {
                        Date = new DateTime(2017, 02, 01), Name = "Public Holiday" ,

                    } , new PublicHoliday()
                    {
                        Date = new DateTime(2017, 02, 02), Name = "Public Holiday" ,

                    } },
                    new List<AbsenceDay>(),
                    new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 02, 01),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedWorkingPatternDay() {AM = false,PM = false,Date = new DateTime(2017, 02, 01)},
                            new PublicHoliday(){ AM = false, PM = false, Date = new DateTime(2017, 02, 01),Name = "Public Holiday" }
                        } },
                         new NotAbsenceDay() { Date = new DateTime(2017, 02, 02),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new PublicHoliday(){ AM = false, PM = false, Date = new DateTime(2017, 02, 02) ,Name = "Public Holiday"}
                        } }
                    })
                 .SetName("RetrieveCannotBeBookedDays: cannot book half day PM absence on same day having non working and public holiday");

                //cannot book half day PM absence as already booked
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 03), EndDateUtc = new DateTime(2017, 02, 03), BeginAbsencePart = AbsencePart.HalfDayPM },
                    _workingPattern,
                    new List<PublicHoliday>(),
                    new List<AbsenceDay>() { new AbsenceDay() { PM = true, Date = new DateTime(2017, 02, 03), AbsenceId = 1 } },
                    new List<INotAbsenceDay>()
                    {
                        new NotAbsenceDay() { Date = new DateTime(2017, 02, 03),NotAbsenceDays = new List<INotAbsenceDay>()
                        {
                            new CanBeBookedWorkingPatternDay() {AM = false,PM = true,Date = new DateTime(2017, 02, 03)},
                            new CanBeBookedAbsenceDay(){ AM = true, PM = false, Date = new DateTime(2017, 02, 03)}
                        } }
                    })
                 .SetName("RetrieveCannotBeBookedDays: cannot book half day PM absence as already booked");
            }
        }

        public static IEnumerable AlreadyBookedAbsenceDaysTestCases
        {
            get
            {
                //Halfday AM Already Booked
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 03), EndDateUtc = new DateTime(2017, 02, 03), BeginAbsencePart = AbsencePart.HalfDayAM },
                    new List<AbsenceDay>() { new AbsenceDay() { AM = true, Date = new DateTime(2017, 02, 03) } },
                    new List<CanBeBookedAbsenceDay>() { new CanBeBookedAbsenceDay() { AM = false, PM = true, Date = new DateTime(2017, 02, 03) } })
                 .SetName("RetrieveAlreadyBookedAbsenceDays: Halfday AM Already Booked");

                //Halfday PM Already Booked
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 03), EndDateUtc = new DateTime(2017, 02, 03), BeginAbsencePart = AbsencePart.HalfDayPM },
                    new List<AbsenceDay>() { new AbsenceDay() { PM = true, Date = new DateTime(2017, 02, 03) } },
                    new List<CanBeBookedAbsenceDay>() { new CanBeBookedAbsenceDay() { AM = true, PM = false, Date = new DateTime(2017, 02, 03) } })
                 .SetName("RetrieveAlreadyBookedAbsenceDays: Halfday PM Already Booked");

                //Fullday Already Booked
                yield return new TestCaseData(
                    new AbsenceRange { BeginDateUtc = new DateTime(2017, 02, 03), EndDateUtc = new DateTime(2017, 02, 03), BeginAbsencePart = AbsencePart.HalfDayPM },
                    new List<AbsenceDay>() { new AbsenceDay() { AM = true, PM = true, Date = new DateTime(2017, 02, 03) } },
                    new List<CanBeBookedAbsenceDay>() { new CanBeBookedAbsenceDay() { AM = false, PM = false, Date = new DateTime(2017, 02, 03) } })
                 .SetName("RetrieveAlreadyBookedAbsenceDays: Fullday Already Booked");
            }
        }


        [Test, TestCaseSource(nameof(CannotBookAbsenceDaysTestCases))]
        public void AbsenceDaysCannotBookTests(AbsenceRange absenceRange, WorkingPattern workingPattern, IEnumerable<PublicHoliday> publicHolidays, IEnumerable<AbsenceDay> alreadyBookedAbsenceDays, IEnumerable<INotAbsenceDay> expectedAbsenceDays)
        {
            // Arrange
            _mockHRDataService.Setup(m => m.RetrievePublicHolidays(It.IsAny<Int32>(), It.IsAny<Int32>(), It.IsAny<Expression<Func<PublicHoliday, bool>>>(), It.IsAny<List<OrderBy>>(), It.IsAny<Paging>())).Returns(PagedResult<PublicHoliday>.Create(publicHolidays, 1, publicHolidays.Count(), 1, publicHolidays.Count()));
            _mockHRDataService.Setup(m => m.RetrieveAbsenceRangeBookedAbsenceDays(It.IsAny<AbsenceRange>()))
                .Returns(alreadyBookedAbsenceDays);
            // Act
            var actualAbsenceDays = _hrBusinessService.RetrieveCannotBeBookedDays(absenceRange, workingPattern);
            // Assert
            actualAbsenceDays.ShouldBeEquivalentTo(expectedAbsenceDays);
            _mockHRDataService.Verify(m => m.RetrievePublicHolidays(It.IsAny<Int32>(), It.IsAny<Int32>(), It.IsAny<Expression<Func<PublicHoliday, bool>>>(), It.IsAny<List<OrderBy>>(), It.IsAny<Paging>()), Times.Once);
            _mockHRDataService.Verify(m => m.RetrieveAbsenceRangeBookedAbsenceDays(It.IsAny<AbsenceRange>()), Times.Once);
        }

        [Test, TestCaseSource(nameof(AlreadyBookedAbsenceDaysTestCases))]
        public void AbsenceDaysAlreadyBookedTests(AbsenceRange absenceRange, IEnumerable<AbsenceDay> alreadyBookedAbsences, IEnumerable<CanBeBookedAbsenceDay> expectedAbsenceRanges)
        {
            // Arrange

            _mockHRDataService.Setup(m => m.RetrieveAbsenceRangeBookedAbsenceDays(It.IsAny<AbsenceRange>()))
                .Returns(alreadyBookedAbsences);
            // Act
            var actualAlreadyBookedAbsenceDays = _hrBusinessService.RetrieveAlreadyBookedAbsencesExcludingCurrentAbsence(absenceRange);
            // Assert
            actualAlreadyBookedAbsenceDays.ShouldBeEquivalentTo(expectedAbsenceRanges);
            _mockHRDataService.Verify(m => m.RetrieveAbsenceRangeBookedAbsenceDays(It.IsAny<AbsenceRange>()), Times.Once);
        }

    }
}
