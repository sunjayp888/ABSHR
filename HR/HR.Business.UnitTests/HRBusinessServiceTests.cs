//using DocumentService.API.RESTClient.Interfaces;
using FluentAssertions;
using FluentAssertions.Equivalency;
using HR.Business.Interfaces;
using HR.Business.Models;
using HR.Data.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HR.Business.UnitTests
{
    [TestFixture]
    public class HRBusinessServiceTests
    {
        private static Mock<IHRDataService> _mockHRDataService;
        private static Mock<ICacheProvider> _mockCacheProvider;
        private static Mock<ITemplateService> _mockTemplateService;
        private static Mock<IEmailService> _mockEmailService;
        //private static Mock<IDocumentServiceRestClient> _mockDocumentServiceRestClient;

        private IHRBusinessService _hrBusinessService;

        [SetUp]
        public void Setup()
        {
            _mockHRDataService = new Mock<IHRDataService>();
            _mockCacheProvider = new Mock<ICacheProvider>();
            _mockTemplateService = new Mock<ITemplateService>();
            _mockEmailService = new Mock<IEmailService>();
            //_mockDocumentServiceRestClient = new Mock<IDocumentServiceRestClient>();

           // _hrBusinessService = new HRBusinessService(_mockHRDataService.Object, _mockCacheProvider.Object, _mockTemplateService.Object, _mockEmailService.Object, _mockDocumentServiceRestClient.Object);
        }

        private void CheckList(IAssertionContext<IEnumerable> a)
        {
            if (a.Expectation == null)
                a.Subject.Should().BeNull();
            else
                a.Subject.ShouldBeEquivalentTo(a.Expectation, opt => opt
                    .Using<IEnumerable>(CheckList)
                    .When(info => typeof(IEnumerable).IsAssignableFrom(info.CompileTimeType)));
        }

        public static IEnumerable AbsenceRangeToAbsenceDaysTestCases
        {
            get
            {
                yield return new TestCaseData(null, false, null, null, null, null)
                    .SetName("AbsenceRangeToAbsenceDays: returns null If absenceRange is null");


                yield return new TestCaseData(
                    new AbsenceRange
                    {
                        PersonnelId = 1,
                        BeginDateUtc = DateTime.Today,
                        EndDateUtc = DateTime.Today.AddDays(1),
                        CountryId = 1,
                        BeginAbsencePart = AbsencePart.FullDay,
                        EndAbsencePart = AbsencePart.FullDay
                    },
                    false,
                    null,
                    null,
                    null,
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = DateTime.Today, AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = DateTime.Today.AddDays(1), AM = true, PM = true, Duration = 1 }
                    })
                    .SetName("AbsenceRangeToAbsenceDays: returns all valid days when workingPattern, CountryPublicHolidays and AlreadyBookedAbsenceDays are null");

                yield return new TestCaseData(
                    new AbsenceRange
                    {
                        PersonnelId = 1,
                        BeginDateUtc = new DateTime(2016, 12, 5),
                        EndDateUtc = new DateTime(2016, 12, 10),
                        CountryId = 1,
                        BeginAbsencePart = AbsencePart.FullDay,
                        EndAbsencePart = AbsencePart.FullDay
                    },
                    false,
                    new WorkingPattern
                    {
                        WorkingPatternDays = new List<WorkingPatternDay>
                        {
                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 2, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 3, AM = false, PM = true },
                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = false},
                            new WorkingPatternDay { DayOfWeek = 5, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
                        }
                    },
                    null,
                    null,
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 5), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 7), AM = false, PM = true, Duration = 0.5, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 8), AM = true, PM = false, Duration = 0.5, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 10), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true }
                    })
                    .SetName("AbsenceRangeToAbsenceDays: returns all valid working days against workingPattern");


                yield return new TestCaseData(
                    new AbsenceRange
                    {
                        PersonnelId = 1,
                        BeginDateUtc = new DateTime(2016, 12, 5),
                        EndDateUtc = new DateTime(2016, 12, 10),
                        CountryId = 1,
                        BeginAbsencePart = AbsencePart.FullDay,
                        EndAbsencePart = AbsencePart.FullDay
                    },
                    false,
                    null,
                    //PagedResult<CountryPublicHoliday>.Create(new List<CountryPublicHoliday>
                    //{
                    //    new CountryPublicHoliday { CountryId = 1, PublicHolidayId = 1, PublicHoliday = new PublicHoliday { Date = new DateTime(2016, 12, 6), PublicHolidayId = 1, Name = "Test1" } },
                    //    new CountryPublicHoliday { CountryId = 2, PublicHolidayId = 2, PublicHoliday = new PublicHoliday { Date = new DateTime(2016, 12, 9), PublicHolidayId = 2, Name = "Test2" } }
                    //}, 1, 2, 1, 2),
                    null,
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 5), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 7), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 8), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 10), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true }
                    })
                    .SetName("AbsenceRangeToAbsenceDays: returns all valid working days against CountryPublicHolidays");


                yield return new TestCaseData(
                    new AbsenceRange
                    {
                        PersonnelId = 1,
                        BeginDateUtc = new DateTime(2016, 12, 5),
                        EndDateUtc = new DateTime(2016, 12, 10),
                        CountryId = 1,
                        BeginAbsencePart = AbsencePart.FullDay,
                        EndAbsencePart = AbsencePart.FullDay
                    },
                    false,
                    null,
                    null,
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 6), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 9), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                    },
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 5), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 7), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 8), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 10), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                    })
                    .SetName("AbsenceRangeToAbsenceDays: returns all valid working days against already booked absences");


                yield return new TestCaseData(
                    new AbsenceRange
                    {
                        PersonnelId = 1,
                        BeginDateUtc = new DateTime(2016, 12, 5),
                        EndDateUtc = new DateTime(2016, 12, 11),
                        CountryId = 1,
                        BeginAbsencePart = AbsencePart.FullDay,
                        EndAbsencePart = AbsencePart.FullDay
                    },
                    false,
                    new WorkingPattern
                    {
                        WorkingPatternDays = new List<WorkingPatternDay>
                        {
                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
                            new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
                        }
                    },
                    //PagedResult<CountryPublicHoliday>.Create(new List<CountryPublicHoliday>
                    //{
                    //    new CountryPublicHoliday { CountryId = 2, PublicHolidayId = 2, PublicHoliday = new PublicHoliday { Date = new DateTime(2016, 12, 9), PublicHolidayId = 2, Name = "Test2" } }
                    //}, 1, 2, 1, 2),
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 5), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 6), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                    },
                    new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 7), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                        new AbsenceDay { AbsenceId = 0, Date = new DateTime(2016, 12, 8), AM = true, PM = true, Duration = 1, CanBeBookedAsAbsence = true },
                    })
                    .SetName("AbsenceRangeToAbsenceDays: returns all valid working days against working pattern, country public holidays and already booked absences");

            }
        }


        //[Test, TestCaseSource(nameof(AbsenceRangeToAbsenceDaysTestCases))]
        //public void AbsenceRangeToAbsenceDaysTests(AbsenceRange absenceRange, bool returnUnbookableDays, WorkingPattern workingPattern, PagedResult<CountryPublicHoliday> countryPublicHolidays, IEnumerable<AbsenceDay> alreadyBookedAbsenceDays, IEnumerable<AbsenceDay> expectedAbsenceDays)
        //{
        //    // Arrange
        //    _mockHRDataService.Setup(mock => mock.RetrievePersonnelWorkingPattern(It.IsAny<int>(), It.IsAny<int>())).Returns(workingPattern);
        //    _mockHRDataService.Setup(mock => mock.RetrieveCountryPublicHolidays(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<CountryPublicHoliday, bool>>>(), It.IsAny<List<OrderBy>>(), It.IsAny<Paging>())).Returns(countryPublicHolidays);
        //    _mockHRDataService.Setup(mock => mock.RetrieveAbsenceRangeBookedAbsenceDays(It.IsAny<AbsenceRange>())).Returns(alreadyBookedAbsenceDays);


        //    // Act
        //    var actualAbsenceDays = _hrBusinessService.AbsenceRangeToAbsenceDays(absenceRange, returnUnbookableDays);

        //    // Assert
        //    actualAbsenceDays.ShouldBeEquivalentTo(expectedAbsenceDays, opt => opt
        //        .Using<IEnumerable>(CheckList)
        //        .When(info => typeof(IEnumerable).IsAssignableFrom(info.CompileTimeType)));

        //}

        public static IEnumerable RemoveAbsenceFromPersonnelAbsenceEntitlementValuesTestCases
        {
            get
            {
                yield return new TestCaseData(null, null, null)
                    .SetName("RemoveAbsenceFromPersonnelAbsenceEntitlementValues: returns null If absence is null");

                yield return new TestCaseData(new Absence { }, null, null)
                   .SetName("RemoveAbsenceFromPersonnelAbsenceEntitlementValues: returns null If personnelAbsenceEntitlement is null");

                yield return new TestCaseData(
                    new Absence { PersonnelAbsenceEntitlementId = 1 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 2 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 2 })
                    .SetName("RemoveAbsenceFromPersonnelAbsenceEntitlementValues: returns PersonnelAbsenceEntitlement If absence.PersonnelAbsenceEntitlementId != personnelAbsenceEntitlement.PersonnelAbsenceEntitlementId");

                yield return new TestCaseData(
                    new Absence { PersonnelAbsenceEntitlementId = 1 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1 })
                    .SetName("RemoveAbsenceFromPersonnelAbsenceEntitlementValues: returns PersonnelAbsenceEntitlement If absence.AbsenceDays is null");

                yield return new TestCaseData(
                    new Absence { PersonnelAbsenceEntitlementId = 1, AbsenceDays = new List<AbsenceDay>() },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1 })
                    .SetName("RemoveAbsenceFromPersonnelAbsenceEntitlementValues: returns PersonnelAbsenceEntitlement If absence.AbsenceDays is empty");

                yield return new TestCaseData(
                    new Absence
                    {
                        PersonnelAbsenceEntitlementId = 1,
                        AbsenceDays = new List<AbsenceDay> { new AbsenceDay { Duration = 1 }, new AbsenceDay { Duration = 0.5 } },
                    },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1, Entitlement = 0, Used = 10, Remaining = 10 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1, Entitlement = 0, Used = 10, Remaining = 10 })
                    .SetName("RemoveAbsenceFromPersonnelAbsenceEntitlementValues: returns PersonnelAbsenceEntitlement unchanged when Entitlement = 0");

                yield return new TestCaseData(
                    new Absence
                    {
                        PersonnelAbsenceEntitlementId = 1,
                        AbsenceDays = new List<AbsenceDay> { new AbsenceDay { Duration = 1 }, new AbsenceDay { Duration = 0.5 } },
                    },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1, Entitlement = 20, Used = 10, Remaining = 10 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1, Entitlement = 20, Used = 8.5, Remaining = 11.5 })
                    .SetName("RemoveAbsenceFromPersonnelAbsenceEntitlementValues: returns PersonnelAbsenceEntitlement correctly recalculated");
            }
        }


        [Test, TestCaseSource(nameof(RemoveAbsenceFromPersonnelAbsenceEntitlementValuesTestCases))]
        public void RemoveAbsenceFromPersonnelAbsenceEntitlementValuesTests(Absence absence, PersonnelAbsenceEntitlement personnelAbsenceEntitlement, PersonnelAbsenceEntitlement expectedPersonnelAbsenceEntitlement)
        {
            // Arrange

            // Act
            var actualPersonnelAbsenceEntitlement = _hrBusinessService.RemoveAbsenceFromPersonnelAbsenceEntitlementValues(absence, personnelAbsenceEntitlement);

            // Assert
            actualPersonnelAbsenceEntitlement.ShouldBeEquivalentTo(expectedPersonnelAbsenceEntitlement);

        }

        public static IEnumerable AddAbsenceToPersonnelAbsenceEntitlementValuesTestCases
        {
            get
            {
                yield return new TestCaseData(null, null, null)
                    .SetName("AddAbsenceToPersonnelAbsenceEntitlementValues: returns null If absence is null");

                yield return new TestCaseData(new Absence { }, null, null)
                   .SetName("AddAbsenceToPersonnelAbsenceEntitlementValues: returns null If personnelAbsenceEntitlement is null");

                yield return new TestCaseData(
                    new Absence { PersonnelAbsenceEntitlementId = 1 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1 },
                    null)
                    .SetName("AddAbsenceToPersonnelAbsenceEntitlementValues: returns null If absence.AbsenceDays is null");

                yield return new TestCaseData(
                    new Absence { PersonnelAbsenceEntitlementId = 1, AbsenceDays = new List<AbsenceDay>() },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1 },
                    null)
                    .SetName("AddAbsenceToPersonnelAbsenceEntitlementValues: returns null If absence.AbsenceDays is empty");

                yield return new TestCaseData(
                    new Absence { PersonnelAbsenceEntitlementId = 1, AbsenceDays = new List<AbsenceDay> { new AbsenceDay { Duration = 1 }, new AbsenceDay { Duration = 0.5 } } },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 2 },
                    null)
                    .SetName("AddAbsenceToPersonnelAbsenceEntitlementValues: returns null If absence.AbsenceType is null");


                yield return new TestCaseData(
                    new Absence
                    {
                        PersonnelAbsenceEntitlementId = 1,
                        AbsenceDays = new List<AbsenceDay> { new AbsenceDay { Duration = 1 }, new AbsenceDay { Duration = 0.5 } },
                        AbsenceType = new AbsenceType { }
                    },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1, Entitlement = 0, Used = 10, Remaining = 10 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1, Entitlement = 0, Used = 10, Remaining = 10 })
                    .SetName("AddAbsenceToPersonnelAbsenceEntitlementValues: returns PersonnelAbsenceEntitlement unchanged when Entitlement = 0");

                yield return new TestCaseData(
                    new Absence
                    {
                        PersonnelAbsenceEntitlementId = 1,
                        AbsenceDays = new List<AbsenceDay> { new AbsenceDay { Duration = 1 }, new AbsenceDay { Duration = 0.5 } },
                        AbsenceType = new AbsenceType { }
                    },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1, Entitlement = 20, Used = 10, Remaining = 10 },
                    new PersonnelAbsenceEntitlement { PersonnelAbsenceEntitlementId = 1, Entitlement = 20, Used = 11.5, Remaining = 8.5 })
                    .SetName("AddAbsenceToPersonnelAbsenceEntitlementValues: returns PersonnelAbsenceEntitlement correctly recalculated");
            }
        }


        [Test, TestCaseSource(nameof(AddAbsenceToPersonnelAbsenceEntitlementValuesTestCases))]
        public void AddAbsenceToPersonnelAbsenceEntitlementValuesTests(Absence absence, PersonnelAbsenceEntitlement personnelAbsenceEntitlement, PersonnelAbsenceEntitlement expectedPersonnelAbsenceEntitlement)
        {
            // Arrange

            // Act
            var actualPersonnelAbsenceEntitlement = _hrBusinessService.AddAbsenceToPersonnelAbsenceEntitlementValues(absence, personnelAbsenceEntitlement);

            // Assert
            actualPersonnelAbsenceEntitlement.ShouldBeEquivalentTo(expectedPersonnelAbsenceEntitlement);

        }


        public static IEnumerable ValidateAbsenceTestCases
        {
            get
            {
                yield return new TestCaseData(
                    1,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new ValidationResult
                    {
                        Succeeded = false,
                        Errors = new List<string> { "Absence range is null" }
                    })
                    .SetName("ValidateAbsenceTests: returns failed ValidationResult If absenceRange is null");


                yield return new TestCaseData(
                    1,
                    new AbsenceRange
                    {
                        OrganisationId = 2,
                        BeginDateUtc = new DateTime(2016, 12, 5),
                        EndDateUtc = new DateTime(2016, 12, 11),
                        CountryId = 1,
                        BeginAbsencePart = AbsencePart.FullDay,
                        EndAbsencePart = AbsencePart.FullDay,
                        AbsenceTypeId = 1
                    },
                    new List<PersonnelAbsenceEntitlement> { new PersonnelAbsenceEntitlement { AbsenceTypeId = 1, Entitlement = 20, Used = 10, Remaining = 10 } },
                    null,
                    null,
                    null,
                    null,
                    new ValidationResult
                    {
                        Succeeded = false,
                        Errors = new List<string> { "Organisation does not equal personnel organisation" }
                    })
                    .SetName("ValidateAbsenceTests: returns failed ValidationResult If organisationids dont match");

                yield return new TestCaseData(
                    2,
                    new AbsenceRange
                    {
                        OrganisationId = 2,
                        BeginDateUtc = new DateTime(2016, 12, 5),
                        EndDateUtc = new DateTime(2016, 12, 6),
                        CountryId = 1,
                        BeginAbsencePart = AbsencePart.FullDay,
                        EndAbsencePart = AbsencePart.FullDay,
                        AbsenceTypeId = 1
                    },
                    new List<PersonnelAbsenceEntitlement> { new PersonnelAbsenceEntitlement { AbsenceTypeId = 1, Entitlement = 20, Used = 19, Remaining = 1 } },
                    null,
                    new WorkingPattern
                    {
                        WorkingPatternDays = new List<WorkingPatternDay>
                        {
                            new WorkingPatternDay { DayOfWeek = 1, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 2, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 3, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
                            new WorkingPatternDay { DayOfWeek = 5, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
                        }
                    },
                    null,
                    null,
                    new ValidationResult
                    {
                        Succeeded = false,
                        Errors = new List<string> { "Absence does not contain any valid days of absence" }
                    })
                    .SetName("ValidateAbsenceTests: returns failed ValidationResult If duration is less than or equal to zero");


                yield return new TestCaseData(
                   2,
                   new AbsenceRange
                   {
                       OrganisationId = 2,
                       BeginDateUtc = new DateTime(2016, 12, 5),
                       EndDateUtc = new DateTime(2016, 12, 6),
                       CountryId = 1,
                       BeginAbsencePart = AbsencePart.FullDay,
                       EndAbsencePart = AbsencePart.FullDay,
                       AbsenceTypeId = 1
                   },
                   new List<PersonnelAbsenceEntitlement> { new PersonnelAbsenceEntitlement { AbsenceTypeId = 1, Entitlement = 20, Used = 19, Remaining = 1 } },
                   null,
                   new WorkingPattern
                   {
                       WorkingPatternDays = new List<WorkingPatternDay>
                       {
                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 3, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
                            new WorkingPatternDay { DayOfWeek = 5, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
                       }
                   },
                   null,
                   null,
                   new ValidationResult
                   {
                       Succeeded = false,
                       Errors = new List<string> { "Request is greater than remaining entitlement" }
                   })
                   .SetName("ValidateAbsenceTests: returns failed ValidationResult If duration is greater than remaining entitlement");


                yield return new TestCaseData(
                   2,
                   new AbsenceRange
                   {
                       OrganisationId = 2,
                       BeginDateUtc = new DateTime(2016, 12, 5),
                       EndDateUtc = new DateTime(2016, 12, 6),
                       CountryId = 1,
                       BeginAbsencePart = AbsencePart.FullDay,
                       EndAbsencePart = AbsencePart.FullDay,
                       AbsenceTypeId = 1
                   },
                   new List<PersonnelAbsenceEntitlement> { new PersonnelAbsenceEntitlement { AbsenceTypeId = 1, Entitlement = 20, Used = 18, Remaining = 2 } },
                   null,
                   new WorkingPattern
                   {
                       WorkingPatternDays = new List<WorkingPatternDay>
                       {
                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 3, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
                            new WorkingPatternDay { DayOfWeek = 5, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
                       }
                   },
                   null,
                   null,
                   new ValidationResult
                   {
                       Succeeded = true
                   })
                   .SetName("ValidateAbsenceTests: returns successful ValidationResult");


                yield return new TestCaseData(
                   2,
                   new AbsenceRange
                   {
                       OrganisationId = 2,
                       BeginDateUtc = new DateTime(2016, 12, 5),
                       EndDateUtc = new DateTime(2016, 12, 6),
                       CountryId = 1,
                       BeginAbsencePart = AbsencePart.FullDay,
                       EndAbsencePart = AbsencePart.FullDay,
                       AbsenceTypeId = 1
                   },
                   new List<PersonnelAbsenceEntitlement> { },
                   new List<PersonnelAbsenceEntitlement> { new PersonnelAbsenceEntitlement { AbsenceTypeId = 1, Entitlement = 0 } },
                   new WorkingPattern
                   {
                       WorkingPatternDays = new List<WorkingPatternDay>
                       {
                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 3, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
                            new WorkingPatternDay { DayOfWeek = 5, AM = false, PM = false },
                            new WorkingPatternDay { DayOfWeek = 6, AM = true, PM = true },
                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
                       }
                   },
                   null,
                   null,
                   new ValidationResult
                   {
                       Succeeded = true
                   })
                   .SetName("ValidateAbsenceTests: returns successful ValidationResult when AbsenceType has Null PersonnelAbsenceEntitlement");
            }

        }

        //[Test, TestCaseSource(nameof(ValidateAbsenceTestCases))]
        //public void ValidateAbsenceTests(int organisationId, AbsenceRange absenceRange, IEnumerable<PersonnelAbsenceEntitlement> personnelAbsenceEntitlements, IEnumerable<PersonnelAbsenceEntitlement> nullAbsenceTypePersonnelAbsenceEntitlements, WorkingPattern workingPattern, PagedResult<CountryPublicHoliday> countryPublicHolidays, IEnumerable<AbsenceDay> alreadyBookedAbsenceDays, ValidationResult expectedValidationResult)
        //{
        //    // Arrange
        //    _mockHRDataService.Setup(mock => mock.RetrieveAbsenceType(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<AbsenceType, bool>>>())).Returns(new AbsenceType { AbsenceTypeId = 1, Name = "Test" });
        //    _mockHRDataService.SetupSequence(mock => mock.RetrievePersonnelAbsenceEntitlements(It.IsAny<int>(), It.IsAny<Expression<Func<PersonnelAbsenceEntitlement, bool>>>())).Returns(personnelAbsenceEntitlements).Returns(nullAbsenceTypePersonnelAbsenceEntitlements);
        //    _mockHRDataService.Setup(mock => mock.RetrievePersonnelWorkingPattern(It.IsAny<int>(), It.IsAny<int>())).Returns(workingPattern);
        //    //_mockHRDataService.Setup(mock => mock.RetrieveCountryPublicHolidays(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<CountryPublicHoliday, bool>>>(), It.IsAny<List<OrderBy>>(), It.IsAny<Paging>())).Returns(countryPublicHolidays);
        //    _mockHRDataService.Setup(mock => mock.RetrieveAbsenceRangeBookedAbsenceDays(It.IsAny<AbsenceRange>())).Returns(alreadyBookedAbsenceDays);


        //    // Act
        //    var actualPersonnelAbsenceEntitlement = _hrBusinessService.ValidateAbsence(organisationId, absenceRange);

        //    // Assert
        //    actualPersonnelAbsenceEntitlement.ShouldBeEquivalentTo(expectedValidationResult);

        //}

        //public static IEnumerable CreatePersonnelAbsenceEntitlementsTestCases
        //{
        //    get
        //    {
        //        yield return new TestCaseData(
        //            1,
        //            null,
        //            null,
        //            1,
        //            1,
        //            null,
        //            null,
        //            null,
        //            null,
        //            null)
        //            .SetName("CreatePersonnelAbsenceEntitlementsTests: returns null If employments is null");

        //        yield return new TestCaseData(
        //            1,
        //            new List<Employment>
        //            {
        //                new Employment
        //                {
        //                    OrganisationId = 1,
        //                    PersonnelId = 1,
        //                    Personnel = new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test" },
        //                    StartDate = new DateTime(2016, 12, 6),
        //                    DivisionId = 1,
        //                    Building = new Building { Site = new Site { CountryId = 1 } }
        //                }
        //            },
        //            new DivisionCountryWorkingPattern
        //            {
        //                OrganisationId = 1,
        //                DivisionId = 1,
        //                CountryId = 1,
        //                WorkingPattern = new WorkingPattern
        //                {
        //                    WorkingPatternDays = new List<WorkingPatternDay>
        //                        {
        //                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                            new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                        }
        //                }
        //            },
        //            1,
        //            1,
        //            null,
        //            PagedResult<DivisionCountryAbsencePeriod>.Create(new List<DivisionCountryAbsencePeriod>
        //            {
        //                new DivisionCountryAbsencePeriod
        //                {
        //                    OrganisationId = 1,
        //                    DivisionId = 1,
        //                    CountryId = 1,
        //                    DivisionCountryAbsencePeriodId = 1,
        //                    AbsencePeriodId = 1,
        //                    AbsencePeriod = new AbsencePeriod { StartDate = new DateTime(2016, 1, 1), EndDate = new DateTime(2016, 12, 31), }
        //                }
        //            }, 1, 1, 1, 1),
        //            new List<PersonnelAbsenceEntitlement> { },
        //            new List<PersonnelAbsenceEntitlement> { },
        //            new List<PersonnelAbsenceEntitlement>
        //            {
        //               new PersonnelAbsenceEntitlement
        //                {
        //                    OrganisationId = 1,
        //                    PersonnelId = 1,
        //                    DivisionCountryAbsencePeriodId = 1,
        //                    AbsenceTypeId = null,
        //                    StartDate = new DateTime(2016, 1, 1),
        //                    EndDate = new DateTime(2016, 12, 31),
        //                    Entitlement = null,
        //                    CarriedOver = 0,
        //                    Used = 0,
        //                    Remaining = null,
        //                    MaximumCarryForward = 0,
        //                    FrequencyId = 1
        //                },
        //            })
        //            .SetName("CreatePersonnelAbsenceEntitlementsTests: returns other entitlement If divisionCountryAbsenceTypeEntitlements is null");

        //        yield return new TestCaseData(
        //            1,
        //            new List<Employment>
        //            {
        //                new Employment
        //                {
        //                    OrganisationId = 1,
        //                    PersonnelId = 1,
        //                    Personnel = new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test" },
        //                    StartDate = new DateTime(2016, 12, 6),
        //                    DivisionId = 1,
        //                    Building = new Building { Site = new Site { CountryId = 1 } }
        //                }
        //            },
        //            new DivisionCountryWorkingPattern
        //            {
        //                OrganisationId = 1,
        //                DivisionId = 1,
        //                CountryId = 1,
        //                WorkingPattern = new WorkingPattern
        //                {
        //                    WorkingPatternDays = new List<WorkingPatternDay>
        //                        {
        //                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                            new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                        }
        //                }
        //            },
        //            1,
        //            1,
        //            new List<DivisionCountryAbsenceTypeEntitlement>
        //            {
        //                new DivisionCountryAbsenceTypeEntitlement
        //                {
        //                    OrganisationId = 1,
        //                    DivisionId = 1,
        //                    CountryAbsenceType = new CountryAbsenceType { CountryId = 1, AbsenceTypeId = 1 },
        //                    StartDate = new DateTime(2016, 1, 1),
        //                    EndDate = new DateTime(2016, 12, 31),
        //                    FrequencyId = 1,
        //                    Frequency = new Frequency { FrequencyId = 1, Name = "yearly" },
        //                    Entitlement = 20,
        //                    MaximumCarryForward = 5
        //                }
        //            },
        //            null,
        //            null,
        //            null,
        //            null)
        //            .SetName("CreatePersonnelAbsenceEntitlementsTests: returns null If divisionCountryAbsencePeriod is null");

        //        yield return new TestCaseData(
        //            1,
        //            new List<Employment>
        //            {
        //                new Employment
        //                {
        //                    OrganisationId = 1,
        //                    PersonnelId = 1,
        //                    Personnel = new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test" },
        //                    StartDate = new DateTime(2016, 12, 6),
        //                    DivisionId = 1,
        //                    Building = new Building { Site = new Site { CountryId = 1 } },
        //                    WorkingPattern = new WorkingPattern
        //                    {
        //                        WorkingPatternDays = new List<WorkingPatternDay>
        //                        {
        //                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                            new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                        }
        //                    }
        //                }
        //            },
        //            new DivisionCountryWorkingPattern
        //            {
        //                OrganisationId = 1,
        //                DivisionId = 1,
        //                CountryId = 1,
        //                WorkingPattern = new WorkingPattern
        //                {
        //                    WorkingPatternDays = new List<WorkingPatternDay>
        //                        {
        //                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                            new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                        }
        //                }
        //            },
        //            1,
        //            1,
        //            new List<DivisionCountryAbsenceTypeEntitlement>
        //            {
        //                new DivisionCountryAbsenceTypeEntitlement
        //                {
        //                    OrganisationId = 1,
        //                    DivisionId = 1,
        //                    CountryAbsenceType = new CountryAbsenceType { CountryId = 1, AbsenceTypeId = 1 },
        //                    StartDate = new DateTime(2016, 1, 1),
        //                    EndDate = new DateTime(2016, 12, 31),
        //                    FrequencyId = 1,
        //                    Frequency = new Frequency { FrequencyId = 1, Name = "yearly" },
        //                    Entitlement = 20,
        //                    MaximumCarryForward = 5
        //                }
        //            },
        //            PagedResult<DivisionCountryAbsencePeriod>.Create(new List<DivisionCountryAbsencePeriod>
        //            {
        //                new DivisionCountryAbsencePeriod
        //                {
        //                    DivisionId = 1,
        //                    CountryId = 1,
        //                    DivisionCountryAbsencePeriodId = 1,
        //                    AbsencePeriodId = 1,
        //                    AbsencePeriod = new AbsencePeriod { StartDate = new DateTime(2016, 1, 1), EndDate = new DateTime(2016, 12, 31), }
        //                }
        //            }, 1, 1, 1, 1),
        //            new List<PersonnelAbsenceEntitlement> { },
        //            new List<PersonnelAbsenceEntitlement> { },
        //            new List<PersonnelAbsenceEntitlement>
        //            {
        //                new PersonnelAbsenceEntitlement
        //                {
        //                    OrganisationId = 1,
        //                    PersonnelId = 1,
        //                    DivisionCountryAbsencePeriodId = 1,
        //                    AbsenceTypeId = 1,
        //                    StartDate = new DateTime(2016, 1, 1),
        //                    EndDate = new DateTime(2016, 12, 31),
        //                    Entitlement = 1.5,
        //                    CarriedOver = 0,
        //                    Used = 0,
        //                    Remaining = 1.5,
        //                    MaximumCarryForward = 5,
        //                    FrequencyId = 1

        //                },
        //                new PersonnelAbsenceEntitlement
        //                {
        //                    OrganisationId = 1,
        //                    PersonnelId = 1,
        //                    DivisionCountryAbsencePeriodId = 1,
        //                    AbsenceTypeId = null,
        //                    StartDate = new DateTime(2016, 1, 1),
        //                    EndDate = new DateTime(2016, 12, 31),
        //                    Entitlement = 0,
        //                    CarriedOver = 0,
        //                    Used = 0,
        //                    Remaining = 0,
        //                    MaximumCarryForward = 0,
        //                    FrequencyId = 1
        //                }
        //            })
        //            .SetName("CreatePersonnelAbsenceEntitlementsTests: Creates new PersonnelAbsenceEntitlements");

        //        yield return new TestCaseData(
        //            2,
        //            new List<Employment>
        //            {
        //                new Employment
        //                {
        //                    OrganisationId = 2,
        //                    PersonnelId = 2,
        //                    Personnel = new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test" },
        //                    StartDate = new DateTime(2016, 11, 6),
        //                    DivisionId = 2,
        //                    Building = new Building { Site = new Site { CountryId = 2 } },
        //                    WorkingPattern = new WorkingPattern
        //                    {
        //                        WorkingPatternDays = new List<WorkingPatternDay>
        //                        {
        //                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                            new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                        }
        //                    }
        //                }
        //            },
        //            new DivisionCountryWorkingPattern
        //            {
        //                OrganisationId = 2,
        //                DivisionId = 2,
        //                CountryId = 2,
        //                WorkingPattern = new WorkingPattern
        //                {
        //                    WorkingPatternDays = new List<WorkingPatternDay>
        //                        {
        //                            new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                            new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                            new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                            new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                        }
        //                }
        //            },
        //            2,
        //            2,
        //            new List<DivisionCountryAbsenceTypeEntitlement>
        //            {
        //                new DivisionCountryAbsenceTypeEntitlement
        //                {
        //                    OrganisationId = 2,
        //                    DivisionId = 2,
        //                    CountryAbsenceType = new CountryAbsenceType { CountryId = 2, AbsenceTypeId = 2 },
        //                    StartDate = new DateTime(2016, 1, 1),
        //                    EndDate = new DateTime(2016, 3, 31),
        //                    FrequencyId = 2,
        //                    Frequency = new Frequency { FrequencyId = 2, Name = "quarterly" },
        //                    Entitlement = 5,
        //                    MaximumCarryForward = 0
        //                }
        //            },
        //            PagedResult<DivisionCountryAbsencePeriod>.Create(new List<DivisionCountryAbsencePeriod>
        //            {
        //                new DivisionCountryAbsencePeriod
        //                {
        //                    DivisionId = 2,
        //                    CountryId = 2,
        //                    DivisionCountryAbsencePeriodId = 2,
        //                    AbsencePeriodId = 2,
        //                    AbsencePeriod = new AbsencePeriod { StartDate = new DateTime(2016, 1, 1), EndDate = new DateTime(2016, 12, 31), }
        //                }
        //            }, 1, 1, 1, 1),
        //            new List<PersonnelAbsenceEntitlement> { },
        //            new List<PersonnelAbsenceEntitlement> { },
        //            new List<PersonnelAbsenceEntitlement>
        //            {
        //                new PersonnelAbsenceEntitlement
        //                {
        //                    OrganisationId = 2,
        //                    PersonnelId = 2,
        //                    DivisionCountryAbsencePeriodId = 2,
        //                    AbsenceTypeId = null,
        //                    StartDate = new DateTime(2016, 1, 1),
        //                    EndDate = new DateTime(2016, 12, 31),
        //                    Entitlement = 0,
        //                    CarriedOver = 0,
        //                    Used = 0,
        //                    Remaining = 0,
        //                    MaximumCarryForward = 0,
        //                    FrequencyId = 1
        //                },
        //                new PersonnelAbsenceEntitlement
        //                {
        //                    OrganisationId = 2,
        //                    PersonnelId = 2,
        //                    DivisionCountryAbsencePeriodId = 2,
        //                    AbsenceTypeId = 2,
        //                    StartDate = new DateTime(2016, 10, 1),
        //                    EndDate = new DateTime(2016, 12, 31),
        //                    Entitlement = 3,
        //                    CarriedOver = 0,
        //                    Used = 0,
        //                    Remaining = 3,
        //                    MaximumCarryForward = 0,
        //                    FrequencyId = 2
        //                }


        //            })
        //            .SetName("CreatePersonnelAbsenceEntitlementsTests: Creates quarterly PersonnelAbsenceEntitlements");
        //    }
        //}

        //[Test, TestCaseSource(nameof(CreatePersonnelAbsenceEntitlementsTestCases))]
        //public void CreatePersonnelAbsenceEntitlementsTests(int organisationId, IEnumerable<Employment> employments, DivisionCountryWorkingPattern divisionCountryWorkingPattern, int divisionId, int countryId, IEnumerable<DivisionCountryAbsenceTypeEntitlement> divisionCountryAbsenceTypeEntitlements, PagedResult<DivisionCountryAbsencePeriod> divisionCountryAbsencePeriods, IEnumerable<PersonnelAbsenceEntitlement> personnelAbsenceEntitlements, IEnumerable<PersonnelAbsenceEntitlement> nullAbsenceTypePersonnelAbsenceEntitlements, IEnumerable<PersonnelAbsenceEntitlement> expectedPersonnelAbsenceEntitlements)
        //{
        //    // Arrange
        //    _mockHRDataService.Setup(mock => mock.RetrieveDivisionCountryAbsenceTypeEntitlements(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(divisionCountryAbsenceTypeEntitlements);
        //    _mockHRDataService.Setup(mock => mock.RetrieveDivisionCountryAbsencePeriods(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<OrderBy>>(), It.IsAny<Paging>())).Returns(divisionCountryAbsencePeriods);
        //    _mockHRDataService.Setup(mock => mock.RetrieveDivisionCountryWorkingPattern(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(divisionCountryWorkingPattern);
        //    _mockHRDataService.SetupSequence(mock => mock.RetrievePersonnelAbsenceEntitlements(It.IsAny<int>(), It.IsAny<Expression<Func<PersonnelAbsenceEntitlement, bool>>>())).Returns(personnelAbsenceEntitlements).Returns(nullAbsenceTypePersonnelAbsenceEntitlements);
        //    _mockHRDataService.Setup(mock => mock.CreatePersonnelAbsenceEntitlements(It.IsAny<int>(), It.IsAny<IEnumerable<PersonnelAbsenceEntitlement>>())).Returns((int org, IEnumerable<PersonnelAbsenceEntitlement> pae) => { return pae; });

        //    // Act
        //    var actualPersonnelAbsenceEntitlements = _hrBusinessService.CreatePersonnelAbsenceEntitlements(organisationId, employments, divisionId, countryId);

        //    // Assert
        //    actualPersonnelAbsenceEntitlements.ShouldAllBeEquivalentTo(expectedPersonnelAbsenceEntitlements);
        //}

        public static IEnumerable RetrieveDefaultWorkingPatternDaysTestCases
        {
            get
            {
                yield return new TestCaseData(new List<WorkingPatternDay>
                    {
                        new WorkingPatternDay { DayOfWeek = 1, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 2, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 3, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 4, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 5, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
                        new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false },
                    })
                    .SetName("RetrieveDefaultWorkingPatternDays: returns week");
            }
        }


        [Test, TestCaseSource(nameof(RetrieveDefaultWorkingPatternDaysTestCases))]
        public void RetrieveDefaultWorkingPatternDaysTests(IEnumerable<WorkingPatternDay> expectedWorkingPatternDays)
        {
            // Arrange

            // Act
            var actualWorkingPatternDays = _hrBusinessService.RetrieveDefaultWorkingPatternDays();

            // Assert
            actualWorkingPatternDays.ShouldAllBeEquivalentTo(expectedWorkingPatternDays);
        }

        public static IEnumerable RetrievePersonnelPermissionsTestCases
        {
            get
            {
                yield return new TestCaseData(
                    true,
                    1,
                    null,
                    null,
                    new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null  }
                    },
                    false,
                    null,
                    new Permissions
                    {
                        IsAdmin = true,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = true,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = true,
                        CanEditEmployments = true,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = true,
                        CanEditApprover = true
                    },
                    Times.Never(),
                    Times.Once(),
                    Times.Once())
                    .SetName("RetrievePersonnelPermissions: Is Admin Not Cached Employments");


                yield return new TestCaseData(
                    true,
                    1,
                    null,
                    null,
                    null,
                    true,
                    Node<Employment>.CreateTree(new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 }
                    }, e => e.PersonnelId, e => e.ReportsToPersonnelId),
                    new Permissions
                    {
                        IsAdmin = true,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = true,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = true,
                        CanEditEmployments = true,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = true,
                        CanEditApprover = true
                    },
                    Times.Once(),
                    Times.Never(),
                    Times.Never())
                    .SetName("RetrievePersonnelPermissions: Is Admin Cached Employments");



                yield return new TestCaseData(
                    true,
                    1,
                    null,
                    1,
                    new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null  }
                    },
                    false,
                    null,
                    new Permissions
                    {
                        IsAdmin = true,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = true,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = true,
                        CanEditEmployments = true,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = true,
                        CanEditApprover = true
                    },
                    Times.Never(),
                    Times.Exactly(2),
                    Times.Exactly(2))
                    .SetName("RetrievePersonnelPermissions: Is Admin viewing personnel Not Cached Employments");


                yield return new TestCaseData(
                    true,
                    1,
                    null,
                    1,
                    null,
                    true,
                    Node<Employment>.CreateTree(new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 }
                    }, e => e.PersonnelId, e => e.ReportsToPersonnelId),
                    new Permissions
                    {
                        IsAdmin = true,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = true,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = true,
                        CanEditEmployments = true,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = true,
                        CanEditApprover = true
                    },
                    Times.Exactly(2),
                    Times.Never(),
                    Times.Never())
                    .SetName("RetrievePersonnelPermissions: Is Admin viewing personnel Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    1,
                    2,
                    new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 }
                    },
                    false,
                    null,
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = true,
                        CanViewProfile = true,
                        CanEditProfile = false,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = false,
                        CanEditApprover = false
                    },
                    Times.Never(),
                    Times.Exactly(2),
                    Times.Exactly(2))
                    .SetName("RetrievePersonnelPermissions: Is Manager viewing personnel Not Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    1,
                    2,
                    null,
                    true,
                    Node<Employment>.CreateTree(new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 }
                    }, e => e.PersonnelId, e => e.ReportsToPersonnelId),
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = true,
                        CanViewProfile = true,
                        CanEditProfile = false,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = false,
                        CanEditApprover = false
                    },
                    Times.Exactly(2),
                    Times.Never(),
                    Times.Never())
                    .SetName("RetrievePersonnelPermissions: Is Manager viewing personnel Cached Employments");


                yield return new TestCaseData(
                    false,
                    1,
                    1,
                    2,
                    new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1, TerminationDate = DateTime.Today }
                    },
                    false,
                    null,
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = true,
                        CanViewProfile = true,
                        CanEditProfile = false,
                        CanCreateAbsence = false,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = false,
                        CanEditOvertime = false,
                        CanDeleteOvertime = false,
                        CanEditApprover = false
                    },
                    Times.Never(),
                    Times.Exactly(2),
                    Times.Exactly(2))
                    .SetName("RetrievePersonnelPermissions: Is Manager viewing terminated personnel Not Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    1,
                    2,
                    null,
                    true,
                    Node<Employment>.CreateTree(new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1, TerminationDate = DateTime.Today }
                    }, e => e.PersonnelId, e => e.ReportsToPersonnelId),
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = true,
                        CanViewProfile = true,
                        CanEditProfile = false,
                        CanCreateAbsence = false,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = false,
                        CanEditOvertime = false,
                        CanDeleteOvertime = false,
                        CanEditApprover = false
                    },
                    Times.Exactly(2),
                    Times.Never(),
                    Times.Never())
                    .SetName("RetrievePersonnelPermissions: Is Manager viewing terminated personnel Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    1,
                    3,
                    new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 },
                        new Employment { PersonnelId = 3, ReportsToPersonnelId = 2 }
                    },
                    false,
                    null,
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = true,
                        CanViewProfile = true,
                        CanEditProfile = false,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = false,
                        CanEditApprover = false
                    },
                    Times.Never(),
                    Times.Exactly(2),
                    Times.Exactly(2))
                    .SetName("RetrievePersonnelPermissions: Is Manager of nested personnel Not Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    1,
                    3,
                    null,
                    true,
                    Node<Employment>.CreateTree(new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 },
                        new Employment { PersonnelId = 3, ReportsToPersonnelId = 2 }
                    }, e => e.PersonnelId, e => e.ReportsToPersonnelId),
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = true,
                        CanViewProfile = true,
                        CanEditProfile = false,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = true,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = false,
                        CanEditApprover = false
                    },
                    Times.Exactly(2),
                    Times.Never(),
                    Times.Never())
                    .SetName("RetrievePersonnelPermissions: Is Manager of nested personnel Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    3,
                    3,
                    new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 },
                        new Employment { PersonnelId = 3, ReportsToPersonnelId = 2 }
                    },
                    false,
                    null,
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = true,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = false,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = true,
                        CanEditApprover = false
                    },
                    Times.Never(),
                    Times.Exactly(2),
                    Times.Exactly(2))
                    .SetName("RetrievePersonnelPermissions: Is personnel Not Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    3,
                    3,
                    null,
                    true,
                    Node<Employment>.CreateTree(new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 },
                        new Employment { PersonnelId = 3, ReportsToPersonnelId = 2 }
                    }, e => e.PersonnelId, e => e.ReportsToPersonnelId),
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = true,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = false,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = true,
                        CanEditApprover = false
                    },
                    Times.Exactly(2),
                    Times.Never(),
                    Times.Never())
                    .SetName("RetrievePersonnelPermissions: Is personnel Cached Employments");

                yield return new TestCaseData(
                   false,
                   1,
                   3,
                   3,
                   new List<Employment>
                   {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 },
                        new Employment { PersonnelId = 3, ReportsToPersonnelId = 2, TerminationDate = DateTime.Today }
                   },
                   false,
                   null,
                   new Permissions
                   {
                       IsAdmin = false,
                       IsManager = false,
                       CanViewProfile = true,
                       CanEditProfile = false,
                       CanCreateAbsence = false,
                       CanEditAbsence = false,
                       CanCancelAbsence = false,
                       //CanApproveAbsence = false,
                       CanEditEntitlements = false,
                       CanEditEmployments = false,
                       CanCreateOvertime = false,
                       CanEditOvertime = false,
                       CanDeleteOvertime = false,
                       CanEditApprover = false
                   },
                   Times.Never(),
                   Times.Exactly(2),
                   Times.Exactly(2))
                   .SetName("RetrievePersonnelPermissions: Is terminated personnel Not Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    3,
                    3,
                    null,
                    true,
                    Node<Employment>.CreateTree(new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 },
                        new Employment { PersonnelId = 3, ReportsToPersonnelId = 2, TerminationDate = DateTime.Today }
                    }, e => e.PersonnelId, e => e.ReportsToPersonnelId),
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = false,
                        CanCreateAbsence = false,
                        CanEditAbsence = false,
                        CanCancelAbsence = false,
                        //CanApproveAbsence = false,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = false,
                        CanEditOvertime = false,
                        CanDeleteOvertime = false,
                        CanEditApprover = false
                    },
                    Times.Exactly(2),
                    Times.Never(),
                    Times.Never())
                    .SetName("RetrievePersonnelPermissions: Is terminated personnel Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    2,
                    2,
                    new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 },
                        new Employment { PersonnelId = 3, ReportsToPersonnelId = 2 }
                    },
                    false,
                    null,
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = true,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = false,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = true,
                        CanEditApprover = false
                    },
                    Times.Never(),
                    Times.Exactly(2),
                    Times.Exactly(2))
                    .SetName("RetrievePersonnelPermissions: Is manager own profile Not Cached Employments");

                yield return new TestCaseData(
                    false,
                    1,
                    2,
                    2,
                    null,
                    true,
                    Node<Employment>.CreateTree(new List<Employment>
                    {
                        new Employment { PersonnelId = 1, ReportsToPersonnelId = null },
                        new Employment { PersonnelId = 2, ReportsToPersonnelId = 1 },
                        new Employment { PersonnelId = 3, ReportsToPersonnelId = 2 }
                    }, e => e.PersonnelId, e => e.ReportsToPersonnelId),
                    new Permissions
                    {
                        IsAdmin = false,
                        IsManager = false,
                        CanViewProfile = true,
                        CanEditProfile = true,
                        CanCreateAbsence = true,
                        CanEditAbsence = true,
                        CanCancelAbsence = true,
                        //CanApproveAbsence = false,
                        CanEditEntitlements = false,
                        CanEditEmployments = false,
                        CanCreateOvertime = true,
                        CanEditOvertime = true,
                        CanDeleteOvertime = true,
                        CanEditApprover = false
                    },
                    Times.Exactly(2),
                    Times.Never(),
                    Times.Never())
                    .SetName("RetrievePersonnelPermissions: Is manager own profile Cached Employments");
            }
        }


        [Test, TestCaseSource(nameof(RetrievePersonnelPermissionsTestCases))]
        public void RetrievePersonnelPermissionsTests(bool isAdmin, int organisationId, int userPersonnelId, int? personnelId, IEnumerable<Employment> mockEmployments, bool isCached, IEnumerable<Node<Employment>> mockEmploymentsTree, Permissions expectedPermissions, Times cacheGetTimes, Times cacheSetTimes, Times dataServiceTimes)
        {
            // Arrange
            _mockCacheProvider.Setup(mock => mock.IsSet(It.IsAny<string>())).Returns(isCached);
            _mockCacheProvider.Setup(mock => mock.Get(It.IsAny<string>())).Returns(mockEmploymentsTree);
            _mockHRDataService.Setup(mock => mock.RetrieveCurrentEmployments(It.IsAny<int>())).Returns(mockEmployments);

            // Act
            var actualPermissions = _hrBusinessService.RetrievePersonnelPermissions(isAdmin, organisationId, userPersonnelId, personnelId);

            // Assert
            actualPermissions.ShouldBeEquivalentTo(expectedPermissions);

            _mockCacheProvider.Verify(mock => mock.Get(It.IsAny<string>()), cacheGetTimes);
            _mockCacheProvider.Verify(mock => mock.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()), cacheSetTimes);
            _mockHRDataService.Verify(mock => mock.RetrieveCurrentEmployments(It.IsAny<int>()), dataServiceTimes);
        }


        public static IEnumerable ValidateEmploymentTestCases
        {
            get
            {
                yield return new TestCaseData(
                    null,
                    new ValidationResult<Employment>
                    {
                        Succeeded = false,
                        Errors = new List<string> { "Employment is null" }
                    })
                    .SetName("ValidateEmployment: If employment is null returns failed validation result");


                yield return new TestCaseData(
                    new Employment
                    {
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(-1)
                    },
                    new ValidationResult<Employment>
                    {
                        Succeeded = false,
                        Errors = new List<string> { "Employment end date should be greater than start date" },
                        Entity = new Employment
                        {
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(-1)
                        }

                    })
                    .SetName("ValidateEmployment: If start date is greater than end date returns failed validation result");

                yield return new TestCaseData(
                    new Employment
                    {
                        StartDate = DateTime.Today,
                        TerminationDate = DateTime.Today.AddDays(-1)
                    },
                    new ValidationResult<Employment>
                    {
                        Succeeded = false,
                        Errors = new List<string> { "Employment termination date should be greater than start date" },
                        Entity = new Employment
                        {
                            StartDate = DateTime.Today,
                            TerminationDate = DateTime.Today.AddDays(-1)
                        }

                    })
                    .SetName("ValidateEmployment: If start date is greater than termination date returns failed validation result");

                yield return new TestCaseData(
                    new Employment
                    {
                        TerminationDate = DateTime.Today.AddDays(-1),
                        EndDate = DateTime.Today
                    },
                    new ValidationResult<Employment>
                    {
                        Succeeded = false,
                        Errors = new List<string> { "Employment termination date should be greater than or equal to end date" },
                        Entity = new Employment
                        {
                            TerminationDate = DateTime.Today.AddDays(-1),
                            EndDate = DateTime.Today
                        }

                    })
                    .SetName("ValidateEmployment: If termination date is greater than end date returns failed validation result");

                yield return new TestCaseData(
                    new Employment
                    {
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(1)
                    },
                    new ValidationResult<Employment>
                    {
                        Succeeded = true,
                        Errors = null,
                        Entity = new Employment
                        {
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(1)
                        }

                    })
                    .SetName("ValidateEmployment: returns successful validation result");

                yield return new TestCaseData(
                    new Employment
                    {
                        StartDate = DateTime.Today,
                        EndDate = null
                    },
                    new ValidationResult<Employment>
                    {
                        Succeeded = true,
                        Errors = null,
                        Entity = new Employment
                        {
                            StartDate = DateTime.Today,
                            EndDate = null
                        }

                    })
                    .SetName("ValidateEmployment: returns successful validation result if end date is null");
            }
        }
        
        [Test, TestCaseSource(nameof(ValidateEmploymentTestCases))]
        public void ValidateEmploymentTests(Employment employment, ValidationResult expectedValidationResult)
        {
            // Arrange

            // Act
            var actualValidationResult = _hrBusinessService.ValidateEmployment(employment);

            // Assert
            actualValidationResult.ShouldBeEquivalentTo(expectedValidationResult);
        }
        
        public static IEnumerable CanApproveTestCases
        {
            get
            {
                yield return new TestCaseData(
                    1,
                    1,
                    true,
                    "UserId",
                    new List<Approver>(),
                    false)
                    .SetName("CanApprove: empty Approval list Admin");


                yield return new TestCaseData(
                    1,
                    1,
                    false,
                    "UserId",
                    new List<Approver>(),
                    false)
                    .SetName("CanApprove: empty Approval list not Admin");

                yield return new TestCaseData(
                    1,
                    1,
                    true,
                    "UserId",
                    new List<Approver>
                    {
                        new Approver {AspNetUserId = "UserId"}
                    },
                    true)
                    .SetName("CanApprove: single Approval list Admin UserId included");

                yield return new TestCaseData(
                    1,
                    1,
                    false,
                    "UserId",
                    new List<Approver>
                    {
                        new Approver {AspNetUserId = "UserId"},
                    },
                    true)
                    .SetName("CanApprove: single Approval list not Admin UserId included");

                yield return new TestCaseData(
                    1,
                    1,
                    true,
                    "UserId",
                    new List<Approver>
                    {
                        new Approver {AspNetUserId = "NotUserId"}
                    },
                    true)
                    .SetName("CanApprove: single Approval list Admin UserId not included");

                yield return new TestCaseData(
                    1,
                    1,
                    false,
                    "UserId",
                    new List<Approver>
                    {
                        new Approver {AspNetUserId = "NotUserId"}
                    },
                    false)
                    .SetName("CanApprove: single Approval list not Admin UserId not included");

                yield return new TestCaseData(
                    1,
                    1,
                    true,
                    "UserId",
                    new List<Approver>
                    {
                        new Approver {AspNetUserId = "UserId"},
                        new Approver {AspNetUserId = "NotUserId"}
                    },
                    true)
                    .SetName("CanApprove: multiple Approval list Admin UserId included");

                yield return new TestCaseData(
                    1,
                    1,
                    false,
                    "UserId",
                    new List<Approver>
                    {
                        new Approver {AspNetUserId = "UserId"},
                        new Approver {AspNetUserId = "NotUserId"}
                    },
                    true)
                    .SetName("CanApprove: multiple Approval list not Admin UserId included");

                yield return new TestCaseData(
                    1,
                    1,
                    true,
                    "UserId",
                    new List<Approver>
                    {
                        new Approver {AspNetUserId = "StillNotUserId"},
                        new Approver {AspNetUserId = "NotUserId"}
                    },
                    true)
                    .SetName("CanApprove: multiple Approval list Admin UserId not included");

                yield return new TestCaseData(
                    1,
                    1,
                    false,
                    "UserId",
                    new List<Approver>
                    {
                        new Approver {AspNetUserId = "StillNotUserId"},
                        new Approver {AspNetUserId = "NotUserId"}
                    },
                    false)
                    .SetName("CanApprove: multiple Approval list not Admin UserId not included");
            }
        }

        [Test, TestCaseSource(nameof(CanApproveTestCases))]
        public void CanApproveTests(int organisationId, int entityId, bool isAdmin, string userId, IEnumerable<Approver>  mockApprovers, bool expectedResult)
        {
            // Arrange
            _mockHRDataService.Setup(mock => mock.RetrieveNextApprovers(It.IsAny<int>(), It.IsAny<ApprovalTypes>(), It.IsAny<int>())).Returns(mockApprovers);

            // Act
            var actualResult1 = _hrBusinessService.CanApproveAbsence(organisationId, entityId, isAdmin, userId);
            var actualResult2 = _hrBusinessService.CanApproveOvertime(organisationId, entityId, isAdmin, userId);

            // Assert
            actualResult1.ShouldBeEquivalentTo(expectedResult);
            actualResult2.ShouldBeEquivalentTo(expectedResult);
        }

        //public static IEnumerable CalculateProRataEntitlementTestCases
        //{
        //    get
        //    {
        //        yield return new TestCaseData(
        //            null,
        //            null,
        //            null,
        //            null,
        //            null,
        //            null,
        //            null
        //            )
        //            .SetName("CalculateProRataEntitlement: If divisionCountryAbsenceTypeEntitlement is null returns null");

        //        yield return new TestCaseData(
        //           new DivisionCountryAbsenceTypeEntitlement
        //           {
        //               Entitlement = 20
        //           },
        //           null,
        //           null,
        //           null,
        //           null,
        //           null,
        //           null
        //           )
        //           .SetName("CalculateProRataEntitlement: If divisionCountryWorkingPattern is null returns null");

        //        yield return new TestCaseData(
        //            new DivisionCountryAbsenceTypeEntitlement
        //            {
        //                Entitlement = 20
        //            },
        //            new WorkingPattern
        //            {
        //                WorkingPatternDays = new List<WorkingPatternDay>
        //                {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                }
        //            },
        //            null,
        //            null,
        //            null,
        //            null,
        //            null
        //            )
        //            .SetName("CalculateProRataEntitlement: If employmentWorkingPattern is null returns null");

        //        yield return new TestCaseData(
        //            new DivisionCountryAbsenceTypeEntitlement
        //            {
        //                Entitlement = 0
        //            },
        //            new WorkingPattern
        //            {
        //                WorkingPatternDays = new List<WorkingPatternDay>
        //                {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                }
        //            },
        //            new WorkingPattern
        //            {
        //                WorkingPatternDays = new List<WorkingPatternDay>
        //                {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //                }
        //            },
        //            null,
        //            null,
        //            null,
        //            0D
        //            )
        //            .SetName("CalculateProRataEntitlement: If divisionCountryAbsenceTypeEntitlement.Entitlement == 0 returns 0");


        //        yield return new TestCaseData(
        //           new DivisionCountryAbsenceTypeEntitlement
        //           {
        //               Entitlement = 20
        //           },
        //           new WorkingPattern
        //           {
        //               WorkingPatternDays = new List<WorkingPatternDay>
        //               {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //               }
        //           },
        //           new WorkingPattern
        //           {
        //               WorkingPatternDays = new List<WorkingPatternDay>
        //               {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //               }
        //           },
        //           new DateTime(2016, 1, 1),
        //           new DateTime(2016, 12, 31),
        //           new DateTime(2016, 1, 1),
        //           20D
        //           )
        //           .SetName("CalculateProRataEntitlement: If employmentStartDate == periodStartDate returns divisionCountryAbsenceTypeEntitlement.Entitlement");


        //        yield return new TestCaseData(
        //           new DivisionCountryAbsenceTypeEntitlement
        //           {
        //               Entitlement = 20
        //           },
        //           new WorkingPattern
        //           {
        //               WorkingPatternDays = new List<WorkingPatternDay>
        //               {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //               }
        //           },
        //           new WorkingPattern
        //           {
        //               WorkingPatternDays = new List<WorkingPatternDay>
        //               {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //               }
        //           },
        //           new DateTime(2016, 1, 1),
        //           new DateTime(2016, 12, 31),
        //           new DateTime(2016, 6, 8),
        //           11.5D
        //           )
        //           .SetName("CalculateProRataEntitlement: returns Entitlement Pro rata for full time employee");


        //        yield return new TestCaseData(
        //          new DivisionCountryAbsenceTypeEntitlement
        //          {
        //              Entitlement = 20
        //          },
        //          new WorkingPattern
        //          {
        //              WorkingPatternDays = new List<WorkingPatternDay>
        //              {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = true, PM = true},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //              }
        //          },
        //          new WorkingPattern
        //          {
        //              WorkingPatternDays = new List<WorkingPatternDay>
        //              {
        //                    new WorkingPatternDay { DayOfWeek = 1, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 2, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 3, AM = true, PM = true },
        //                    new WorkingPatternDay { DayOfWeek = 4, AM = false, PM = false},
        //                    new WorkingPatternDay { DayOfWeek = 5, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 6, AM = false, PM = false },
        //                    new WorkingPatternDay { DayOfWeek = 0, AM = false, PM = false }
        //              }
        //          },
        //          new DateTime(2016, 1, 1),
        //          new DateTime(2016, 12, 31),
        //          new DateTime(2016, 6, 8),
        //          7D
        //          )
        //          .SetName("CalculateProRataEntitlement: returns Entitlement Pro rata for part time employee");
        //    }
        //}


        //[Test, TestCaseSource(nameof(CalculateProRataEntitlementTestCases))]
        //public void CalculateProRataEntitlementTests(DivisionCountryAbsenceTypeEntitlement divisionCountryAbsenceTypeEntitlement, WorkingPattern divisionCountryWorkingPattern, WorkingPattern employmentWorkingPattern, DateTime periodStartDate, DateTime periodEndDate, DateTime employmentStartDate, double? expectedValue)
        //{
        //    // Arrange

        //    // Act
        //    var actualValidationResult = _hrBusinessService.CalculateProRataEntitlement(divisionCountryAbsenceTypeEntitlement, divisionCountryWorkingPattern, employmentWorkingPattern, periodStartDate, periodEndDate, employmentStartDate);

        //    // Assert
        //    actualValidationResult.ShouldBeEquivalentTo(expectedValue);
        //}
    }
}
