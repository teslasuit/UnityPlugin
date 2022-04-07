using System.Collections.Generic;
using TsApi;
using TsSDK;
using UnityEngine;

[RequireComponent(typeof(TsHandTracking))]
public class HandCollisionHandler : MonoBehaviour
{
    private TsHandTracking tsHand;

    private Dictionary<FingerIndex, TsFingerCollisionHandler> m_fingerCollisionHandlers =
        new Dictionary<FingerIndex, TsFingerCollisionHandler>();

    void Start()
    {
        this.tsHand = GetComponent<TsHandTracking>();
        InitializeFingers();
    }

    private void InitializeFingers()
    {
        m_fingerCollisionHandlers.Clear();
        foreach (var fingerKv in tsHand.Fingers)
        {
            var handler = fingerKv.Value.GetComponent<TsFingerCollisionHandler>();
            m_fingerCollisionHandlers[fingerKv.Key] = handler;
        }
    }

    public void OnForceFeedbackEnter(GameObject what, ForceFeedbackObject feedbackObject)
    {
        var index = GetFingerIndex(what);
        if (m_fingerCollisionHandlers.TryGetValue(index, out var handler))
        {
            if (handler.LockState != TsDirection.Undefined)
            {
                return;
            }
            if (handler.OnForceFeedbackEnter(what, feedbackObject))
            {
                if(handler.LockState != TsDirection.Undefined)
                tsHand.Glove?.HandProcessor.LockFinger(index, handler.LockState);
            }
        }
    }

    public void OnForceFeedbackExit(GameObject what, ForceFeedbackObject feedbackObject)
    {
        var index = GetFingerIndex(what);
        if (m_fingerCollisionHandlers.TryGetValue(index, out var handler))
        {
            if (handler.OnForceFeedbackExit(what, feedbackObject))
            {
                tsHand.Glove?.HandProcessor.UnlockFinger(index);
            }
        }
    }

    private FingerIndex GetFingerIndex(GameObject obj)
    {
        FingerIndex index = FingerIndex.Undefined;
        foreach (var fingersKV in m_fingerCollisionHandlers)
        {
            if (fingersKV.Value.HasInnerCollider(obj))
            {
                return fingersKV.Key;
            }

            if (fingersKV.Value.HasOuterCollider(obj))
            {
                return fingersKV.Key;
            }
        }

        return index;
    }
}
