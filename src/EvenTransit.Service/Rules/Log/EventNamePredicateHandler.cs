using System.Linq.Expressions;
using EvenTransit.Service.Dto.Log;

namespace EvenTransit.Service.Rules.Log
{
    public class EventNamePredicateHandler : LogPredicateHandler
    {
        public override BinaryExpression HandleRequest(ParameterExpression parameter, Expression leftExpression, LogSearchRequestDto request)
        {
            BinaryExpression andExp;
            var expression = TrueExpression;
            
            if (!string.IsNullOrEmpty(request.EventName))
            {
                var property = Expression.Property(parameter, "EventName");
                var expressionValue = Expression.Constant(request.EventName);
                expression = Expression.Equal(property, expressionValue);
                andExp = Expression.AndAlso(TrueExpression, expression);
            }
            else
                andExp = Expression.AndAlso(TrueExpression, TrueExpression);

            if (Successor != null)
                andExp = Expression.AndAlso(leftExpression, Successor.HandleRequest(parameter, expression, request));

            return andExp;
        }
    }
}