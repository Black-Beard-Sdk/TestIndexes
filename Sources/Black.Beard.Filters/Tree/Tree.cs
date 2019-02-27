using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bb.Filters.Tree
{

    public class Tree<TExpected>
    {

        /// <summary>
        /// Sort on specific properties
        /// </summary>
        /// <param name="properties"></param>
        public Tree(params Expression<Func<TExpected, object>>[] properties)
        {

            HashSet<string> hashDedoublon = new HashSet<string>();
            List<PropertyInfo> props = new List<PropertyInfo>();

            foreach (var item in properties)
            {
                var member = MemberFind.Get(item).First();
                if (hashDedoublon.Add(member.Name))
                    props.Add(member);
            }

            // On ajoute les proprietes manquantes
            var p2 = typeof(TExpected).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var member in p2)
                if (hashDedoublon.Add(member.Name))
                    props.Add(member);

            _properties = props.ToArray();

        }

        /// <summary>
        /// Sort on all properties
        /// </summary>
        public Tree()
        {
            _properties = typeof(TExpected).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public void Sort(TExpected[] datas)
        {

            foreach (var item in datas)
            {

                var property = _properties[0];
                var value = ModelData.Read(item, property);

                if (!_tree.TryGetValue(value, out ModelData model))
                    _tree.Add(value, (model = new ModelData(value, property, null)));

                if (1 < _properties.Length)
                    model.Append(_properties, 1, item);

            }

        }

        public void Reduce()
        {
            foreach (var item in _tree)
                item.Value.Reduce();
        }

        public Predicate<object> GetEvaluator<TModel>(params Expression<Func<TModel, TExpected, bool>>[] mathings)
        {

            // On pourrait imaginer un algo qui match par convention de nom

            // On récupère les noms de propriété du model expected
            Dictionary<string, string> _filters = new Dictionary<string, string>();
            foreach (var item in mathings)
            {

                var nameModels = MemberFind.Get(item);

                var nameModel = nameModels
                    .Where(c => c.DeclaringType == typeof(TExpected))
                    .Select(c => c.Name)
                    .First();

                var nameModel2 = nameModels
                    .Where(c => c.DeclaringType == typeof(TModel))
                    .Select(c => c.Name)
                    .First();

                _filters.Add(nameModel, nameModel2);

            }

            var filter = new Filter<TExpected, TModel>(_tree, _filters);
            Predicate<object> fnc = a => filter._node.Evaluate(a);

            return fnc;

        }

        private readonly PropertyInfo[] _properties;
        private Dictionary<object, ModelData> _tree = new Dictionary<object, ModelData>();

    }

}


