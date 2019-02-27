using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Bb.Filters.Tree
{

    internal static class PropertyHelper
    {

        public static Dictionary<string, (Func<object, object>, Action<object, object>)> GetPropertiesAccessor(this Type self)
        {

            Dictionary<string, (Func<object, object>, Action<object, object>)> result = new Dictionary<string, (Func<object, object>, Action<object, object>)>();
            var properties = self.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var item in properties)
                result.Add(item.Name, item.GetPropertyAccessor(self));

            return result;

        }

        public static (Func<object, object>, Action<object, object>) GetPropertyAccessor(this PropertyInfo property, Type componentType)
        {

            var Member = property;
            var Name = property.Name;
            var DeclaringType = property.DeclaringType;
            var m = property.GetMethod ?? property.SetMethod;
            var IsStatic = m != null ? (m.Attributes & MethodAttributes.Static) == MethodAttributes.Static : false;
            var Type = property.PropertyType;

            Action<object, object> SetValue = null;
            Func<object, object> GetValue = null;

            #region Get

            if (property.CanRead)
            {
                var sourceParameterExpr = Expression.Parameter(typeof(object), "i");

                GetValue =
                Expression.Lambda<Func<object, object>>
                (
                    Expression.Convert
                    (
                        Expression.Property
                        (
                            IsStatic ? null : Expression.Convert(sourceParameterExpr, componentType),
                            property
                        ),
                        typeof(object)
                    ),
                    sourceParameterExpr
                ).Compile();
            }

            #endregion

            #region Set

            if (property.CanWrite)
            {

                var targetObjectParameter = Expression.Parameter(typeof(object), "i");
                var convertedObjectParameter = Expression.ConvertChecked(targetObjectParameter, componentType);
                var valueParameter = Expression.Parameter(typeof(object), "value");
                var convertedValueParameter = Expression.ConvertChecked(valueParameter, property.PropertyType);
                var propertyExpression = Expression.Property(IsStatic ? null : convertedObjectParameter, property);

                SetValue =
                Expression.Lambda<Action<object, object>>
                (
                    Expression.Assign
                    (
                        propertyExpression,
                        convertedValueParameter
                    ),
                    targetObjectParameter,
                    valueParameter
                ).Compile();

            }

            #endregion

            return (GetValue, SetValue);

        }

        public static Expression<Func<object, object>> GetPropertyAccessor2(this PropertyInfo property, Type componentType)
        {

            var Member = property;
            var Name = property.Name;
            var DeclaringType = property.DeclaringType;
            var Type = property.PropertyType;

            Expression<Func<object, object>> GetValue = null;

            if (property.CanRead)
            {
                var sourceParameterExpr = Expression.Parameter(typeof(object), "i");
                GetValue = Expression.Lambda<Func<object, object>>
                (
                    Expression.Convert
                    (
                        Expression.Property
                        (
                            Expression.Convert(sourceParameterExpr, componentType),
                            property
                        ),
                        typeof(object)
                    ),
                    sourceParameterExpr
                );
            }

            return GetValue;

        }


    }
}
