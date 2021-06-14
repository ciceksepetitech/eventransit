using System.Linq.Expressions;
using EvenTransit.Core.Enums;
using EvenTransit.Service.Dto.Log;

namespace EvenTransit.Service.Rules.Log
{
    public class LogTypePredicateHandler : LogPredicateHandler
    {
        public override BinaryExpression HandleRequest(ParameterExpression parameter, Expression leftExpression, LogSearchRequestDto request)
        {
            BinaryExpression andExp;
            var expression = TrueExpression;
            
            if (request.LogType != LogType.None)
            {
                var property = Expression.Property(parameter, "LogType");
                var expressionValue = Expression.Constant(request.LogType);
                expression = Expression.Equal(property, expressionValue);
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