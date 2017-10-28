using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
//using DocumentService.API.RESTClient.Interfaces;
using FluentAssertions;
using HR.Business.Interfaces;
using HR.Data.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using Moq;
using NUnit.Framework;

namespace HR.Business.UnitTests
{
    public class AbsenceSlotTests
    {
        private Mock<IHRDataService> _mockHRDataService;
        private Mock<ICacheProvider> _mockCacheProvider;
        private Mock<ITemplateService> _mockTemplateService;
        private Mock<IEmailService> _mockEmailService;
        //private Mock<IDocumentServiceRestClient> _mockDocumentServiceRestClient;
        private IHRBusinessService _hrBusinessService;
        private static readonly DateTime _monday = new DateTime(2017, 02, 27);
        [SetUp]
        public void Setup()
        {
            _mockHRDataService = new Mock<IHRDataService>();
            _mockCacheProvider = new Mock<ICacheProvider>();
            _mockTemplateService = new Mock<ITemplateService>();
            _mockEmailService = new Mock<IEmailService>();
            //_mockDocumentServiceRestClient = new Mock<IDocumentServiceRestClient>();

            //_hrBusinessService = new HRBusinessService(_mockHRDataService.Object, _mockCacheProvider.Object,
            //_mockTemplateService.Object, _mockEmailService.Object, _mockDocumentServiceRestClient.Object);


        }

        public static IEnumerable ToAbsenceDayListTestCases
        {
            get
            {
                //From monday to wednesday, Monday and Wednesday fullday is booked,tuesday is not working day
                yield return new TestCaseData(

                    new Absence
                    {
                        AbsenceDays = new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = _monday, AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(2), AM = true, PM = true, Duration = 1 },
                    }
                    },
                    _monday,
                    _monday.AddDays(6),
                    new List<AbsenceSlot>
                    { new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday, 
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                        new AbsenceDay { AbsenceId = 0, Date = _monday, AM = true, PM = true, Duration = 1 },
                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false,
                    },
                    new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday.AddDays(2),
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(2), AM = true, PM = true, Duration = 1 },
                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false
                    }
                    }
                    ).SetName("AbsenceSlot:From monday to wednesday, Monday and Wednesday fullday is booked,tuesday is not working day");

                //From monday to sunday, Monday fullday  and tuesday HalfDay AM , Wednesday and Thursday fullday ,Friday HalfDay PM , Saturday and Sunday FullDay is booked
                yield return new TestCaseData(

                    new Absence
                    {
                        AbsenceDays = new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = _monday, AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(1), AM = true, PM = false, Duration = 0.5 },
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(2), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(3), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(4), AM = false, PM = true, Duration = 0.5 },
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(5), AM = true, PM = true, Duration = 1 },
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(6), AM = true, PM = true, Duration = 1 },
                    }
                    },
                    _monday,
                    _monday.AddDays(6),
                    new List<AbsenceSlot>
                    { new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday,
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                         new AbsenceDay { AbsenceId = 0, Date = _monday, AM = true, PM = true, Duration = 1 },
                         new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(1), AM = true, PM = false, Duration =0.5 },
                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false
                    },

                    new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday.AddDays(2),
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(2), AM = true, PM = true, Duration =1 },
                         new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(3), AM = true, PM = true, Duration =1 },
                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false
                    }
                    ,

                    new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday.AddDays(4),
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                          new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(4), AM = false, PM = true, Duration =0.5 },
                          new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(5), AM = true, PM = true, Duration =1 },
                          new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(6), AM = true, PM = true, Duration =1 },
                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = true
                    }
                                          }
                    ).SetName("AbsenceSlot:From monday to sunday, Monday fullday  and tuesday HalfDay AM , Wednesday and Thursday fullday ,Friday HalfDay PM , Saturday and Sunday FullDay is booked");
                
                //From monday to tuesday, Monday HalfDay PM and tuesday HalfDay AM
                yield return new TestCaseData(

                    new Absence
                    {
                        AbsenceDays = new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = _monday, AM = false, PM = true, Duration = 0.5},
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(1), AM = true, PM = false, Duration = 0.5 },

                    }
                    },
                    _monday,
                    _monday.AddDays(6),
                    new List<AbsenceSlot>
                    { new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday,
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                          new AbsenceDay { AbsenceId = 0, Date = _monday, AM = false, PM = true, Duration = 0.5},
                            new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(1), AM = true, PM = false, Duration = 0.5 },

                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false  
                    }
                    }
                    ).SetName("AbsenceSlot:From monday to tuesday, Monday HalfDay AM and tuesday HalfDay AM");
                //From monday to tuesday, Monday HalfDay PM and tuesday HalfDay AM
                yield return new TestCaseData(

                    new Absence
                    {
                        AbsenceDays = new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = _monday, AM = true, PM = false, Duration = 0.5},
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(1), AM = true, PM = false, Duration = 0.5 },

                    }
                    },
                    _monday,
                    _monday.AddDays(6),
                    new List<AbsenceSlot>
                    { new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday,
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                          new AbsenceDay { AbsenceId = 0, Date = _monday, AM = true, PM = false, Duration = 0.5},
                          

                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false
                    },
                    new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday.AddDays(1),
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                            new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(1), AM = true, PM = false, Duration = 0.5 },

                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false
                    }
                    }
                    ).SetName("AbsenceSlot:From monday to tuesday, Monday HalfDay PM and tuesday HalfDay AM");

                //From Friday to Monday AM only (working schedule is AM only) is booked, saturday and sunday is not working day
                yield return new TestCaseData(

                    new Absence
                    {
                        AbsenceDays = new List<AbsenceDay>
                    {
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(4), AM = true, PM = false, Duration = 0.5 },
                        new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(7), AM = true, PM = false, Duration = 0.5 },
                    }
                    },
                    _monday,
                    _monday.AddDays(6),
                    new List<AbsenceSlot>
                    { new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday.AddDays(4),
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                            new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(4), AM = true, PM = false, Duration = 0.5 },
                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false
                    },
                    new AbsenceSlot
                    {
                        AbsenceId = 0,
                        SlotBeginDate = _monday.AddDays(7),
                        BeginDate = _monday,
                        EndDate = _monday.AddDays(6),
                        AbsenceDays = new List<AbsenceDay>
                        {
                            new AbsenceDay { AbsenceId = 0, Date = _monday.AddDays(7), AM = true, PM = false, Duration = 0.5 },
                        },
                        Colour = "6699CC",
                        CanApprove = null,
                        BeginsPM = false
                    }
                    }
                    ).SetName("AbsenceSlot:From Friday to Monday AM only (working schedule is AM only) is booked, saturday and sunday is not working day");
            }
        }

        [Test, TestCaseSource(nameof(ToAbsenceDayListTestCases))]
        public void RetrieveAbsenceSlotTests(Absence absence, DateTime beginDate, DateTime endDate, IEnumerable<AbsenceSlot> expectedAbsenceSlots)
        {
            // Arrange
            // Act
            var actualAbsenceSlots = _hrBusinessService.RetrieveAbsenceSlots(absence, beginDate, endDate);
            // Assert
            actualAbsenceSlots.ShouldBeEquivalentTo(expectedAbsenceSlots);
        }
    }
}
