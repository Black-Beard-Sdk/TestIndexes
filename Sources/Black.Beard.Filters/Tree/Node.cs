using System;
using System.Collections.Generic;

namespace Bb.Filters.Tree
{

    /// <summary>
    /// Node contains property reader accessor
    /// </summary>
    /// <typeparam name="TExpected"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    internal class Node<TExpected, TModel>
    {

        public Node(string name, (Func<object, object>, Action<object, object>) accessors)
        {
            Name = name;
            Get = accessors.Item1;
            _dic = new Dictionary<Object, NodeValue<TExpected, TModel>>();
        }

        internal void AddValue(ModelData value, Dictionary<string, string> filters)
        {

            var k = value.Value;
            var n = new NodeValue<TExpected, TModel>(value.Value);

            _dic.Add(k, n);

            foreach (var item in value._tree)
            {
                if (n.Node == null)
                {
                    var name = filters[item.Value.PropertyName]; // On récupère l'expression de comparaison qui va pour le noeud à traiter                        
                    var _accessor = Filter<TExpected, TModel>._properties[name];
                    var e = new Node<TExpected, TModel>(item.Value.PropertyName, _accessor);
                    n.Node = e;
                }

                n.Node.AddValue(item.Value, filters);

            }

        }

        internal bool Evaluate(object context)
        {

            if (_dic.Count > 0)
            {

                var value = Get(context) ?? FilterContstants.Null;

                if (_dic.TryGetValue(value, out NodeValue<TExpected, TModel> n))
                {

                    if (n.Node == null)
                        return true;

                    return n.Node.Evaluate(context);

                }

            }

            return _dic.Count == 0;

        }

        public string Name { get; }

        public Func<object, object> Get { get; }

        private readonly Dictionary<object, NodeValue<TExpected, TModel>> _dic;

    }



}


