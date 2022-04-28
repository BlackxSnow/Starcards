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

    public Action OnStartDrag;
    public Action OnEndDrag;

    private Vector2 _CursorOffset;

    public IDirectable Directable;

    private bool _IsDragging = false;

    private Vector2 _LastMousePos;

    private (Ray ray, float distance) RaycastBoard(Vector3 origin)
    {
        Ray ray = Camera.main.ScreenPointToRay(origin);
        bool isIntersecting = Board.Raycast(ray, out float distance);

        if (!isIntersecting)
        {
            throw new System.Exception("No interesection between board and cursor.");
        }
        return (ray, distance);
    }

    private Vector3 PosFromRayAndDistance(Ray ray, float distance)
    {
        return Camera.main.transform.position + (ray.direction * distance);
    }

    private void MoveTo(Vector3 pos)
    {
        if (Directable != null)
        {
            Directable.TargetPosition = pos;
        }
        else
        {
            transform.position = pos;
        }
    }

    private Vector3 GetCurrentPos()
    {
        if (Directable != null)
        {
            return Directable.TargetPosition;
        }
        else
        {
            return transform.position;
        }
    }

    private void Update()
    {
        if (_IsDragging)
        {
            (Ray ray, float distance) = RaycastBoard(Mouse.current.position.ReadValue());
            Vector3 hitPos = PosFromRayAndDistance(ray, distance);
            Vector3 currentPos = GetCurrentPos();
            MoveTo(new Vector3(hitPos.x - _CursorOffset.x, hitPos.y - _CursorOffset.y, currentPos.z));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        (Ray ray, float distance) = RaycastBoard(eventData.position);
        Vector3 hitPos = PosFromRayAndDistance(ray, distance);
        _IsDragging = true;
        OnStartDrag?.Invoke();
        _CursorOffset = new Vector2(hitPos.x - transform.position.x, hitPos.y - transform.position.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _IsDragging = false;
        OnEndDrag?.Invoke();
    }
}
