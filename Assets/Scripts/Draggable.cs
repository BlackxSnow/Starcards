using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static readonly Plane Board = new Plane(Vector3.back, 0);

    public Action OnStartDrag;
    public Action OnEndDrag;

    private Vector2 _CursorOffset;

    private (Ray ray, float distance) RaycastBoard(PointerEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(data.position);
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

    public void OnDrag(PointerEventData eventData)
    {
        (Ray ray, float distance) = RaycastBoard(eventData);
        Vector3 hitPos = PosFromRayAndDistance(ray, distance);
        transform.position = new Vector3(hitPos.x - _CursorOffset.x, hitPos.y - _CursorOffset.y, transform.position.z);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        (Ray ray, float distance) = RaycastBoard(eventData);
        Vector3 hitPos = PosFromRayAndDistance(ray, distance);
        OnStartDrag?.Invoke();
        _CursorOffset = new Vector2(hitPos.x - transform.position.x, hitPos.y - transform.position.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnEndDrag?.Invoke();
    }
}
