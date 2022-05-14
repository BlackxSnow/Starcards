using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems;
using UnityEngine;
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

    private int CountBetween(Card first, Card last, bool firstInclusive = true, bool lastInclusive = false)
    {
        Debug.Assert(first != last, "Cannot count between a card and itself.");
        Card current = first;
        int count = firstInclusive ? 0 : -1;
        while (current != last)
        {
            Debug.Assert(current.StackNode.Next != null, $"First '{first.name}' and last '{last.name}' are not in the same stack or in the wrong order.");
            current = current.StackNode.Next.Value;
            count++;
        }
        if (lastInclusive) count++;
        return count;
    }

    private void ResetCardStackNodes(EnumeratorBase<Card> enumerator)
    {
        foreach (Card card in enumerator)
        {
            card.StackNode.Stack = this;
            card.StackNode.Node = enumerator.CurrentNode;
        }
    }

    /// <summary>
    /// Checks if a MultiCard has valid cards to include in the immediate stack and updates the MultiCard.
    /// </summary>
    /// <param name="start"></param>
    private void CheckMultiCard(Card start)
    {
        Card multiCard = start.MultiCard != null ? start.MultiCard : start;

        Card current = start;
        int count = 0;
        while (multiCard.IsSameType(current.StackNode.Next?.Value))
        {
            current = current.StackNode.Next.Value;
            current.MultiCardStack(multiCard);
            count++;
        }
        if (count == 0)
        {
            start.StackNode.Next.Value.MoveToStackedCard();
            return;
        }
        if (current.StackNode.Next != null)
        {
            current.StackNode.Next.Value.MoveToStackedCard();
        }
        multiCard.MultiCardTail = current;
        multiCard.Quantity += count;
    }

    /// <summary>
    /// Update a MultiCard to a new quantity and tail. Does not update the old children.
    /// </summary>
    /// <param name="multi"></param>
    /// <param name="quantity"></param>
    private void UpdateMultiCard(Card multi, int quantity)
    {
        if (quantity == multi.Quantity) return;
        if (quantity == 1)
        {
            multi.Quantity = quantity;
            multi.MultiCardTail = null;
            return;
        }

        if (quantity > multi.Quantity)
        {
            Card current = multi;
            for (int i = 1; i < multi.Quantity; i++)
            {
                current = current.StackNode.Next.Value;
            }

            for (int i = multi.Quantity; i < quantity; i++)
            {
                current = current.StackNode.Next.Value;
                current.MultiCardStack(multi);
            }
            multi.Quantity = quantity;
            multi.MultiCardTail = current;
        }
        else
        {
            Card current = multi;
            for (int i = 1; i < multi.Quantity; i++)
            {
                current = current.StackNode.Next.Value;
            }
            multi.Quantity = quantity;
            multi.MultiCardTail = current;
        }
    }

    private void SplitMultiCard(Card splitHead)
    {
        Card firstMulti = splitHead.MultiCard;
        int firstQuantity = CountBetween(firstMulti, splitHead);
        int secondQuantity = firstMulti.Quantity - firstQuantity;
        UpdateMultiCard(firstMulti, firstQuantity);
        splitHead.MultiCardUnstack();
        UpdateMultiCard(splitHead, secondQuantity);
        if (secondQuantity == 1) splitHead.StackNode.Next?.Value.MoveToStackedCard();
    }

    public static Card GetMultiChild(Card multi, int distance)
    {
        var current = multi.StackNode.Node;
        for (int i = 0; i < distance; i++)
        {
            current = current.Next;
        }
        return current.Value;
    }
    public Stack Split(StackNode splitHead)
    {
        if (splitHead.Node.Value.MultiCard != null) SplitMultiCard(splitHead.Node.Value);
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
        CheckMultiCard(otherHead.Previous.Value);
    }
    public void Extract(StackNode toExtract)
    {
        var parentNode = toExtract.Previous;
        var childNode = toExtract.Next;

        if (toExtract.Node.Value.MultiCard != null)
        {
            Card extractCard = toExtract.Node.Value;
            extractCard.MultiCard.Quantity--;
            if (extractCard == extractCard.MultiCard.MultiCardTail)
            {
                extractCard.MultiCard.MultiCardTail = extractCard.MultiCard.Quantity > 1 ? parentNode.Value : null;
            }
            toExtract.Node.Value.MultiCardUnstack();
        }
        Cards.Remove(toExtract.Node);
        new Stack(toExtract.Node.Value);
        if (childNode != null)
        {
            childNode.Value.transform.SetParent(parentNode != null ? parentNode.Value.transform : GameManager.CardContainer);
            if (childNode.Value.MultiCard == null)
            {
                childNode.Value.MoveToStackedCard();
            }
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
