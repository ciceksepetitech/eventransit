using System;
using System.Linq.Expressions;
using EvenTransit.Core.Domain.Common;
using EvenTransit.Core.Dto.Service.Log;

namespace EvenTransit.Service.Rules.Log
{
    public class DateToPredicateHandler : LogPredicateHandler
    {
        public override BinaryExpression HandleRequest(ParameterExpression parameter, Expression leftExpression, LogSearchRequestDto request)
        {
            BinaryExpression andExp;
            var expression = TrueExpression;
            
            if (request.LogDateTo != DateTime.MinValue)
            {
                var property = Expression.Property(parameter, "CreatedOn");
                var expressionValue = Expression.Constant(request.LogDateTo);
                expression = Expression.LessThanOrEqual(property, expressionValue);
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