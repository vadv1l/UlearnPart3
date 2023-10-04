using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection.Differentiation
{
    public class Algebra : ExpressionVisitor
    {
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function) =>
            new DifferentiateVisitor().Differentiate(function);
    }

    public class DifferentiateVisitor : ExpressionVisitor
    {
        private static readonly Dictionary<MethodInfo, IDifferentiable> s_methods = 
            new Dictionary<MethodInfo, IDifferentiable>();

        static DifferentiateVisitor()
        {
            s_methods.Add(typeof(Math).GetMethod(nameof(Math.Sin)) ?? throw new ArgumentNullException(), 
                new SinDifferentiable());
            s_methods.Add(typeof(Math).GetMethod(nameof(Math.Cos)) ?? throw new ArgumentNullException(), 
                new CosDifferentiable());
        }

        public Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function) =>
            Expression.Lambda<Func<double, double>>(Visit(function.Body), function.Parameters);

        protected override Expression VisitParameter(ParameterExpression _) => Expression.Constant(1d, typeof(double));

        protected override Expression VisitConstant(ConstantExpression _) => Expression.Constant(0d, typeof(double));

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    return Expression.Add(Visit(expression.Left), Visit(expression.Right));
                case ExpressionType.Subtract:
                    return Expression.Subtract(Visit(expression.Left), Visit(expression.Right));
                case ExpressionType.Multiply:
                    return Expression.Add(Expression.Multiply(Visit(expression.Left), expression.Right),
                        Expression.Multiply(expression.Left, Visit(expression.Right)));
                default:
                    return Expression.Divide(Expression.Subtract(Expression.Multiply(
                                Visit(expression.Left), expression.Right),
                            Expression.Multiply(expression.Left, Visit(expression.Right))),
                        Expression.Multiply(expression.Right, expression.Right));
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (!s_methods.ContainsKey(expression.Method)) throw new ArgumentException(expression.Method.Name);
            return s_methods[expression.Method].Differentiate(expression);
        }
    }

    public interface IDifferentiable
    {
        Expression Differentiate(MethodCallExpression expression);
    }

    public class SinDifferentiable : DifferentiateVisitor, IDifferentiable
    {
        public Expression Differentiate(MethodCallExpression expression)
        {
            var cosMethod = typeof(Math).GetMethod(nameof(Math.Cos)) ?? throw new ArgumentNullException();
            var sinArgument = expression.Arguments[0];
            return Expression.Multiply(Expression.Call(null, cosMethod, sinArgument),
             Visit(sinArgument));
        }
    }

    public class CosDifferentiable : DifferentiateVisitor, IDifferentiable
    {
        public Expression Differentiate(MethodCallExpression expression)
        {
            var sinMethod = typeof(Math).GetMethod(nameof(Math.Sin)) ?? throw new ArgumentNullException();
            var cosArgument = expression.Arguments[0];
            return Expression.Multiply(Expression.Multiply(Expression.Constant(-1d, typeof(double)),
                Expression.Call(null, sinMethod, cosArgument)), Visit(cosArgument));
        }
    }
}