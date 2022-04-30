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

[RequireComponent(typeof(Draggable), typeof(VelocityMove))]
public class Card : MonoBehaviour
{
    #region CardLayoutVars

    /// <summary>
    /// Height the card is moved towards the camera ('up') when drag begins
    /// </summary>
    public const float PickupHeight = 0.5f;
    /// <summary>
    /// Offset towards the camera ('up') the card is moved when stacking on another.
    /// </summary>
    public const float StackZOffset = 0.002f;
    /// <summary>
    /// Offset towards the bottom of the screen from the stacked parent's position.
    /// </summary>
    public const float StackYOffset = 0.25f;
    /// <summary>
    /// Default offset towards the camera that unstacked cards will sit at.
    /// </summary>
    public const float DefaultZ = 0.002f;

    #endregion

    private Draggable _Draggable;
    private VelocityMove _VelocityMove;
    private Interactor _Interactor;

    private bool _IsDraggable = true;

    public Card StackedOn { get; protected set; } = null;
    public Card StackedChild { get; protected set; } = null;

    /// <summary>
    /// Called on stacking or unstacking of this card or one of its parents.
    /// </summary>
    public event Action<Card> OnStackStateChange;

    public string CardName { get => Data.Name; }

    public CardData Data { get; protected set; }

    [SerializeField]
    private GameObject _ProgressBar;
    [SerializeField]
    private Transform _ProgressBarContainer;

    [SerializeField]
    private TextMeshProUGUI _NameText;

    [SerializeField]
    private RawImage _CardImage;

    [SerializeField]
    private Canvas _CardInfo;

    [SerializeField]
    private Collider _Collider;

    private void Awake()
    {
        _Draggable = GetComponent<Draggable>();
        _VelocityMove = GetComponent<VelocityMove>();

        _Draggable.Directable = _VelocityMove;
        _Draggable.enabled = _IsDraggable;
        
        _Draggable.OnStartDrag += OnPickup;
        _Draggable.OnEndDrag += OnDrop;
    }

    public void Initialise(CardData data)
    {
        Data = data;
        _NameText.text = Data.Name;
        _CardImage.texture = Data.Image;
        _Interactor = new Interactor(this);
    }

    /// <summary>
    /// Return a progress bar attached to this card.
    /// </summary>
    /// <returns></returns>
    public UI.ProgressBar RequestProgressBar()
    {
        return Instantiate(_ProgressBar, _ProgressBarContainer).GetComponent<UI.ProgressBar>();
    }

    public void SetDraggable(bool isDraggable)
    {
        _IsDraggable = isDraggable;
        _Draggable.enabled = _IsDraggable;
    }

    public void SpawnCard(string cardName, Vector3 position = default)
    {
        Card card = CardManager.SpawnCard(cardName);
        // TODO: Implement output waypoints
        card.transform.position = position;
        card._VelocityMove.TargetPosition = transform.position + new Vector3(1f, 0, -DefaultZ);
    }

    /// <summary>
    /// Return whether this card's child list contains an amount of a certain card.
    /// </summary>
    /// <param name="cardName"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool HasStackedCards(string cardName, int quantity)
    {
        Card current = this;
        while (current.StackedChild != null)
        {
            current = current.StackedChild;
            if (current.Data.Name == cardName)
            {
                quantity--;
                if (quantity <= 0) return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// Appends the found cards to the provided list.
    /// </summary>
    /// <param name="cardName"></param>
    /// <param name="quantity"></param>
    /// <param name="cards"></param>
    /// <returns></returns>
    public bool HasStackedCards(string cardName, int quantity, ref List<Card> cards)
    {
        if (cards == null)
        {
            cards = new List<Card>();
        }

        Card current = this;
        while (current.StackedChild != null)
        {
            current = current.StackedChild;
            if (current.Data.Name == cardName)
            {
                cards.Add(current);
                quantity--;
                if (quantity <= 0) return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Returns whether this card is below 'parent' in its stack.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public bool HasParent(Card parent)
    {
        Card current = this;
        while (current.StackedOn != null)
        {
            current = current.StackedOn;
            if (current == parent)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Raycast downwards to determine if there is a stack below the card.
    /// </summary>
    /// <returns></returns>
    private Card CheckForStackUnderCard()
    {
        Ray ray = new Ray(new Vector3(_VelocityMove.TargetPosition.x, _VelocityMove.TargetPosition.y, transform.position.z), Vector3.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform.parent && hit.collider.transform.parent.TryGetComponent(out Card otherCard))
            {
                return otherCard;
            }
        }
        return null;
    }

    /// <summary>
    /// Executred on start of drag ('Picking up' card).
    /// </summary>
    private void OnPickup()
    {
        Unstack();
        _VelocityMove.TargetPosition = new Vector3(transform.position.x, transform.position.y, -DefaultZ - PickupHeight);
    }

    /// <summary>
    /// Executed on end of drag ('Dropping' card).
    /// </summary>
    private void OnDrop()
    {
        Card other = CheckForStackUnderCard();
        if (other)
        {
            TryStackOn(other);
        }
        else
        {
            _VelocityMove.TargetPosition = new Vector3(_VelocityMove.TargetPosition.x, _VelocityMove.TargetPosition.y, -DefaultZ);
        }
    }

    /// <summary>
    /// Sets the correct position / offset of this card from its current parent.
    /// </summary>
    private void MoveToStackedCard()
    {
        Debug.Assert(StackedOn, "MoveToStackedCard cannot be called with a null parent.");
        _VelocityMove.IsStacking = true;
        _VelocityMove.TargetPosition = new Vector3(StackedOn.transform.position.x, StackedOn.transform.position.y - StackYOffset, StackedOn.transform.position.z - StackZOffset);
    }

    /// <summary>
    /// Remove the currently stacked child.
    /// </summary>
    public void UnstackChild()
    {
        if (StackedChild == null) Debug.LogWarning("UnstackChild called with no existing child.");
        StackedChild = null;
        OnStackStateChange?.Invoke(this);
    }

    /// <summary>
    /// Attempt to stack a child on this card. Return whether successful.
    /// </summary>
    /// <param name="toStack"></param>
    /// <returns></returns>
    public bool TryStackChild(Card toStack)
    {
        if (StackedChild != null) return false;

        StackedChild = toStack;
        OnStackStateChange?.Invoke(this);
        return true;
    }

    /// <summary>
    /// Stack this card on another. Throws on failure. Null is the same as calling Unstack.
    /// </summary>
    /// <param name="other"></param>
    public void StackOn(Card other)
    {
        StackedOn?.UnstackChild();
        StackedOn = other;

        if (other != null)
        {
            if (!other.TryStackChild(this))
            transform.SetParent(other.transform);
            CallStackChangeFullStack();
            MoveToStackedCard();
        }
    }

    /// <summary>
    /// Attempt to stack on another card and return the result. Null is an invalid argument.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool TryStackOn(Card other)
    {
        if (other.TryStackChild(this))
        {
            StackedOn?.UnstackChild();
            StackedOn = other;
            transform.SetParent(other.transform);
            CallStackChangeFullStack();
            MoveToStackedCard();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Unstacks this card.
    /// </summary>
    public void Unstack()
    {
        _VelocityMove.IsStacking = false;
        _VelocityMove.ResetFinished();
        StackedOn?.UnstackChild();
        StackedOn = null;
        transform.SetParent(GameManager.CardContainer);
        CallStackChangeFullStack();
    }

    /// <summary>
    /// Calls OnStackStateChange on this card and all its children.
    /// </summary>
    public void CallStackChangeFullStack()
    {
        Card current = this;
        OnStackStateChange?.Invoke(this);
        while (current.StackedChild != null)
        {
            current = current.StackedChild;
            current.OnStackStateChange?.Invoke(current);
        }

        current = this;
        while (current.StackedOn != null)
        {
            current = current.StackedOn;
            current.OnStackStateChange?.Invoke(current);
        }
    }

    private void OnDestroy()
    {
        CardManager.RemoveCard(this);
    }
}
