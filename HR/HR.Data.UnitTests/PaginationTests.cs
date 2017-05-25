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
    public class PaginationTests
    {

        public static IEnumerable PaginationTestCases
        {
            get
            {
                yield return new TestCaseData(new List<Personnel>().AsQueryable(), null, PagedResult<Personnel>.Empty).SetName("Paginate returns empty PagedResult");

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
                    PagedResult<Personnel>.Create(
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
                        }, 1, 11, 1, 11)

                    )
                    .SetName("Paginate returns all when paging is null");

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
                    new Paging { Page = 1, PageSize = 5 },
                    PagedResult<Personnel>.Create(
                        new List<Personnel>
                        {
                            new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 1 },
                            new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 2 },
                            new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 3 },
                            new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 4 },
                            new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 5 }
                        }, 1, 5, 3, 11))
                    .SetName("Paginate returns first page of results");


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
                   new Paging { Page = -1, PageSize = 5 },
                   PagedResult<Personnel>.Create(
                       new List<Personnel>
                       {
                            new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 1 },
                            new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 2 },
                            new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 3 },
                            new Personnel { Title = "Mrs", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 4 },
                            new Personnel { Title = "Mr", Forenames = "Test", Surname = "Test", DOB = DateTime.Today, PersonnelId = 5 }
                       }, 1, 5, 3, 11))
                   .SetName("Paginate returns first page of results when page < 0");


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
                   new Paging { Page = 1, PageSize = -1 },
                   PagedResult<Personnel>.Create(
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
                       }, 1, 10, 2, 11))
                   .SetName("Paginate returns pagesize of 10 when pagesize < 0");

            }

        }
        [Test, TestCaseSource(nameof(PaginationTestCases))]
        public void Paginate(IQueryable<Personnel> mockedPersonnel, Paging paging, PagedResult<Personnel> expectedPersonnel)
        {
            //Arrange

            //Act
            var actual = mockedPersonnel.Paginate(paging);

            //Assert
            actual.ShouldBeEquivalentTo(expectedPersonnel);

        }
    }
}
