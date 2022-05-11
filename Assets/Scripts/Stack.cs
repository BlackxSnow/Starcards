using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems;
using Utility;
using Utility.Enumerators;

public class StackNode
{
    public ImprovedLinkedListNode<Card> Node { get; set; }
    public Stack Stack { get; set; }
    public ImprovedLinkedListNode<Card> Previous => Node.Previous;
    public ImprovedLinkedListNode<Card> Next => Node.Next;
}

public class Stack
{
    public ImprovedLinkedList<Card> Cards { get; private set; }
    public Card First => Cards.Head.Value;
    public Card Last => Cards.Tail.Value;
    public int Count => Cards.Count;

    private void NotifyCards()
    {
        foreach(Card card in Cards)
        {
            card.NotifyStackChange();
        }
    }

    private void ResetCardStackNodes(EnumeratorBase<Card> enumerator)
    {
        foreach (Card card in enumerator)
        {
            card.StackNode.Stack = this;
            card.StackNode.Node = enumerator.CurrentNode;
        }
    }

    public Stack Split(StackNode splitHead)
    {
        Stack newStack = new Stack(Cards.Split(splitHead.Node));
        NotifyCards();
        return newStack;
    }
    public void Concat(Stack other)
    {
        ImprovedLinkedListNode<Card> otherHead = other.Cards.Head;
        Cards.Concat(other.Cards);
        ResetCardStackNodes((EnumeratorBase<Card>)Cards.GetEnumerator(otherHead));
        NotifyCards();
    }
    public void Extract(StackNode toExtract)
    {
        var parentNode = toExtract.Previous;
        var childNode = toExtract.Next;

        Cards.Remove(toExtract.Node);
        new Stack(toExtract.Node.Value);
        if (childNode != null)
        {
            childNode.Value.transform.SetParent(parentNode != null ? parentNode.Value.transform : GameManager.CardContainer);
            childNode.Value.MoveToStackedCard();
        }
    }

    public IEnumerator<Card> EnumeratorFrom(ImprovedLinkedListNode<Card> start) => Cards.GetEnumerator(start);
    public IEnumerator<Card> ReverseEnumeratorFrom(ImprovedLinkedListNode<Card> start) => Cards.GetReverseEnumerator(start);

    public Stack()
    {
        Cards = new ImprovedLinkedList<Card>();
    }
    public Stack(Card card)
    {
        Cards = new ImprovedLinkedList<Card>();
        card.StackNode.Node = Cards.AddFirst(card);
        card.StackNode.Stack = this;
        NotifyCards();
    }
    public Stack(ImprovedLinkedList<Card> cards)
    {
        Cards = cards;
        ResetCardStackNodes((EnumeratorBase<Card>)Cards.GetEnumerator());
        NotifyCards();
    }
}
