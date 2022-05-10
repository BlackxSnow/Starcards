using System.Collections.Generic;
using Utility;

namespace Utility.Enumerators
{
    public abstract class EnumeratorBase<T> : IEnumerator<T>
    {
        protected ImprovedLinkedList<T> _List;
        public ImprovedLinkedListNode<T> CurrentNode { get; protected set; }
        protected ImprovedLinkedListNode<T> _StartNode;
        public T Current => CurrentNode.Value;
        object System.Collections.IEnumerator.Current => Current;

        public void Dispose() { }
        public abstract bool MoveNext();
        public void Reset()
        {
            CurrentNode = _StartNode;
        }
    }
    public class ForwardsEnumerator<T> : EnumeratorBase<T>
    {
        public override bool MoveNext()
        {
            CurrentNode = CurrentNode.Next;
            return CurrentNode != null;
        }
        public ForwardsEnumerator(ImprovedLinkedList<T> list, ImprovedLinkedListNode<T> head)
        {
            _List = list;
            _StartNode = new ImprovedLinkedListNode<T>(default, null, head);
            CurrentNode = _StartNode;
        }
    }
    public class BackwardsEnumerator<T> : EnumeratorBase<T>
    {
        public override bool MoveNext()
        {
            CurrentNode = CurrentNode.Previous;
            return CurrentNode != null;
        }
        public BackwardsEnumerator(ImprovedLinkedList<T> list, ImprovedLinkedListNode<T> tail)
        {
            _List = list;
            _StartNode = new ImprovedLinkedListNode<T>(default, tail, null);
            CurrentNode = _StartNode;
        }
    }
}