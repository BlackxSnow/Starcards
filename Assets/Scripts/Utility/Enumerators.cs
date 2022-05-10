using System.Collections.Generic;
using Utility;

namespace Utility.Enumerators
{
    public abstract class EnumeratorBase<T> : IEnumerator<T>
    {
        protected ImprovedLinkedList<T> _List;
        protected ImprovedLinkedListNode<T> _CurrentNode;
        protected ImprovedLinkedListNode<T> _StartNode;
        public T Current => _CurrentNode.Value;
        object System.Collections.IEnumerator.Current => Current;

        public void Dispose() { }
        public abstract bool MoveNext();
        public void Reset()
        {
            _CurrentNode = _StartNode;
        }
    }
    public class ForwardsEnumerator<T> : EnumeratorBase<T>
    {
        public override bool MoveNext()
        {
            _CurrentNode = _CurrentNode.Next;
            return _CurrentNode != null;
        }
        public ForwardsEnumerator(ImprovedLinkedList<T> list, ImprovedLinkedListNode<T> head)
        {
            _List = list;
            _StartNode = new ImprovedLinkedListNode<T>(default, null, head);
            _CurrentNode = _StartNode;
        }
    }
    public class BackwardsEnumerator<T> : EnumeratorBase<T>
    {
        public override bool MoveNext()
        {
            _CurrentNode = _CurrentNode.Previous;
            return _CurrentNode != null;
        }
        public BackwardsEnumerator(ImprovedLinkedList<T> list, ImprovedLinkedListNode<T> tail)
        {
            _List = list;
            _StartNode = new ImprovedLinkedListNode<T>(default, tail, null);
            _CurrentNode = _StartNode;
        }
    }
}