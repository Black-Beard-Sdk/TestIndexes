using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Bb.Filters
{

    internal class MemberFind : ExpressionVisitor
    {

        private MemberFind()
        {
            Members = new List<PropertyInfo>();
        }

        public static List<PropertyInfo> Get(Expression expression)
        {
            var visitor = new MemberFind();
            visitor.Visit(expression);
            return visitor.Members;
        }

        protected override Expression VisitMember(MemberExpression node)
        {

            if (node.Member is PropertyInfo prop)
                Members.Add(prop);

            return base.VisitMember(node);

        }

        public List<PropertyInfo> Members { get; private set; }

    }

}


