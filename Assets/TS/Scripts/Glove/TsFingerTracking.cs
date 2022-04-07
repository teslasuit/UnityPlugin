using System.Collections.Generic;
using System.Linq;
using TsSDK;
using UnityEngine;

public class TsFingerTracking : MonoBehaviour
{
    public FingerIndex Index
    {
        get { return m_fingerIndex; }
    }
    [SerializeField]
    private FingerIndex m_fingerIndex;
    [SerializeField]
    private Transform m_proximal;
    [SerializeField]
    private Transform m_intermediate;
    [SerializeField]
    private Transform m_distal;

    [SerializeField]
    private float m_flexionAngle = -90;
    [SerializeField]
    private float m_abductionOffset = -30;

    public float currentAbduction = 0.0f;

    public bool invertAbduction = false;

    private Vector3 m_initialProximal;
    private Vector3 m_initialIntermediate;
    private Vector3 m_initialDistal;

    public IReadOnlyDictionary<PhalanxIndex, Transform> Phalanxes
    {
        get { return m_phalanxes; }
    }

    private Dictionary<PhalanxIndex, Transform> m_phalanxes = new Dictionary<PhalanxIndex, Transform>();
    private Dictionary<PhalanxIndex, Vector3> m_initial = new Dictionary<PhalanxIndex, Vector3>();


    private void Awake()
    {
        InitializePhalanxes();
    }

    private void InitializePhalanxes()
    {
        m_phalanxes[PhalanxIndex.Distal] = m_distal;
        m_phalanxes[PhalanxIndex.Intermediate] = m_intermediate;
        m_phalanxes[PhalanxIndex.Proximal] = m_proximal;

        foreach (var phalanxKV in m_phalanxes)
        {
            m_initial[phalanxKV.Key] = phalanxKV.Value.localRotation.eulerAngles;
        }
    }

    public void SetFlexion(float flexion)
    {
        var degrees = flexion * m_flexionAngle;
        SetFlexionDegrees(degrees);
    }

    private void SetFlexionDegrees(float degrees)
    {
        SetFlexionDegrees(PhalanxIndex.Distal, degrees);
        SetFlexionDegrees(PhalanxIndex.Intermediate, degrees);
        SetFlexionDegrees(PhalanxIndex.Proximal, degrees);
    }

    private void SetFlexionDegrees(PhalanxIndex index, float degrees)
    {
        var phalanx = m_phalanxes[index];
        var ea = phalanx.localRotation.eulerAngles;
        var initial = m_initial[index];
        ea.z = initial.z + degrees;

        phalanx.localRotation = Quaternion.Euler(ea);
    }

    public void SetAbduction(float abduction)
    {
        var degrees = abduction * 15 / 2;
        SetAbductionDegrees(degrees);
    }

    private void SetAbductionDegrees(float degrees)
    {
        var eaP = m_proximal.localRotation.eulerAngles;
        eaP.y = m_initialProximal.y + degrees;
        m_proximal.localRotation = Quaternion.Euler(eaP);
    }

    public void UpdateData(Finger finger)
    {
        //finger.AbductionMax = m_abductionAngle;
        foreach (var phalanx in finger.Phalanxes)
        {
            phalanx.FlexionMax = m_flexionAngle;
            SetFlexionDegrees(phalanx.Index, phalanx.FlexionAngle);
        }

        var invertion = invertAbduction ? -1 : 1;
        SetAbductionDegrees(invertion * finger.AbductionAngle + m_abductionOffset);
    }

    public PhalanxIndex GetPhalanxIndex(Transform comp)
    {
        var phalanxKV = m_phalanxes.Where((item) => item.Value == comp);
        if (phalanxKV.Any())
        {
            return phalanxKV.First().Key;
        }

        return PhalanxIndex.Undefined;
    }
}
