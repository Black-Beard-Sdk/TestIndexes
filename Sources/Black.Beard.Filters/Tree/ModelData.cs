using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bb.Filters.Tree
{

    internal class ModelData
    {

        public ModelData(object value, PropertyInfo property, ModelData parent)
        {
            _parent = parent;
            Value = value;
            PropertyName = property.Name;
            PropertyType = property.PropertyType;
        }

        public object Value { get; }

        public string PropertyName { get; }

        public Type PropertyType { get; }

        public bool IsNull => Value == (object)FilterContstants.Null;

        public bool IsEmpty => _tree.Count == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Read(object model, PropertyInfo property)
        {
            return property.GetValue(model, null) ?? FilterContstants.Null;
        }

        internal void Append(PropertyInfo[] properties, int index, object item)
        {

            var property = properties[index];

            var value = ModelData.Read(item, property);

            if (!_tree.TryGetValue(value, out ModelData model))
                _tree.Add(value, (model = new ModelData(value, property, this)));

            index++;
            if (index < properties.Length)
                model.Append(properties, index, item);

        }

        internal void Reduce()
        {
            // On recopie la liste pour pouvoir la modifier pendant qu on itere dedans
            var tt = _tree.Values.ToList();
            foreach (var item in tt)        // On commence par la fin
                item.Reduce();


            List<object> toRemove = new List<object>();     // On collecte tout les derniers noeuds null et sans enfants
            foreach (var item in _tree)
                if (item.Value.IsNull && item.Value.IsEmpty)
                    toRemove.Add(item.Key);

            foreach (var item in toRemove)                  // On les supprime
                _tree.Remove(item);

        }

        internal Dictionary<object, ModelData> _tree = new Dictionary<object, ModelData>();
        private readonly ModelData _parent;

    }

}


