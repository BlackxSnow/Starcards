using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Interfaces;
using Systems;
using TMPro;
using UnityEngine.UI;
using Data;
using Interactions;
using System;
using Utility;
using System.Threading.Tasks;
using System.Threading;
using UnityAsync;

[RequireComponent(typeof(Draggable), typeof(Moveable))]
public class Card : MonoBehaviour
{
    #region CardLayoutVariables
    /// <summary>
    /// Height the card is moved towards the camera ('up') when drag begins
    /// </summary>
    public const float PickupHeight = 0.5f;
    /// <summary>
    /// Offset towards the camera ('up') the card is moved for each stack.
    /// </summary>
    public const float StackZOffset = 0.002f;
    /// <summary>
    /// Offset towards the bottom of the screen from the stacked parent's position.
    /// </summary>
    public float StackYOffset { get; protected set; } = 0.25f;
    #endregion

    public Moveable MoveComponent { get; private set; }
    private Draggable _DragComponent;
    private Interactor _Interactor;

    public bool IsBeingDestroyed { get; private set; } = false;

    public StackNode StackNode { get; private set; }

    /// <summary>
    /// Called on modification of this card's stack.
    /// </summary>
    public event Action<Card> StackChanged;

    public string CardName { get => Data.Name; }
    public CardData Data { get; protected set; }

    private int _Quantity;
    public int Quantity { get => _Quantity;
        set
        {
            if (value == 1) _QuantityText.text = "";
            else _QuantityText.text = $"x{value}";
            _Quantity = value;
        }
    }

    public Card MultiCard { get; private set; } = null;
    public Card MultiCardTail { get; set; } = null;

    [Header("UI references")]
    [SerializeField]
    private GameObject _ProgressBarPrefab;
    [SerializeField]
    private Transform _ProgressBarContainer;
    [SerializeField]
    private TextMeshProUGUI _NameText;
    [SerializeField]
    private TextMeshProUGUI _QuantityText;
    [SerializeField]
    private RawImage _CardImage;
    [SerializeField]
    private Canvas _CardInfo;
    public Canvas CardInfo { get => _CardInfo; }
    [Header("Component references")]
    [SerializeField]
    private Collider _Collider;
    [SerializeField]
    private MeshRenderer _Renderer;

    #region Initialisation
    private void Awake()
    {
        _DragComponent = GetComponent<Draggable>();
        MoveComponent = GetComponent<Moveable>();
        //_DragComponent.enabled = _IsDraggable;
        _DragComponent.enabled = true;

        StackNode = new StackNode();
        new Stack(this);

        _DragComponent.DragStarted += OnPickup;
        _DragComponent.DragEnded += OnDrop;
        _DragComponent.PointerDownCallback = OnPointerDown;
        _DragComponent.PointerUpCallback = OnPointerUp;
    }

    public void Initialise(CardData data)
    {
        Data = data;
        _NameText.text = Data.Name;
        _CardImage.texture = Data.Image;
        _Interactor = new Interactor(this);
        Quantity = 1;
    }

    #endregion

    public void NotifyStackChange()
    {
        StackChanged?.Invoke(this);
    }

    /// <summary>
    /// Return next card that is not a multicard child.
    /// </summary>
    /// <returns></returns>
    public Card GetNextMajor()
    {
        if (MultiCardTail != null) return MultiCardTail.StackNode.Next?.Value;
        if (MultiCard != null) return MultiCard.GetNextMajor();
        return StackNode.Next?.Value;
    }

    public Card[] GetChildren(int quantity, bool includeSelf = false)
    {
        List<Card> children = new List<Card>(quantity);

        foreach (Card child in StackNode.Stack.EnumeratorFrom(includeSelf ? StackNode.Node : StackNode.Next))
        {
            children.Add(child);
            if (children.Count == quantity) return children.ToArray();
        }
        throw new ArgumentOutOfRangeException($"Expected {quantity} children, got {children.Count}. First: {children[0].name}, Last: {children[children.Count - 1].name}");
    }

    /// <summary>
    /// Return a progress bar attached to this card.
    /// </summary>
    /// <returns></returns>
    public UI.ProgressBar RequestProgressBar()
    {
        return Instantiate(_ProgressBarPrefab, _ProgressBarContainer).GetComponent<UI.ProgressBar>();
    }

    public void PrepareForDestroy()
    {
        SetDraggable(false);
        SetInteractable(Interactor.InteractorState.None);
        SetActive(true);
        IsBeingDestroyed = true;
    }

    public void SetInteractable(Interactor.InteractorState state)
    {
        _Interactor.SetState(state);
    }

    public void SetDraggable(bool isDraggable)
    {
        //_IsDraggable = isDraggable;
        _DragComponent.enabled = isDraggable;
    }

    public void SetActive(bool isActive)
    {
        _Collider.enabled = isActive;
        _Renderer.enabled = isActive;
        _CardInfo.enabled = isActive;
    }

    public bool IsSameType(Card other)
    {
        return CardName == other?.CardName;
    }
    public bool IsCard(string cardName)
    {
        return CardName == cardName;
    }
    public bool InSameStack(Card other)
    {
        return StackNode.Stack == other.StackNode.Stack;
    }

    protected void OnPointerDown(PointerEventData eventData)
    {
        if (Quantity == 1 || CardHoverManager.CursorQuantity == 0)
        {
            _DragComponent.StartDrag(eventData);
            return;
        }

        Card splitHead = Stack.GetMultiChild(this, Quantity - CardHoverManager.CursorQuantity);
        splitHead.StartDrag(eventData);
        _PointerUpRedirect = splitHead.StopDrag;
    }

    private Action<PointerEventData> _PointerUpRedirect;

    protected void OnPointerUp(PointerEventData eventData)
    {
        if (_PointerUpRedirect != null)
        {
            _PointerUpRedirect(eventData);
            _PointerUpRedirect = null;
            return;
        }
        _DragComponent.StopDrag(eventData);
    }

    public Card[] RequestCards(ref Dictionary<string, int> request)
    {
        if (request.TryGetValue(CardName, out int requestQuantity))
        {
            if (Quantity >= requestQuantity)
            {
                request.Remove(CardName);
                return GetChildren(requestQuantity, Quantity == requestQuantity);
            }
            else
            {
                request[CardName] -= Quantity;
                return GetChildren(Quantity, true);
            }
        }
        return new Card[0];
    }

    public void CreateCard(string cardName, Vector3 position = default, Vector3 moveOffset = default)
    {
        bool isStack = CheckForStackUnder(position + moveOffset + new Vector3(0,0,-PickupHeight), out Stack other);
        Card card = CardManager.SpawnCard(cardName, position);
        // TODO: Implement output waypoints
        Moveable.EasingOverride = Easings.EaseOutCubic;
        if (isStack)
        {
            card.StackOn(other);
        }
        else if (moveOffset != default)
        {
            _ = card.MoveComponent.MoveTo(card.transform.position + moveOffset);
        }
        Moveable.EasingOverride = null;
    }

    public bool HasChildCards(string cardName, int quantity, ref List<Card> cards)
    {
        if (cards == null)
        {
            cards = new List<Card>();
        }

        foreach(Card child in StackNode.Stack.EnumeratorFrom(StackNode.Node))
        {
            if (child.IsCard(cardName))
            {
                cards.Add(child);
                quantity--;
                if (quantity <= 0) return true;
            }
        }
        return false;
    }
    
    public bool HasParentCards(string cardName, int quantity, ref List<Card> cards)
    {
        if (cards == null)
        {
            cards = new List<Card>();
        }

        foreach(Card parent in StackNode.Stack.ReverseEnumeratorFrom(StackNode.Node))
        {
            if (parent.IsCard(cardName))
            {
                cards.Add(parent);
                quantity--;
                if (quantity <= 0) return true;
            }
        }
        return false;
    }

    public bool HasParentCard(Card card)
    {
        foreach (Card parent in StackNode.Stack.ReverseEnumeratorFrom(StackNode.Node))
        {
            if (parent == card)
            {
                return true;
            }
        }
        return false;
    }

    #region Stacking
    /// <summary>
    /// Split stack into two with this card as the new head.
    /// </summary>
    public void Unstack()
    {
        if (StackNode.Previous == null) return; // Is head of stack.
        StackNode.Stack.Split(StackNode);
        transform.SetParent(GameManager.CardContainer);
    }
    /// <summary>
    /// Remove card from stack without splitting stack.
    /// </summary>
    public void Extract()
    {
        if (StackNode.Previous == null && StackNode.Next == null) return; // Is alone in stack.
        StackNode.Stack.Extract(StackNode);
        transform.SetParent(GameManager.CardContainer);
    }
    public void StackOn(Stack other)
    {
        other.Concat(StackNode.Stack);
        transform.SetParent(StackNode.Previous.Value.transform);
        CardInfo.sortingOrder = StackNode.Previous.Value.CardInfo.sortingOrder + 1;

        foreach (Card card in StackNode.Stack.EnumeratorFrom(StackNode.Next))
        {
            card.CardInfo.sortingOrder = card.StackNode.Previous.Value.CardInfo.sortingOrder + 1;
        }
        //MoveToStackedCard();
    }

    public void MultiCardStack(Card multi)
    {
        MultiCard = multi;
        SetDraggable(false);
        MoveToMultiStack();
    }
    public void MultiCardUnstack()
    {
        MultiCard = null;
        SetDraggable(true);
        SetActive(true);
    }
    
    #endregion

    /// <summary>
    /// Sets the correct position / offset of this card from its current parent.
    /// </summary>
    public void MoveToStackedCard()
    {
        Vector3 offset = new Vector3(0, -StackYOffset, -StackZOffset);
        if (StackNode.Previous.Value.MultiCard != null) offset.y *= 2;
        MoveToStackedCard(offset);
    }
    public void MoveToStackedCard(Vector3 offset)
    {
        _ = MoveComponent.MoveTo(StackNode.Previous.Value.transform, offset);
    }

    public void MoveToMultiStack()
    {
        if (MultiCard == null) throw new InvalidOperationException("MultiCard cannot be null.");
        _ = MoveComponent.MoveTo(StackNode.Previous.Value.transform, () => SetActive(false));
    }

    public void StartDrag(PointerEventData eventData)
    {
        _DragComponent.StartDrag(eventData);
    }

    public void StopDrag(PointerEventData eventData)
    {
        _DragComponent.StopDrag(eventData);
    }

    /// <summary>
    /// Executed on start of drag ('Picking up' card).
    /// </summary>
    private void OnPickup()
    {
        Unstack();
        _ = MoveComponent.MoveZ(-StackZOffset - PickupHeight);
    }

    /// <summary>
    /// Executed on end of drag ('Dropping' card).
    /// </summary>
    private void OnDrop(Vector2 cursorOffset)
    {
        bool isStack = CheckForStackUnder(transform.position + new Vector3(cursorOffset.x, cursorOffset.y, 0), out Stack other) || CheckForStackUnder(transform.position, out other);
        if (isStack && other != StackNode.Stack)
        {
            StackOn(other);
        }
        else
        {
            _ = MoveComponent.MoveTo(new Vector3(transform.position.x, transform.position.y, -StackZOffset));
        }
    }

    /// <summary>
    /// Raycast downwards to determine if there is a stack below the given position.
    /// </summary>
    /// <returns></returns>
    private bool CheckForStackUnder(Vector3 position, out Stack stack)
    {
        Ray ray = new Ray(position, Vector3.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform.parent && hit.collider.transform.parent.TryGetComponent(out Card otherCard))
            {
                stack = otherCard.StackNode.Stack;
                return true;
            }
        }
        stack = null;
        return false;
    }
}
