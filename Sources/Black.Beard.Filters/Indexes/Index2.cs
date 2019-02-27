using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bb.Filters.Indexes
{

    public class Index2
    {

        static Index2()
        {
            Index2._hashNull = FilterContstants.Null.GetHashCode();
            Index2._hashPipe = '|'.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Calculate(int value, object value2)
        {

            int result = value;

            if (value2 == null)
                result ^= Index2._hashNull;
            else
                result ^= value2.GetHashCode();

            int result2 = result ^ Index2._hashPipe;

            //Debug.WriteLine($"{value} -> '{value2 ?? FilterContstants.Null}' -> {result} -> '|' -> {result2}");

            return result2;

        }

        protected static readonly int _hashNull;
        protected static readonly int _hashPipe;
    }

    public class Index2<TExpected> : Index2
    {

        public Index2()
        {

            _properties = typeof(TExpected).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            _method = typeof(Index2).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(c => c.Name == "Calculate")
                .First()
                ;
        }

        public Index2(string nullValue, params Expression<Func<TExpected, object>>[] properties)
        {

            HashSet<string> hashDedoublon = new HashSet<string>();
            _properties = GetProperties(properties);
            _method = typeof(Index2).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(c => c.Name == "Calculate")
                .First()
                ;

        }

        public void Sort(TExpected[] datas)
        {

            foreach (var item in datas)
            {
                int hash = 0;

                foreach (var property in _properties)
                    hash = Calculate(hash, Read(item, property));

                _hash.Add(hash);

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

        private Func<object, int> GetDelegate<TModel>(Expression<Func<TModel, object>>[] properties)
        {

            PropertyInfo[] props = GetProperties(properties);

            var sourceParameterExpr = Expression.Parameter(typeof(object), "arg1");
            //var hashExpr = Expression.Variable(typeof(int), "sb");
            var varExpr = Expression.Variable(typeof(TModel), "i");

            List<Expression> _expression = new List<Expression>()
            {
                //Expression.Assign(hashExpr, Expression.Constant(0)),
                Expression.Assign(varExpr, Expression.Convert(sourceParameterExpr, typeof(TModel)))
            };

            AppendProperties(props, varExpr, _expression);

            var blk = Expression.Block(new ParameterExpression[] { varExpr }, _expression);

            Expression<Func<object, int>> lbd = Expression.Lambda<Func<object, int>>
            (
                blk,
                new ParameterExpression[] { sourceParameterExpr }
            );

            return lbd.Compile();
        }

        private void AppendProperties(PropertyInfo[] props, ParameterExpression varExpr, List<Expression> _expression)
        {

            Expression lastResult = Expression.Constant(0);

            foreach (var property in props)
            {

                Expression prop1 = Expression.Property(varExpr, property);
                if (property.PropertyType != typeof(object))
                    prop1 = Expression.Convert(prop1, typeof(object));

                lastResult = Expression.Call(null, _method, lastResult, prop1);
            }

            _expression.Add(lastResult);

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

        private HashSet<int> _hash = new HashSet<int>();
        private readonly MethodInfo _method;
        private readonly PropertyInfo[] _properties;

    }

}
