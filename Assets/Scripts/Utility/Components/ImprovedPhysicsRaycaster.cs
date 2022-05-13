using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class ImprovedPhysicsRaycaster : MonoBehaviour
{
    public static ImprovedPhysicsRaycaster Instance { get; protected set; }

    public static Collider HitObject { get; protected set; }
    public static Action<Collider> HitChanged;

    private Camera _Camera;

    private void Awake()
    {
        if (Instance != null)
        {
            throw new Exception("Instance of ImprovedPhysicsRaycaster already exists.");
        }
        Instance = this;
        _Camera = GetComponent<Camera>();
    }

    private void CheckHitChange(Collider hit)
    {
        if (HitObject != hit)
        {
            HitObject = hit;
            HitChanged?.Invoke(hit);
        }
    }

    private void PerformRaycast()
    {
        Ray ray = _Camera.ScreenPointToRay(Pointer.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CheckHitChange(hit.collider);
        }
        else
        {
            CheckHitChange(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PerformRaycast();
    }
}
