using PrintingJobTracker.Domain.Specifications;
using System.Linq.Expressions;

namespace PrintingJobTracker.Application.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public int Skip { get; private set; }
        public int Take { get; private set; }

        public BaseSpecification()
        {
            Criteria = x => true;
        }

        public void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = Criteria is null ? criteria : CombineExpressions(Criteria, criteria);
        }

        public void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        private static Expression<Func<T, bool>> CombineExpressions(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }
}
