using System.IO;
using MoreMountains.InventoryEngine;
using UnityEditor;
using UnityEngine;

// Ensure this is the correct namespace for your InventoryItem class

public class CheckInventoryItemIDs
{
    [MenuItem("Debug/Inventory/Check Inventory Item IDs")]
    public static void CheckItemIDs()
    {
        var guids = AssetDatabase.FindAssets("t:InventoryItem");
        var mismatchFound = false;

        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<InventoryItem>(assetPath);

            if (item != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(assetPath); // Get the file name without .asset
                if (item.ItemID != fileName)
                {
                    Debug.LogWarning(
                        $"ItemID Mismatch: File '{fileName}' has ItemID '{item.ItemID}'.\nFix it in the Inspector.");

                    mismatchFound = true;
                }
            }
        }

        if (!mismatchFound) Debug.Log("All InventoryItems have matching file names and ItemIDs!");
    }
}
