using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ImprovedLinkedList
{
    private class Container
    {
        public int Value;
        public Container(int value) { Value = value; }
    }

    // A Test behaves as an ordinary method
    [Test]
    public void Create()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        Assert.IsNull(list.Head);
        Assert.IsNull(list.Tail);
        Assert.AreEqual(0, list.Count);
    }

    [Test]
    public void AddFirst_Single_Empty()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(15);
        Assert.AreEqual(15, list.Head.Value);
        Assert.AreEqual(15, list.Tail.Value);
        Assert.AreEqual(1, list.Count);
    }

    [Test]
    public void AddFirst_Single_Occupied()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(5);
        list.AddFirst(2);
        Assert.AreEqual(2, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(2, list.Count);
    }

    [Test]
    public void AddFirst_Multiple_Empty()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(new int[] { 1, 2, 3, 4, 5 });
        Assert.AreEqual(1, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(5, list.Count);
    }

    [Test]
    public void AddFirst_Multiple_Occupied()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(new int[] { 1, 2, 3, 4, 5 });
        list.AddFirst(new int[] { -4, -3, -2, -1, 0 });
        Assert.AreEqual(-4, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(10, list.Count);
    }

    [Test]
    public void AddLast_Single_Empty()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddLast(15);
        Assert.AreEqual(15, list.Head.Value);
        Assert.AreEqual(15, list.Tail.Value);
        Assert.AreEqual(1, list.Count);
    }

    [Test]
    public void AddLast_Single_Occupied()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddLast(5);
        list.AddLast(2);
        Assert.AreEqual(5, list.Head.Value);
        Assert.AreEqual(2, list.Tail.Value);
        Assert.AreEqual(2, list.Count);
    }

    [Test]
    public void AddLast_Multiple_Empty()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddLast(new int[] { 1, 2, 3, 4, 5 });
        Assert.AreEqual(1, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(5, list.Count);
    }

    [Test]
    public void AddLast_Multiple_Occupied()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddLast(new int[] { 1, 2, 3, 4, 5 });
        list.AddLast(new int[] { 6, 7, 8, 9, 10 });
        Assert.AreEqual(1, list.Head.Value);
        Assert.AreEqual(10, list.Tail.Value);
        Assert.AreEqual(10, list.Count);
    }

    [Test]
    public void Find_Exists()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddLast(new int[] { 1, 2, 3, 4, 5 });
        var node = list.Find(3);
        Assert.AreEqual(3, node.Value);
    }
    [Test]
    public void Find_Invalid()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddLast(new int[] { 1, 2, 3, 4, 5 });
        Assert.Throws(typeof(System.ArgumentOutOfRangeException), ()=>list.Find(6));
    }

    [Test]
    public void AddBefore_Single()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(5);
        list.AddBefore(5, 2);
        Assert.AreEqual(2, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(2, list.Count);
    }

    [Test]
    public void AddBefore_Multiple()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(5);
        list.AddBefore(5, new int[] { 1, 2, 3, 4 });
        Assert.AreEqual(1, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(5, list.Count);
    }

    [Test]
    public void AddAfter_Single()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(5);
        list.AddAfter(5, 2);
        Assert.AreEqual(5, list.Head.Value);
        Assert.AreEqual(2, list.Tail.Value);
        Assert.AreEqual(2, list.Count);
    }

    [Test]
    public void AddAfter_Multiple()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(5);
        list.AddAfter(5, new int[] { 6, 7, 8, 9 });
        Assert.AreEqual(5, list.Head.Value);
        Assert.AreEqual(9, list.Tail.Value);
        Assert.AreEqual(5, list.Count);
    }

    [Test]
    public void RemoveFirst()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(new int[] { 1, 2, 3, 4, 5 });
        list.RemoveFirst();
        Assert.AreEqual(2, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(4, list.Count);
        list.RemoveFirst();
        Assert.AreEqual(3, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(3, list.Count);
    }

    [Test]
    public void RemoveLast()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(new int[] { 1, 2, 3, 4, 5 });
        list.RemoveLast();
        Assert.AreEqual(1, list.Head.Value);
        Assert.AreEqual(4, list.Tail.Value);
        Assert.AreEqual(4, list.Count);
        list.RemoveLast();
        Assert.AreEqual(1, list.Head.Value);
        Assert.AreEqual(3, list.Tail.Value);
        Assert.AreEqual(3, list.Count);
    }

    [Test]
    public void Remove()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(new int[] { 1, 2, 3, 4, 5 });
        list.Remove(1);
        Assert.AreEqual(2, list.Head.Value);
        Assert.AreEqual(5, list.Tail.Value);
        Assert.AreEqual(4, list.Count);
        list.Remove(5);
        Assert.AreEqual(2, list.Head.Value);
        Assert.AreEqual(4, list.Tail.Value);
        Assert.AreEqual(3, list.Count);
        list.Remove(3);
        Assert.AreEqual(2, list.Head.Value);
        Assert.AreEqual(4, list.Tail.Value);
        Assert.AreEqual(2, list.Count);

        Assert.AreEqual(4, list.Head.Next.Value);
    }

    [Test]
    public void Split()
    {
        Utility.ImprovedLinkedList<int> list = new Utility.ImprovedLinkedList<int>();
        list.AddFirst(new int[] { 1, 2, 3, 4, 5 });
        var splitList = list.Split(3);

        Assert.AreEqual(1, list.Head.Value);
        Assert.AreEqual(2, list.Tail.Value);
        Assert.AreEqual(2, list.Count);

        Assert.AreEqual(3, splitList.Head.Value);
        Assert.AreEqual(5, splitList.Tail.Value);
        Assert.AreEqual(3, splitList.Count);
    }
}
