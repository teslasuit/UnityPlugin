
using TsSDK;
using UnityEngine;
using UnityEngine.UI;

public class TsHapticAnimationPlayback : MonoBehaviour
{
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_pauseButton;
    [SerializeField] private Button m_stopButton;
    [SerializeField] private Slider m_progressSlider;


    [SerializeField]
    private TsHapticPlayer m_hapticPlayer;

    [SerializeField]
    private TsHapticAsset m_hapticAsset;

    private IAsset m_assetInstance = null;

    private void Start()
    {
        m_assetInstance = LoadAsset();

        m_playButton.onClick.AddListener(Play);
        m_pauseButton.onClick.AddListener(Pause);
        m_stopButton.onClick.AddListener(Stop);
    }

    private void Update()
    {
        if (m_assetInstance != null)
        {
            if (m_hapticPlayer.IsPlaying(m_assetInstance))
            {
                var duration = GetDuration();
                var time = GetTime();

                var progress = ((float) time) / duration;
                m_progressSlider.value = progress;
            }
        }
    }

    public void Play()
    {
        m_hapticPlayer.Play(m_assetInstance);
    }

    public void Stop()
    {
        m_hapticPlayer.Stop(m_assetInstance);
    }

    public void Pause()
    {
        m_hapticPlayer.SetPaused(m_assetInstance, true);
    }

    public ulong GetTime()
    {
        return m_hapticPlayer.GetTime(m_assetInstance);
    }

    public ulong GetDuration()
    {
        return m_hapticPlayer.GetDuration(m_assetInstance);
    }

    private IAsset LoadAsset()
    {
        if (m_assetInstance != null)
        {
            return m_assetInstance;
        }

        m_assetInstance = TsManager.Root.AssetManager.Create(m_hapticAsset.Bytes);
        return m_assetInstance;
    }

    private void OnDestroy()
    {
        Unload();
    }

    private void Unload()
    {
        if (m_assetInstance == null)
        {
            return;
        }
        TsManager.Root.AssetManager.Destroy(m_assetInstance);
    }
}
