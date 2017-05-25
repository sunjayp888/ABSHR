using FluentAssertions;
using HR.Data.Extensions;
using HR.Entity;
using HR.Entity.Dto;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HR.Data.UnitTests
{
    [TestFixture]
    public class OrderingTests
    {

        public static IEnumerable OrderingTestCases
        {
            get
            {
                yield return new TestCaseData(new List<Personnel>().AsQueryable(), null, new List<Personnel>().AsQueryable()).SetName("OrderBy returns source when source is empty");

                yield return new TestCaseData(
                    new List<Personnel>
                    {
                        new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 1 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 2 },
                        new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 3 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 4 },
                        new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 5 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 6 },
                        new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 7 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 8 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 9 },
                        new Personnel { Title = "Dr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 10 },
                        new Personnel { Title = "Miss", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 11 }
                    }.AsQueryable(),
                    null,
                    new List<Personnel>
                    {
                        new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 1 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 2 },
                        new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 3 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 4 },
                        new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 5 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 6 },
                        new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 7 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 8 },
                        new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 9 },
                        new Personnel { Title = "Dr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 10 },
                        new Personnel { Title = "Miss", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 11 }
                    }.AsQueryable()
                    )
                    .SetName("OrderBy returns source when List<OrderBy> is null");

                yield return new TestCaseData(
                    new List<Personnel>
                    {
                        new Personnel { Title = "Mr", Forenames = "A", Surname = "Test", DOB = DateTime.Today, PersonnelId = 1 },
                        new Personnel { Title = "Mrs", Forenames = "c", Surname = "Test", DOB = DateTime.Today, PersonnelId = 2 },
                        new Personnel { Title = "Mr", Forenames = "e", Surname = "Test", DOB = DateTime.Today, PersonnelId = 3 },
                        new Personnel { Title = "Mrs", Forenames = "d", Surname = "Test", DOB = DateTime.Today, PersonnelId = 4 },
                        new Personnel { Title = "Mr", Forenames = "b", Surname = "Test", DOB = DateTime.Today, PersonnelId = 5 },
                        new Personnel { Title = "Mrs", Forenames = "f", Surname = "Test", DOB = DateTime.Today, PersonnelId = 6 }
                    }.AsQueryable(),
                    new List<OrderBy> { new OrderBy { Property = "Forenames", Direction = System.ComponentModel.ListSortDirection.Ascending } },
                    new List<Personnel>
                    {
                        new Personnel { Title = "Mr", Forenames = "A", Surname = "Test", DOB = DateTime.Today, PersonnelId = 1 },
                        new Personnel { Title = "Mr", Forenames = "b", Surname = "Test", DOB = DateTime.Today, PersonnelId = 5 },
                        new Personnel { Title = "Mrs", Forenames = "c", Surname = "Test", DOB = DateTime.Today, PersonnelId = 2 },
                        new Personnel { Title = "Mrs", Forenames = "d", Surname = "Test", DOB = DateTime.Today, PersonnelId = 4 },
                        new Personnel { Title = "Mr", Forenames = "e", Surname = "Test", DOB = DateTime.Today, PersonnelId = 3 },
                        new Personnel { Title = "Mrs", Forenames = "f", Surname = "Test", DOB = DateTime.Today, PersonnelId = 6 }
                    }.AsQueryable())
                    .SetName("OrderBy orders the results Ascending");


                yield return new TestCaseData(
                   new List<Personnel>
                   {
                       new Personnel { Title = "Mr", Forenames = "A", Surname = "B", DOB = DateTime.Today, PersonnelId = 1 },
                       new Personnel { Title = "Mrs", Forenames = "c", Surname = "Test", DOB = DateTime.Today, PersonnelId = 2 },
                       new Personnel { Title = "Mr", Forenames = "b", Surname = "C", DOB = DateTime.Today, PersonnelId = 3 },
                       new Personnel { Title = "Mrs", Forenames = "d", Surname = "Test", DOB = DateTime.Today, PersonnelId = 4 },
                       new Personnel { Title = "Mr", Forenames = "b", Surname = "B", DOB = DateTime.Today, PersonnelId = 5 },
                       new Personnel { Title = "Mrs", Forenames = "A", Surname = "A", DOB = DateTime.Today, PersonnelId = 6 }
                   }.AsQueryable(),
                   new List<OrderBy> {
                       new OrderBy { Property = "Forenames", Direction = System.ComponentModel.ListSortDirection.Descending },
                       new OrderBy { Property = "Surname", Direction = System.ComponentModel.ListSortDirection.Ascending }
                   },
                   new List<Personnel>
                   {
                       new Personnel { Title = "Mrs", Forenames = "d", Surname = "Test", DOB = DateTime.Today, PersonnelId = 4 },
                       new Personnel { Title = "Mrs", Forenames = "c", Surname = "Test", DOB = DateTime.Today, PersonnelId = 2 },
                       new Personnel { Title = "Mr", Forenames = "b", Surname = "B", DOB = DateTime.Today, PersonnelId = 5 },
                       new Personnel { Title = "Mr", Forenames = "b", Surname = "C", DOB = DateTime.Today, PersonnelId = 3 },
                       new Personnel { Title = "Mrs", Forenames = "A", Surname = "A", DOB = DateTime.Today, PersonnelId = 6 },
                       new Personnel { Title = "Mr", Forenames = "A", Surname = "B", DOB = DateTime.Today, PersonnelId = 1 }

                   }.AsQueryable())
                   .SetName("OrderBy orders the results by multiple properties");


            }

        }
        [Test, TestCaseSource(nameof(OrderingTestCases))]
        public void OrderBy(IQueryable<Personnel> mockedPersonnel, List<OrderBy> ordering, IQueryable<Personnel> expectedPersonnel)
        {
            //Arrange

            //Act
            var actual = mockedPersonnel.OrderBy(ordering);

            //Assert
            actual.ShouldBeEquivalentTo(expectedPersonnel);

        }
    }
}
