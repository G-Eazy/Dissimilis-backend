using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dissimilis.Core.Context
{

    public static class PredicateBuilder
    {
        // Creates a predicate that evaluates to true.
        public static Expression<Func<T, bool>> True<T>() { return param => true; }

        // Creates a predicate that evaluates to false.
        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        // Creates a predicate expression from the specified lambda expression.
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

        // Combines the first predicate with the second using the logical "and".
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
            {
                return second;
            }

            return first.Compose(second, Expression.AndAlso);
        }

        // Combines the first predicate with the second using the logical "or".
        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
            {
                return second;
            }

            return first.Compose(second, Expression.OrElse);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, Expression<Func<T, string>> exp, bool reverse)
        {
            if (reverse)
                queryable = queryable.OrderByDescending(exp);
            else
                queryable = queryable.OrderBy(exp);
            return queryable;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, Expression<Func<T, DateTimeOffset>> exp, bool reverse)
        {
            if (reverse)
                queryable = queryable.OrderByDescending(exp);
            else
                queryable = queryable.OrderBy(exp);
            return queryable;
        }


        // Negates the predicate.
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        public static IQueryable<T> WhereOptional<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate = null) =>
            predicate == null
                ? source
                : source.Where(predicate);


        public static IEnumerable<T> WhereOptional<T>(this IEnumerable<T> source, Func<T, bool> predicate = null) =>
            predicate == null
                ? source
                : source.Where(predicate);


        // Combines the first expression with the second using the specified merge function.
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            readonly Dictionary<ParameterExpression, ParameterExpression> _map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                if (_map.TryGetValue(p, out var replacement))
                {
                    p = replacement;
                }
                return base.VisitParameter(p);
            }
        }
    }
}
