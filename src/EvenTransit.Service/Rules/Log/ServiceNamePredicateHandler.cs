using System.Linq.Expressions;
using EvenTransit.Core.Domain.Common;
using EvenTransit.Core.Dto.Service.Log;

namespace EvenTransit.Service.Rules.Log
{
    public class ServiceNamePredicateHandler : LogPredicateHandler
    {
        public override BinaryExpression HandleRequest(ParameterExpression parameter, Expression leftExpression, LogSearchRequestDto request)
        {
            var expression = TrueExpression;
            BinaryExpression andExp;
            
            if (!string.IsNullOrEmpty(request.ServiceName))
            {
                var property = Expression.Property(parameter, "ServiceName");
                var expressionValue = Expression.Constant(request.ServiceName);
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