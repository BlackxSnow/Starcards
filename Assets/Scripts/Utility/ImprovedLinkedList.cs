using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Enumerators;

namespace Utility
{
    public class ImprovedLinkedListNode<T>
    { 
        public ImprovedLinkedListNode<T> Previous { get; internal set; }
        public ImprovedLinkedListNode<T> Next { get; internal set; }
        public T Value { get; set; }

        public ImprovedLinkedListNode(T value, ImprovedLinkedListNode<T> previous, ImprovedLinkedListNode<T> next)
        {
            Value = value;
            Previous = previous;
            Next = next;
        }
    }
    public class ImprovedLinkedList<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return new ForwardsEnumerator<T>(this, Head);
        }
        public IEnumerator<T> GetEnumerator(ImprovedLinkedListNode<T> start)
        {
            return new ForwardsEnumerator<T>(this, start);
        }
        public IEnumerator<T> GetEnumerator(T value)
        {
            return new ForwardsEnumerator<T>(this, Find(value));
        }
        public IEnumerator<T> GetEnumerator(int offset)
        {
            ImprovedLinkedListNode<T> start = Head;
            for (int i = 0; i < offset; i++)
            {
                start = start.Next;
                if (start == null) throw new ArgumentOutOfRangeException();
            }
            return new ForwardsEnumerator<T>(this, start);
        }

        public IEnumerator<T> GetReverseEnumerator()
        {
            return new BackwardsEnumerator<T>(this, Head);
        }
        public IEnumerator<T> GetReverseEnumerator(ImprovedLinkedListNode<T> start)
        {
            return new BackwardsEnumerator<T>(this, start);
        }
        public IEnumerator<T> GetReverseEnumerator(T value)
        {
            return new BackwardsEnumerator<T>(this, Find(value));
        }
        public IEnumerator<T> GetReverseEnumerator(int offset)
        {
            ImprovedLinkedListNode<T> start = Tail;
            for (int i = 0; i < offset; i++)
            {
                start = start.Previous;
                if (start == null) throw new ArgumentOutOfRangeException();
            }
            return new BackwardsEnumerator<T>(this, start);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ImprovedLinkedListNode<T> Head { get; private set; }
        public ImprovedLinkedListNode<T> Tail { get; private set; }

        private int _Count;
        private bool _IsCountDirty;
        public int Count { get => _IsCountDirty ? RecalculateCount() : _Count; }

        private int RecalculateCount()
        {
            _IsCountDirty = false;
            _Count = 0;
            foreach (var _ in this)
            {
                _Count++;
            }
            return _Count;
        }

        public ImprovedLinkedListNode<T> Find(T value)
        {
            ImprovedLinkedListNode<T> current = Head;
            while (current != null)
            {
                if (current.Value.Equals(value))
                {
                    return current;
                }
                current = current.Next;
            }
            throw new ArgumentOutOfRangeException();
        }

        public void AddFirst(T value)
        {
            ImprovedLinkedListNode<T> newNode = new ImprovedLinkedListNode<T>(value, null, Head);
            if (Head != null) Head.Previous = newNode;
            Head = newNode;
            if (Tail == null) Tail = newNode;
            _Count++;
        }
        public void AddFirst(IEnumerable<T> values)
        {
            Insert(null, Head, values);
        }
        private void Insert(ImprovedLinkedListNode<T> previous, ImprovedLinkedListNode<T> next, T value)
        {
            ImprovedLinkedListNode<T> newNode = new ImprovedLinkedListNode<T>(value, previous, next);
            if (previous != null) previous.Next = newNode;
            if (next != null) next.Previous = newNode;
            _Count++;
        }
        private void Insert(ImprovedLinkedListNode<T> previous, ImprovedLinkedListNode<T> next, IEnumerable<T> values)
        {
            if (!values.Any()) return;
            ImprovedLinkedListNode<T> first = new ImprovedLinkedListNode<T>(values.First(), previous, null);
            ImprovedLinkedListNode<T> last = first;

            foreach (var value in values.Skip(1))
            {
                ImprovedLinkedListNode<T> newNode = new ImprovedLinkedListNode<T>(value, last, null);
                last.Next = newNode;
                last = newNode;
            }

            if (previous != null)
            {
                previous.Next = first;
            }
            else
            {
                Head = first;
            }
            if (next != null)
            {
                next.Previous = last;
                last.Next = next;
            }
            else
            {
                Tail = last;
            }
            _Count += values.Count();
        }
        public void AddLast(T value)
        {
            ImprovedLinkedListNode<T> newNode = new ImprovedLinkedListNode<T>(value, Tail, null);
            if (Tail != null) Tail.Next = newNode;
            Tail = newNode;
            if (Head == null) Head = newNode;
            _Count++;
        }
        public void AddLast(IEnumerable<T> values)
        {
            Insert(Tail, null, values);
        }
        public void AddBefore(ImprovedLinkedListNode<T> before, T value)
        {
            if (before == Head)
            {
                AddFirst(value);
                return;
            }
            Insert(before.Previous, before, value);
        }
        public void AddBefore(ImprovedLinkedListNode<T> before, IEnumerable<T> values)
        {
            Insert(before.Previous, before, values);
        }
        public void AddBefore(T before, T value)
        {
            AddBefore(Find(before), value);
        }
        public void AddBefore(T before, IEnumerable<T> values)
        {
            AddBefore(Find(before), values);
        }
        public void AddAfter(ImprovedLinkedListNode<T> after, T value)
        {
            if (after == Tail)
            {
                AddLast(value);
                return;
            }
            Insert(after, after.Next, value);
        }
        public void AddAfter(ImprovedLinkedListNode<T> after, IEnumerable<T> values)
        {
            Insert(after, after.Next, values);
        }
        public void AddAfter(T after, T value)
        {
            AddAfter(Find(after), value);
        }
        public void AddAfter(T after, IEnumerable<T> values)
        {
            AddAfter(Find(after), values);
        }

        private void Attach(ImprovedLinkedListNode<T> a, ImprovedLinkedListNode<T> b)
        {
            a.Next = b;
            b.Previous = a;
        }

        public void RemoveFirst()
        {
            if (Head == null) throw new InvalidOperationException();
            Head = Head.Next;
            Head.Previous = null;
            _Count--;
        }
        public void RemoveLast()
        {
            if (Tail == null) throw new InvalidOperationException();
            Tail = Tail.Previous;
            Tail.Next = null;
            _Count--;
        }
        public void Remove(ImprovedLinkedListNode<T> toRemove)
        {
            if (toRemove == Head)
            {
                RemoveFirst();
                return;
            }
            if (toRemove == Tail)
            {
                RemoveLast();
                return;
            }

            Attach(toRemove.Previous, toRemove.Next);
            _Count--;
        }
        public void Remove(T toRemove)
        {
            Remove(Find(toRemove));
        }

        public ImprovedLinkedList<T> Split(ImprovedLinkedListNode<T> splitHead)
        {
            if (splitHead == Head) throw new InvalidOperationException("Cannot split from head of list.");
            ImprovedLinkedList<T> newList = new ImprovedLinkedList<T>() { Head = splitHead, Tail = Tail, _IsCountDirty = true };
            splitHead.Previous.Next = null;
            Tail = splitHead.Previous;
            splitHead.Previous = null;
            _IsCountDirty = true;
            return newList;
        }
        public ImprovedLinkedList<T> Split(T splitHead)
        {
            return Split(Find(splitHead));
        }

        public ImprovedLinkedList()
        {
            Head = null;
            Tail = null;
        }
        public ImprovedLinkedList(IEnumerable<T> collection)
        {
            Insert(null, null, collection);
        }
    }
}
