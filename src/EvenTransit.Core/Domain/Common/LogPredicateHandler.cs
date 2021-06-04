using System.Linq.Expressions;
using EvenTransit.Core.Dto.Service.Log;

namespace EvenTransit.Core.Domain.Common
{
    public abstract class LogPredicateHandler
    {
        protected LogPredicateHandler Successor;
        protected readonly Expression TrueExpression;

        protected LogPredicateHandler()
        {
            TrueExpression = Expression.Constant(true);
        }

        public void SetSuccessor(LogPredicateHandler successor)
        {
            Successor = successor;
        }
 
        public abstract BinaryExpression HandleRequest(ParameterExpression parameter, Expression leftExpression, LogSearchRequestDto logSearchRequestDto);
    }
}