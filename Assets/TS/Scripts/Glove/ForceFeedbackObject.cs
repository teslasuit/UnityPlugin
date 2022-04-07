using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFeedbackObject : MonoBehaviour
{
    public LayerMask TargetLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        var handler = GetHandler(collision);
        if(handler)
        {
            handler.OnForceFeedbackEnter(collision.collider.gameObject, this);
            Debug.Log($"Enter: {collision.collider.name} : [{ collision.contacts[0].normal}]");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var handler = GetHandler(collision);
        if(handler)
        {
            handler.OnForceFeedbackExit(collision.collider.gameObject, this);
            Debug.Log($"Exit: {collision.collider.name}]");
        }
    }

    private HandCollisionHandler GetHandler(Collision collision)
    {
        var withObj = collision.collider.gameObject;
        var targetVal = TargetLayerMask.value;
        var collisionLayer = withObj.layer;

        if ((targetVal & 1 << collisionLayer) == 1 << collisionLayer)
        {
            var handler = withObj.transform.root.GetComponentInChildren<HandCollisionHandler>();
            if (handler)
            {
                return handler;
            }
        }

        return null;
    }
}
