using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bb.Filters.Indexes
{

    public class Index<TExpected>
    {

        public Index()
        {
            _properties = typeof(TExpected).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            _nullValue = FilterContstants.Null.ToString();

            _methods = typeof(StringBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(c => c.Name == "Append" && c.GetParameters().Length == 1)
                .Select(c => new KeyValuePair<Type, MethodInfo>(c.GetParameters()[0].ParameterType, c))
                .ToDictionary(c => c.Key, c => c.Value)
                ;

            _methodObj = _methods[typeof(object)];
            _methodchar = _methods[typeof(char)];
            _calculateMethod = typeof(Crc32).GetMethod("Calculate", BindingFlags.Static | BindingFlags.Public);

        }

        public Index(string nullValue, params Expression<Func<TExpected, object>>[] properties)
        {

            HashSet<string> hashDedoublon = new HashSet<string>();
            _properties = GetProperties(properties);
            _nullValue = FilterContstants.Null;

            _methods = typeof(StringBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(c => c.Name == "Append" && c.GetParameters().Length == 1)
                .Select(c => new KeyValuePair<Type, MethodInfo>(c.GetParameters()[0].ParameterType, c))
                .ToDictionary(c => c.Key, c => c.Value)
                ;

            _methodObj = _methods[typeof(object)];
            _methodchar = _methods[typeof(char)];
            _calculateMethod = typeof(Crc32).GetMethod("Calculate", BindingFlags.Static | BindingFlags.Public);

        }

        public void Sort(TExpected[] datas)
        {

            StringBuilder sb = new StringBuilder(_properties.Length * (1024 / 2));

            foreach (var item in datas)
            {

                foreach (var property in _properties)
                {
                    sb.Append(Read(item, property) ?? _nullValue);
                    sb.Append('|');
                }
                var key = Crc32.Calculate(sb);
                _hash.Add(key);
                sb.Clear();

            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object Read(object model, PropertyInfo property)
        {
            return property.GetValue(model, null) ?? FilterContstants.Null;
        }

        public Predicate<object> GetEvaluator<TModel>(params Expression<Func<TModel, object>>[] properties)
        {
            var @delegate = GetDelegate(properties);
            bool func(object p) => _hash.Contains(@delegate(p));
            return func;
        }

        private Func<object, uint> GetDelegate<TModel>(Expression<Func<TModel, object>>[] properties)
        {

            PropertyInfo[] props = GetProperties(properties);

            var sourceParameterExpr = Expression.Parameter(typeof(object), "arg1");
            var sbExpr = Expression.Variable(typeof(StringBuilder), "sb");
            var varExpr = Expression.Variable(typeof(TModel), "i");

            List<Expression> _expression = new List<Expression>()
            {
                Expression.Assign(sbExpr, Expression.New(typeof(StringBuilder))),
                Expression.Assign(varExpr, Expression.Convert(sourceParameterExpr, typeof(TModel)))
            };

            AppendProperties(props, sbExpr, varExpr, _expression);

            _expression.Add(Expression.Call(null, _calculateMethod, sbExpr));

            var blk = Expression.Block(new ParameterExpression[] { sbExpr, varExpr }, _expression);

            Expression<Func<object, uint>> lbd = Expression.Lambda<Func<object, uint>>(
                blk,
                new ParameterExpression[] { sourceParameterExpr }
                );

            return lbd.Compile();
        }

        private void AppendProperties(PropertyInfo[] props, ParameterExpression sbExpr, ParameterExpression varExpr, List<Expression> _expression)
        {
            foreach (var property in props)
            {
                Expression e = Expression.Coalesce(Expression.Convert(Expression.Property(varExpr, property), typeof(object)), Expression.Convert(Expression.Constant(_nullValue), typeof(object)));
                e = Expression.Call(sbExpr, _methodObj, e);
                _expression.Add(e);
                _expression.Add(Expression.Call(sbExpr, _methodchar, Expression.Constant('|')));
            }
        }

        private static PropertyInfo[] GetProperties<TModel>(Expression<Func<TModel, object>>[] properties)
        {
            HashSet<string> hashDedoublon = new HashSet<string>();
            List<PropertyInfo> props = new List<PropertyInfo>();

            foreach (var item in properties)
            {
                var member = MemberFind.Get(item).First();
                if (hashDedoublon.Add(member.Name))
                    props.Add(member);
            }

            return props.ToArray();

        }

        private HashSet<uint> _hash = new HashSet<uint>();
        private readonly string _nullValue;
        private readonly Dictionary<Type, MethodInfo> _methods;
        private readonly MethodInfo _methodObj;
        private readonly MethodInfo _methodchar;
        private readonly MethodInfo _calculateMethod;
        private readonly PropertyInfo[] _properties;

    }

}
