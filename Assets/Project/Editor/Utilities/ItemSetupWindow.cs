using Gameplay.Player.Inventory;
using Gameplay.SaveLoad.Triggers;
using MoreMountains.Feedbacks;
using UnityEditor;
using UnityEngine;

public class ItemSetupWindow : EditorWindow
{
    GameObject pickedFeedbackPrefab;
    GameObject selectedObject;
    GameObject selectionFeedbackPrefab;

    void OnGUI()
    {
        GUILayout.Label("Pickable Item Setup", EditorStyles.boldLabel);

        selectedObject =
            EditorGUILayout.ObjectField("Target Object", selectedObject, typeof(GameObject), true) as GameObject;

        pickedFeedbackPrefab = EditorGUILayout.ObjectField(
            "Picked Feedback Prefab", pickedFeedbackPrefab, typeof(GameObject), false) as GameObject;

        selectionFeedbackPrefab = EditorGUILayout.ObjectField(
            "Selection Feedback Prefab", selectionFeedbackPrefab, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Setup Item")) SetupItem();
    }

    [MenuItem("Tools/Setup Pickable Item")]
    public static void ShowWindow()
    {
        GetWindow<ItemSetupWindow>("Item Setup");
    }

    void SetupItem()
    {
        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select an object to setup.", "OK");
            return;
        }

        // Validate the selected object has required components
        if (!selectedObject.GetComponent<MeshRenderer>() || !selectedObject.GetComponent<MeshCollider>())
        {
            EditorUtility.DisplayDialog(
                "Error", "Selected object must have both MeshRenderer and MeshCollider components.", "OK");

            return;
        }

        // Create parent object
        var parentObject = new GameObject(selectedObject.name + "_Parent");
        Undo.RegisterCreatedObjectUndo(parentObject, "Create Parent Object");

        // Set parent's position to match selected object
        parentObject.transform.position = selectedObject.transform.position;
        parentObject.transform.rotation = selectedObject.transform.rotation;

        // Parent the selected object to the new parent
        Undo.SetTransformParent(selectedObject.transform, parentObject.transform, "Parent Object");

        // Add components to parent
        var rb = Undo.AddComponent<Rigidbody>(parentObject);
        rb.isKinematic = true;
        rb.useGravity = false;

        var trigger = Undo.AddComponent<SphereCollider>(parentObject);
        trigger.isTrigger = true;
        trigger.radius = 2f;

        var itemTrigger = Undo.AddComponent<ItemSelectableTrigger>(parentObject);
        var itemPicker = Undo.AddComponent<ManualItemPicker>(parentObject);

        // Instantiate feedback prefabs if provided
        if (pickedFeedbackPrefab != null)
        {
            var pickedFeedback =
                PrefabUtility.InstantiatePrefab(pickedFeedbackPrefab, parentObject.transform) as GameObject;

            Undo.RegisterCreatedObjectUndo(pickedFeedback, "Create Picked Feedback");

            // Set the picked feedback reference
            var mmFeedbacks = pickedFeedback.GetComponent<MMFeedbacks>();
            if (mmFeedbacks != null) itemPicker.pickedMmFeedbacks = mmFeedbacks;
        }

        if (selectionFeedbackPrefab != null)
        {
            var selectionFeedback =
                PrefabUtility.InstantiatePrefab(selectionFeedbackPrefab, parentObject.transform) as GameObject;

            Undo.RegisterCreatedObjectUndo(selectionFeedback, "Create Selection Feedback");

            // Set the selection feedback reference
            var mmFeedbacks = selectionFeedback.GetComponent<MMFeedbacks>();
            if (mmFeedbacks != null)
            {
                var serializedItemTrigger = new SerializedObject(itemTrigger);
                var feedbacksProp = serializedItemTrigger.FindProperty("_selectionFeedbacks");
                feedbacksProp.objectReferenceValue = mmFeedbacks;
                serializedItemTrigger.ApplyModifiedProperties();
            }
        }

        // Select the newly created parent object
        Selection.activeGameObject = parentObject;

        EditorUtility.DisplayDialog("Success", "Item setup completed successfully!", "OK");
    }
}
