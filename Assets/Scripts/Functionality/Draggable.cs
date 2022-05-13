using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static readonly Plane Board = new Plane(Vector3.back, 0);

    public event Action DragStarted;
    public event Action<Vector2> DragEnded;

    public Action<PointerEventData> PointerDownCallback;
    public Action<PointerEventData> PointerUpCallback;

    private Vector2 _CursorOffset;

    private bool _IsDragging = false;

    private (Ray ray, float distance) RaycastBoard(Vector3 origin)
    {
        Ray ray = Camera.main.ScreenPointToRay(origin);
        bool isIntersecting = Board.Raycast(ray, out float distance);

        if (!isIntersecting)
        {
            throw new System.Exception("No intersection between board and cursor.");
        }
        return (ray, distance);
    }

    private Vector3 PosFromRayAndDistance(Ray ray, float distance)
    {
        return Camera.main.transform.position + (ray.direction * distance);
    }

    private void Awake()
    {
        if (PointerDownCallback == null) PointerDownCallback = StartDrag;
        if (PointerUpCallback == null) PointerUpCallback = StopDrag;
    }

    private void Update()
    {
        if (_IsDragging)
        {
            (Ray ray, float distance) = RaycastBoard(Mouse.current.position.ReadValue());
            Vector3 hitPos = PosFromRayAndDistance(ray, distance);
            Vector3 currentPos = transform.position;
            transform.position = new Vector3(hitPos.x - _CursorOffset.x, hitPos.y - _CursorOffset.y, currentPos.z);
        }
    }

    public void StartDrag(PointerEventData eventData)
    {
        Debug.Assert(!_IsDragging, "StartDrag called while already dragging.");
        (Ray ray, float distance) = RaycastBoard(eventData.position);
        Vector3 hitPos = PosFromRayAndDistance(ray, distance);
        _IsDragging = true;
        DragStarted?.Invoke();
        _CursorOffset = new Vector2(hitPos.x - transform.position.x, hitPos.y - transform.position.y);
    }

    public void StopDrag(PointerEventData eventData)
    {
        _IsDragging = false;
        DragEnded?.Invoke(_CursorOffset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDownCallback?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUpCallback?.Invoke(eventData);
    }
}
