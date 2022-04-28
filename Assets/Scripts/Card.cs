using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Draggable))]
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
    public const float StackZOffset = 0.01f;
    /// <summary>
    /// Offset towards the bottom of the screen from the stacked parent's position.
    /// </summary>
    public const float StackYOffset = 0.25f;
    /// <summary>
    /// Default offset towards the camera that unstacked cards will sit at.
    /// </summary>
    public const float DefaultZ = 0.01f;

    #endregion

    private Draggable _Draggable;

    private bool _IsDraggable = true;

    private Card _StackedOn = null;
    private Card _StackedChild = null;

    private void Awake()
    {
        _Draggable = GetComponent<Draggable>();

        _Draggable.enabled = _IsDraggable;
        _Draggable.OnStartDrag += OnPickup;
        _Draggable.OnEndDrag += OnDrop;
    }

    public void SetDraggable(bool isDraggable)
    {
        _IsDraggable = isDraggable;
        _Draggable.enabled = _IsDraggable;
    }



    /// <summary>
    /// Remove the currently stacked child.
    /// </summary>
    public void UnstackChild()
    {
        if (_StackedChild == null) Debug.LogWarning("UnstackChild called with no existing child.");
        _StackedChild = null;
    }

    /// <summary>
    /// Attempt to stack a child on this card. Return whether successful.
    /// </summary>
    /// <param name="toStack"></param>
    /// <returns></returns>
    public bool TryStackChild(Card toStack)
    {
        if (_StackedChild != null) return false;

        _StackedChild = toStack;
        return true;
    }

    /// <summary>
    /// Raycast downwards to determine if there is a stack below the card.
    /// </summary>
    /// <returns></returns>
    private Card CheckForStackUnderCard()
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
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
        transform.position = new Vector3(transform.position.x, transform.position.y, -DefaultZ - PickupHeight);
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
            transform.position = new Vector3(transform.position.x, transform.position.y, -DefaultZ);
        }
    }

    /// <summary>
    /// Sets the correct position / offset of this card from its current parent.
    /// </summary>
    private void MoveToStackedCard()
    {
        Debug.Assert(_StackedOn, "MoveToStackedCard cannot be called with a null parent.");
        transform.position = new Vector3(_StackedOn.transform.position.x, _StackedOn.transform.position.y - StackYOffset, _StackedOn.transform.position.z - StackZOffset);
    }

    /// <summary>
    /// Stack this card on another. Throws on failure. Null is the same as calling Unstack.
    /// </summary>
    /// <param name="other"></param>
    public void StackOn(Card other)
    {
        _StackedOn?.UnstackChild();
        _StackedOn = other;

        if (other != null)
        {
            if (!other.TryStackChild(this))
            transform.SetParent(other.transform);
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
            _StackedOn?.UnstackChild();
            _StackedOn = other;
            transform.SetParent(other.transform);
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
        _StackedOn?.UnstackChild();
        _StackedOn = null;
        transform.SetParent(GameManager.CardContainer);
    }
}
