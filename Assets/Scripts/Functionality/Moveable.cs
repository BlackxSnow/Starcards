using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    static Func<float, float> DEFAULT_EASING = Utility.Easings.EaseInQuad;
    public static Func<float, float> EasingOverride = null;
    private CancellationTokenSource _MoveTokenSource = new CancellationTokenSource();
    private bool _IsMoving = false;
    public float Speed = 10.0f;

    private async Task DoMove(Func<Vector3> targetGetter, Action onFinished, Func<float, float> easingFunction)
    {
        if (_IsMoving)
        {
            _MoveTokenSource.Cancel();
            _MoveTokenSource = new CancellationTokenSource();
        }
        CancellationToken token = _MoveTokenSource.Token;
        _IsMoving = true;

        Vector3 startPosition = transform.position;
        float timeToMove = Vector3.Distance(transform.position, targetGetter()) / Speed;
        float t = 0;
        while (t < 1)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            t = Mathf.Min(1, t + Time.fixedDeltaTime / timeToMove);
            transform.position = Vector3.Lerp(startPosition, targetGetter(), easingFunction(t));
            //if (onFinished != null) Debug.Log($"{Time.frameCount}: {gameObject.name} t value: {t}");
            await Await.NextFixedUpdate();
        }

        _IsMoving = false;
        onFinished?.Invoke();
        return;
    }
    public void StopMove()
    {
        if (_IsMoving)
        {
            _MoveTokenSource.Cancel();
            _IsMoving = false;
            _MoveTokenSource = new CancellationTokenSource();
        }
    }
    public async Task MoveTo(Vector3 pos, Action onFinished = null, Func<float, float> easingFunction = null)
    {
        await DoMove(() => pos, onFinished, easingFunction ?? EasingOverride ?? DEFAULT_EASING);
    }
    public async Task MoveTo(Transform target, Action onFinished = null, Func<float, float> easingFunction = null)
    {
        await DoMove(() => target.position, onFinished, easingFunction ?? EasingOverride ?? DEFAULT_EASING);
    }
    public async Task MoveTo(Transform target, Vector3 offset, Action onFinished = null, Func<float, float> easingFunction = null)
    {
        await DoMove(() => target.position + offset, onFinished, easingFunction ?? EasingOverride ?? DEFAULT_EASING);
    }
    public async Task MoveZ(float target, Func<float, float> easingFunction = null)
    {
        await DoMove(() => new Vector3(transform.position.x, transform.position.y, target), null, easingFunction ?? EasingOverride ?? DEFAULT_EASING);
    }
}
