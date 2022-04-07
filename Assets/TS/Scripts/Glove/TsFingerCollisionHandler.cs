using System;
using System.Linq;
using TsApi;
using TsSDK;
using UnityEngine;

[RequireComponent(typeof(TsFingerTracking))]
public class TsFingerCollisionHandler : MonoBehaviour
{
    [SerializeField]
    private Collider[] m_innerColliders;
    [SerializeField]
    private Collider[] m_outerColliders;

    private int m_innerLockCount = 0;
    private int m_outerLockCount = 0;

    public TsDirection LockState 
    {
        get { return m_state; }
    }

    [NonSerialized]
    private TsDirection m_state = TsDirection.Undefined;

    public bool OnForceFeedbackEnter(GameObject what, ForceFeedbackObject feedbackObject)
    {
        if (HasInnerCollider(what))
        {
            m_innerLockCount++;
        }
        else if (HasOuterCollider(what))
        {
            m_outerLockCount++;
        }

        UpdateState();

        return m_state != TsDirection.Undefined;
    }

    public bool OnForceFeedbackExit(GameObject what, ForceFeedbackObject feedbackObject)
    {
        if (HasInnerCollider(what))
        {
            m_innerLockCount--;
        }
        else if (HasOuterCollider(what))
        {
            m_outerLockCount--;
        }

        UpdateState();
        return m_state == TsDirection.Undefined;;
    }

    private void UpdateState()
    {
        if (m_innerLockCount > 0 && m_innerLockCount >= m_outerLockCount)
        {
            m_state = TsDirection.Down;
        } 
        else if (m_outerLockCount > 0 && m_outerLockCount >= m_innerLockCount)
        {
            m_state = TsDirection.Down;
        }
        else
        {
            m_state = TsDirection.Undefined;
        }
        Debug.Log(m_state);
    }

    public bool HasInnerCollider(GameObject cond)
    {
        return m_innerColliders.Where((item) => item.gameObject == cond.gameObject).Any();
    }

    public bool HasOuterCollider(GameObject cond)
    {
        return m_outerColliders.Where((item) => item.gameObject == cond.gameObject).Any();
    }

}
