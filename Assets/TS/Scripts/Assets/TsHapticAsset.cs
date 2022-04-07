using UnityEngine;

public class TsHapticAsset : ScriptableObject
{
    public byte[] Bytes { get => m_bytes; }
    [SerializeField]
    private byte[] m_bytes = null;

    public static TsHapticAsset Create(byte[] bytes)
    {
        var asset = CreateInstance<TsHapticAsset>();
        asset.m_bytes = bytes;
        return asset;
    }
}
