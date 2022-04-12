using System.Linq.Expressions;
using EvenTransit.Service.Dto.Log;

namespace EvenTransit.Service.Rules.Log;

public class DateFromPredicateHandler : LogPredicateHandler
{
    public override BinaryExpression HandleRequest(ParameterExpression parameter, Expression leftExpression,
        LogSearchRequestDto request)
    {
        BinaryExpression andExp;
        var expression = TrueExpression;

        if (request.LogDateFrom != null)
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
        else
            andExp = Expression.AndAlso(leftExpression, expression);

        return andExp;
    }
}
