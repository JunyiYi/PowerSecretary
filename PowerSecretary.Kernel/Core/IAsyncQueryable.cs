using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PowerSecretary.Kernel
{
    public interface IAsyncQueryable<T>
    {
        Task<IEnumerable<T>> LoadAsync(CancellationToken cancelToken = default);
        IAsyncQueryProvider Provider { get; }
        Expression Expression { get; }
    }

    public interface IAsyncQueryProvider
    {
        IAsyncQueryable<T> CreateQuery<T>(Expression expression);
    }

    public static class AsyncQueryable
    {
        public static IAsyncQueryable<T> Where<T>(this IAsyncQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            EnsureNotNull(source, nameof(source));
            EnsureNotNull(predicate, nameof(predicate));
            return CreateQuery(Where, source, predicate);
        }

        public static IAsyncQueryable<TResult> Select<TSource, TResult>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            EnsureNotNull(source, nameof(source));
            EnsureNotNull(selector, nameof(selector));
            return CreateQuery(Select, source, selector);
        }

        private static IAsyncQueryable<R> CreateQuery<T, TExpr, R>(Func<IAsyncQueryable<T>, TExpr, IAsyncQueryable<R>> f, IAsyncQueryable<T> source, TExpr expression)
            where TExpr : Expression
            => source.Provider.CreateQuery<R>(Expression.Call(f.Method, source.Expression, expression));

        private static void EnsureNotNull(object param, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
