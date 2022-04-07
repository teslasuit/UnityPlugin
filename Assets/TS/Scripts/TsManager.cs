using TsSDK;
using UnityEditor;
using UnityEngine;

public class TsManager : MonoBehaviour
{
    public static TsManager Instance { get; private set; }

    public static TsRoot Root
    {
        get { return Instance.m_root; }
    }

    private TsRoot m_root;

    private void Awake()
    {
        InitTsManager();
    }

    private void InitTsManager()
    {
        // Only allow one instance at runtime.
        if (Instance != null)
        {
            enabled = false;
            DestroyImmediate(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
        InitializeRoot();
    }

    private void InitializeRoot()
    {
#if UNITY_EDITOR
        AssemblyReloadEvents.beforeAssemblyReload += BeforeAssemblyReload;
#endif
        m_root = new TsRoot();
        Debug.Log("[TS] Initialized");
    }

    private void OnDestroy()
    {
        Destroy();
    }

    private void Destroy()
    {
        if (m_root != null)
        {
            Debug.Log("[TS] Destroyed");
            m_root.Dispose();
            m_root = null;
        }
        
#if UNITY_EDITOR
        AssemblyReloadEvents.beforeAssemblyReload -= BeforeAssemblyReload;
#endif
    }

#if UNITY_EDITOR
    private void BeforeAssemblyReload()
    {
        Destroy();
    }
#endif
}
