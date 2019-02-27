using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bb.Filters.Tree
{
    /// <summary>
    /// object specific filter
    /// </summary>
    /// <typeparam name="TExpected"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    internal class Filter<TExpected, TModel>
    {

        static Filter()
        {
            Filter<TExpected, TModel>._properties = typeof(TModel).GetPropertiesAccessor();
        }

        public Filter(Dictionary<object, ModelData> tree, Dictionary<string, string> _filters)
        {

            foreach (KeyValuePair<object, ModelData> item in tree)
            {

                if (_node == null)
                {
                    var name = item.Value.PropertyName;
                    var name2 = _filters[name];
                    var _accessor = Filter<TExpected, TModel>._properties[name2];
                    _node = new Node<TExpected, TModel>(name, _accessor);
                }

                _node.AddValue(item.Value, _filters);

            }


        }

        internal readonly Node<TExpected, TModel> _node;
        internal static readonly Dictionary<string, (Func<object, object>, Action<object, object>)> _properties;

    }



}


