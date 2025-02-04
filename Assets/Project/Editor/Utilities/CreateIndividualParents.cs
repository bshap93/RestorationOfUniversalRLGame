using UnityEditor;
using UnityEngine;

public class CreateIndividualParents : MonoBehaviour
{
    [MenuItem("Debug/NewObjects/Create Empty Parents")]
    static void CreateParents()
    {
        var selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected!");
            return;
        }

        foreach (var obj in selectedObjects)
        {
            var newParent = new GameObject(obj.name + "_Parent");
            newParent.transform.position = obj.transform.position; // Keep same position
            obj.transform.SetParent(newParent.transform);
        }

        Debug.Log("Created individual parents for selected objects.");
    }

    [MenuItem("Debug/NewObjects/Create Empty Child")]
    static void CreateChildren()
    {
        var selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected!");
            return;
        }

        foreach (var obj in selectedObjects)
        {
            // Create empty child
            var newChild = new GameObject(obj.name + "_Child");
            newChild.transform.SetParent(obj.transform);
            newChild.transform.localPosition = Vector3.zero; // Center it under the parent
        }

        Debug.Log("Created one empty child for each selected object.");
    }

    [MenuItem("Debug/NewObjects/Batch Rename as Feedbacks")]
    static void BatchRename()
    {
        var selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected for renaming!");
            return;
        }

        var baseName = "Feedbacks"; // Change this to whatever prefix you want

        for (var i = 0; i < selectedObjects.Length; i++) selectedObjects[i].name = baseName;

        Debug.Log($"Renamed {selectedObjects.Length} objects to '{baseName} X'");
    }
}
