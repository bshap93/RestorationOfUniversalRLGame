using System.IO;
using UnityEditor;
using UnityEngine;

public class FlattenInventoryItems
{
    [MenuItem("Debug/Inventory/Flatten Inventory Items")]
    public static void FlattenItems()
    {
        var resourcesPath = "Assets/Resources/Items/";

        if (!Directory.Exists(resourcesPath)) Directory.CreateDirectory(resourcesPath);

        var guids = AssetDatabase.FindAssets("t:InventoryItem");

        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var fileName = Path.GetFileName(assetPath);
            var newPath = Path.Combine(resourcesPath, fileName);

            if (assetPath != newPath)
            {
                Debug.Log($"Moving {assetPath} to {newPath}");
                AssetDatabase.MoveAsset(assetPath, newPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Flattening complete. All InventoryItem assets are now in Resources/Items/");
    }
}
