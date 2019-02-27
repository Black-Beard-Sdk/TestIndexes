namespace Bb.Filters.Tree
{

    /// <summary>
    /// Containt all values of the tree
    /// </summary>
    /// <typeparam name="TExpected"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    internal class NodeValue<TExpected, TModel>
    {

        public NodeValue(object key)
        {
            Value = key;
        }

        public object Value { get; }

        public Node<TExpected, TModel> Node { get; internal set; }

    }



}


