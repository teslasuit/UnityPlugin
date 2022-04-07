using System.Collections;
using System.Collections.Generic;
using TsSDK;
using UnityEngine;

public class TsHandTracking : MonoBehaviour
{
    public TsGloveBehaviour gloveBehaviour;
    public GameObject visiblity;
    public IGlove Glove { get { return m_glove; } }
    private IGlove m_glove;

    public IReadOnlyDictionary<FingerIndex, TsFingerTracking> Fingers
    {
        get { return m_fingers; }
    }
    private Dictionary<FingerIndex, TsFingerTracking> m_fingers = new Dictionary<FingerIndex, TsFingerTracking>();

    // Start is called before the first frame update
    void Awake()
    {
        visiblity.SetActive(true);
        FindFingers();
        visiblity.SetActive(false);
    }

    void Start()
    {
        gloveBehaviour.ConnectionStateChanged += GloveBehaviour_ConnectionStateChanged; ;
    }


    private void FindFingers()
    {
        m_fingers.Clear();
        var fingerComponents = this.GetComponentsInChildren<TsFingerTracking>();
        foreach (var finger in fingerComponents)
        {
            m_fingers[finger.Index] = finger;
        }
    }

    private void GloveBehaviour_ConnectionStateChanged(TsDeviceBehaviour deviceBehaviour, bool isConnected)
    {
        if (isConnected)
        {
            m_glove = gloveBehaviour.Glove;
            if (!m_glove.MagneticEncoder.MagneticEncoderStarted)
            {
                m_glove.MagneticEncoder.StartMagneticEncoder();
            }
        }
        else
        {
            m_glove = null;
        }
    }

    void FixedUpdate()
    {
        if (m_glove == null)
        {
            visiblity.SetActive(false);
            return;
        }
        visiblity.SetActive(true);
        UpdateFingers(m_glove);
    }

    private void UpdateFingers(IGlove glove)
    {
        foreach (var tsFinger in glove.HandProcessor.Hand.Fingers)
        {
            var finger = m_fingers[tsFinger.Index];
            finger.UpdateData(tsFinger);
        }
    }
}
