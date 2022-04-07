using TsSDK;
using UnityEngine;

public class TsGloveBehaviour : TsDeviceBehaviour
{
    public GloveIndex TargetGloveIndex { get { return m_gloveIndex; } }
    public GloveSide TargetGloveSide { get { return m_gloveSide; } }

    public IGlove Glove { get { return (IGlove)Device; } }

    [SerializeField]
    private GloveIndex m_gloveIndex = GloveIndex.Glove0;
    [SerializeField] 
    private GloveSide m_gloveSide = GloveSide.Right;

    void Start()
    {
        var gloveManager = TsManager.Root.GloveManager;
        gloveManager.OnGloveConnected += OnGloveConnected; ;
        gloveManager.OnGloveDisconnected += OnGloveDisconnected;

        switch (m_gloveSide)
        {
            case GloveSide.Left:
            {
                foreach (var glove in gloveManager.LeftGloves)
                {
                    OnGloveConnected(glove);
                }
                break;
            }
            case GloveSide.Right:
            {
                foreach (var glove in gloveManager.RightGloves)
                {
                    OnGloveConnected(glove);
                }
                break;
            }
        }
    }

    private void OnGloveConnected(IGlove obj)
    {
        if(obj.Index == TargetGloveIndex && obj.Side == m_gloveSide)
        {
            UpdateState(obj, true);
        }
    }

    private void OnGloveDisconnected(IGlove obj)
    {
        if(obj.Index == TargetGloveIndex && obj.Side == m_gloveSide)
        {
            UpdateState(obj, false);
        }
    }
}
