using System;
using System.Linq.Expressions;
using EvenTransit.Core.Domain.Common;
using EvenTransit.Core.Dto.Service.Log;

namespace EvenTransit.Service.Rules.Log
{
    public class DateFromPredicateHandler : LogPredicateHandler
    {
        public override BinaryExpression HandleRequest(ParameterExpression parameter, Expression leftExpression, LogSearchRequestDto request)
        {
            BinaryExpression andExp;
            var expression = TrueExpression;
            
            if (request.LogDateFrom != DateTime.MinValue)
            {
                var property = Expression.Property(parameter, "CreatedOn");
                var expressionValue = Expression.Constant(request.LogDateFrom);
                expression = Expression.GreaterThanOrEqual(property, expressionValue);
                andExp = Expression.AndAlso(leftExpression, expression);
            }
            else
                andExp = Expression.AndAlso(TrueExpression, TrueExpression);

            if (Successor != null)
                andExp = Expression.AndAlso(leftExpression, Successor.HandleRequest(parameter, expression, request));

            return andExp;
        }
    }
}