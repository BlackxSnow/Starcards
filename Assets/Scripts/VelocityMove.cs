using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMove : MonoBehaviour, IDirectable
{
    /// <summary>
    /// Distance within which to snap the transform and zero velocity.
    /// </summary>
    public const float SnapDistance = 0.1f;
    public const float SqrSnapDistance = SnapDistance * SnapDistance;

    /// <summary>
    /// Multiplier on Suvat formula output - multiplies the calculated max velocity.
    /// </summary>
    public float SpeedMultiplier = 0.98f;

    /// <summary>
    /// Acceleration along the x,y plane.
    /// </summary>
    public float PlanarAcceleration = 500.0f;
    /// <summary>
    /// Acceleration along the z axis.
    /// </summary>
    public float VerticalAcceleration = 15.0f;
    
    public bool IsStacking = false;

    private bool IsPlanarFinished = false;
    private bool IsVerticalFinished = false;

    /// <summary>
    /// Acceleration while stacking onto another card.
    /// </summary>
    public float PlanarStackingAcceleration = 15.0f;

    
    
    public Vector2 PlanarVelocity { get; protected set; } = Vector2.zero;
    public float VerticalVelocity { get; protected set; } = 0;

    private Vector3 _TargetPosition;
    public Vector3 TargetPosition
    {
        get => _TargetPosition;
        set
        {
            Debug.Assert(!Utility.Vector.IsNaN(value), "Target position cannot be NaN.");
            _TargetPosition = value;
        }
    }

    private void Awake()
    {
        TargetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    /// <summary>
    /// Resets the 'finished' states, allowing the card to move again.
    /// </summary>
    public void ResetFinished()
    {
        IsPlanarFinished = false;
        IsVerticalFinished = false;
    }

    /// <summary>
    /// Calculate and set the planar velocity for the object based on the target position.
    /// </summary>
    /// <param name="displacement"></param>
    private void CalculatePlanarVelocity(Vector3 displacement)
    {
        if (IsPlanarFinished) return;
        
        Vector2 planarDisplacement = new Vector2(displacement.x, displacement.y);
        if (planarDisplacement.sqrMagnitude > SqrSnapDistance)
        {
            // General form of Suvat equation: https://en.wikipedia.org/wiki/Equations_of_motion#Constant_translational_acceleration_in_a_straight_line
            float actualAcceleration = PlanarAcceleration;
            if (IsStacking && Vector2.Dot(planarDisplacement, PlanarVelocity) >= 0)
            {
                actualAcceleration = PlanarStackingAcceleration;
            }

            Vector2 targetPlanarVelocity = planarDisplacement.normalized * Mathf.Sqrt(2 * actualAcceleration * planarDisplacement.magnitude) * SpeedMultiplier;
            PlanarVelocity = Vector2.MoveTowards(PlanarVelocity, targetPlanarVelocity, actualAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            transform.position = new Vector3(TargetPosition.x, TargetPosition.y, transform.position.z);
            PlanarVelocity = Vector2.zero;
            if (IsStacking)
            {
                IsPlanarFinished = true;
            }
        }
    }

    /// <summary>
    /// Calculate and set the vertical velocity for the object based on the target position.
    /// </summary>
    /// <param name="displacement"></param>
    private void CalculateVerticalVelocity(Vector3 displacement)
    {
        if (IsVerticalFinished) return;

        if (Mathf.Abs(displacement.z) > SnapDistance)
        {
            float targetVerticalVelocity = Mathf.Sqrt(2 * VerticalAcceleration * Mathf.Abs(displacement.z)) * Mathf.Sign(displacement.z) * SpeedMultiplier;
            VerticalVelocity = Mathf.MoveTowards(VerticalVelocity, targetVerticalVelocity, VerticalAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, TargetPosition.z);
            VerticalVelocity = 0;
            if (IsStacking)
            {
                IsVerticalFinished = true;
            }
        }
    }

    private void MoveTowardsTarget()
    {
        if (TargetPosition != transform.position)
        {
            Vector3 displacement = TargetPosition - transform.position;

            CalculatePlanarVelocity(displacement);
            CalculateVerticalVelocity(displacement);

            transform.Translate(new Vector3(PlanarVelocity.x, PlanarVelocity.y, VerticalVelocity) * Time.fixedDeltaTime);
        }
    }
}
