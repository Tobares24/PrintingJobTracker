using System.Linq.Expressions;

namespace PrintingJobTracker.Domain.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        int Skip { get; }
        int Take { get; }
        void AddCriteria(Expression<Func<T, bool>> criteria);
        void ApplyPaging(int skip, int take);
    }
}
