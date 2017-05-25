using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace HR.Data.UnitTests.Extensions
{
    public static class EntityExtensions
    {
        public static Mock<DbSet<T>> ToMockDbSet<T>(this List<T> table) where T : class
        {
            var dbSet = new Mock<DbSet<T>>();
            var data = table.AsQueryable();

            dbSet.As<IQueryable<T>>().Setup(q => q.Provider).Returns(() => data.AsQueryable().Provider);
            dbSet.As<IQueryable<T>>().Setup(q => q.Expression).Returns(() => data.AsQueryable().Expression);
            dbSet.As<IQueryable<T>>().Setup(q => q.ElementType).Returns(() => data.AsQueryable().ElementType);
            dbSet.As<IQueryable<T>>().Setup(q => q.GetEnumerator()).Returns(() => data.AsQueryable().GetEnumerator());

            dbSet.Setup(set => set.Add(It.IsAny<T>())).Callback<T>(table.Add);
            dbSet.Setup(set => set.AddRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>(table.AddRange);
            dbSet.Setup(set => set.Remove(It.IsAny<T>())).Callback<T>(t => table.Remove(t));
            dbSet.Setup(set => set.RemoveRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>(ts =>
            {
                foreach (var t in ts) { table.Remove(t); }
            });

            return dbSet;
        }
    }
}
