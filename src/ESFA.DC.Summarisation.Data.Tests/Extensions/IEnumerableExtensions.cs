using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Data.Tests.DbAsync;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ESFA.DC.Summarisation.Data.Tests.Extensions
{
    public static class IEnumerableExtensions
    {
        public static DbSet<T> AsMockDbSet<T>(this IEnumerable<T> sourceList)
            where T : class
        {
            var mockData = sourceList.AsQueryable();

            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(mockData.Provider));
            mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetEnumerator()).Returns(new TestDbAsyncEnumerator<T>(mockData.GetEnumerator()));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            return mockSet.Object;
        }
    }
}
