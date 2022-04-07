using TsSDK;
using UnityEngine;

public class TsHapticMaterialObject : MonoBehaviour
{
    [SerializeField]
    private TsHapticAsset m_asset;

    private IAsset m_assetInstance = null;

    private void Start()
    {
        m_assetInstance = TsManager.Root.AssetManager.Create(m_asset.Bytes);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collisionHandler = collision.gameObject.GetComponent<TsHapticCollisionHandler>();

        if(collisionHandler != null)
        {
            collisionHandler.HapticPlayer.Play(m_assetInstance);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var collisionHandler = collision.gameObject.GetComponent<TsHapticCollisionHandler>();

        if (collisionHandler != null)
        {
            collisionHandler.HapticPlayer.Stop(m_assetInstance);
        }
    }

}
