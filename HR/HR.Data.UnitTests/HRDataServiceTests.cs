using FluentAssertions;
using HR.Data.Interfaces;
using HR.Data.Models;
using HR.Data.UnitTests.Extensions;
using HR.Entity;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HR.Data.UnitTests
{
    [TestFixture]
    public class HRDataServiceTests
    {
        private static Mock<IHRDatabaseFactory> _mockHRDatabaseFactory;



        private HRDataService _hRDataService;

        [SetUp]
        public void Setup()
        {
            _mockHRDatabaseFactory = new Mock<IHRDatabaseFactory>();
            _hRDataService = new HRDataService(_mockHRDatabaseFactory.Object);
        }


        //public static IEnumerable RetrieveNextApproversTestCases
        //{
        //    //get
        //    //{
        //    //    yield return new TestCaseData(
        //    //        //1,
        //    //        //1,
        //    //        //true,
        //    //        //"UserId",
        //    //        //new List<Approver>(),
        //    //        false)
        //    //        .SetName("CanApproveOvertime: empty Approval list Admin");
        //    //}
        //}

        //[Test, TestCaseSource(nameof(RetrieveNextApproversTestCases))]
        //public void RetrieveNextApproversTests(int organisationId, int entityId, bool isAdmin, string userId, IEnumerable<Approver> mockApprovers, bool expectedResult)
        //{
        //    //_mockHRDataService.Setup(mock => mock.RetrieveNextApprovers(It.IsAny<int>(), It.IsAny<ApprovalTypes>(), It.IsAny<int>())).Returns(mockApprovers);
        //    // Arrange
        //    ////_context.Setup(mock => mock.Approvals.)
        //    //_mockHRDatabaseFactory.Setup(mock => mock.Create(It.IsAny<int>())).Returns(_mockHRDatabaseFactory.Object.Create());
        //    //// Act
        //    //var actualResult = _hrBusinessService.CanApproveOvertime(organisationId, entityId, isAdmin, userId);

        //    //// Assert
        //    //actualResult.ShouldBeEquivalentTo(expectedResult);
        //}
    }
}
