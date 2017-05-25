using FluentAssertions;
using HR.Data.Models;
using NUnit.Framework;
using System;
using System.Collections;

namespace HR.Data.UnitTests
{
    [TestFixture]
    public class HRDatabaseFactoryTests
    {
        public static IEnumerable CreateThrowsNullReferenceExceptionWhenTestCases
        {
            get
            {
                yield return new TestCaseData(null).SetName("HRDatabaseFactory.Create Throws Null Reference Exception When nameOrConnectionString is null");
                yield return new TestCaseData(string.Empty).SetName("HRDatabaseFactory.Create Throws Null Reference Exception When nameOrConnectionString is empty");
                yield return new TestCaseData("   ").SetName("HRDatabaseFactory.Create Throws Null Reference Exception When nameOrConnectionString is white space");
            }
        }

        [Test, TestCaseSource(nameof(CreateThrowsNullReferenceExceptionWhenTestCases))]
        public void HRDatabaseFactoryCreate_ThrowsNullReferenceExceptionWhen(string nameOrConnectionString)
        {
            //Arrange
            var factory = new HRDatabaseFactory(nameOrConnectionString);
            
            //Act
            Action actual = () => factory.Create();

            //Assert
            actual.ShouldThrow<NullReferenceException>();
        }


        [Test]
        public void HRDatabaseFactoryCreate_ReturnsHRDatabase()
        {
            //Arrange
            var factory = new HRDatabaseFactory("name=HRDatabase");

            //Act
            var actual = factory.Create();

            //Assert
            actual.Should().BeOfType<HRDatabase>();
        }
    }
}
