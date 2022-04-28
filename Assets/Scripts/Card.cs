using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Draggable))]
public class Card : MonoBehaviour
{
    private Draggable _Draggable;

    private bool _IsDraggable = true;

    private void Awake()
    {
        _Draggable = GetComponent<Draggable>();

        _Draggable.enabled = _IsDraggable;
    }

    public void SetDraggable(bool isDraggable)
    {
        _IsDraggable = isDraggable;
        _Draggable.enabled = _IsDraggable;
    }
}
