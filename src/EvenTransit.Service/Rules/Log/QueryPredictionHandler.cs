using EvenTransit.Domain.Entities;
using EvenTransit.Service.Dto.Log;
using System.Linq.Expressions;

namespace EvenTransit.Service.Rules.Log;

public class QueryPredictionHandler : LogPredicateHandler
{
    public override BinaryExpression HandleRequest(ParameterExpression parameter, Expression leftExpression, LogSearchRequestDto request)
    {
        if (string.IsNullOrEmpty(request.Query))
            return Expression.AndAlso(TrueExpression, TrueExpression);

        var property = Expression.Property(parameter, nameof(Logs.Details.Request.Body));
        var expressionValue = Expression.Constant(request.Query);
        var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var expression = Expression.Call(property, method, expressionValue);

        return Successor != null
            ? Expression.AndAlso(leftExpression, Successor.HandleRequest(parameter, expression, request))
            : Expression.AndAlso(leftExpression, expression);
    }
}
