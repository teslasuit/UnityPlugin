using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "ts_asset")]
public class TsAssetImporter : ScriptedImporter
{

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var asset = TsHapticAsset.Create(File.ReadAllBytes(ctx.assetPath));
        ctx.AddObjectToAsset("main obj", asset);
        ctx.SetMainObject(asset);

        
    }
}
