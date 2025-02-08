﻿using System;
using System.IO;
using Gameplay.ItemManagement.InventoryTypes.Destructables;
using Gameplay.ItemsInteractions;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEditor;
using UnityEngine;

public class DestructibleMineableSetupWindow : EditorWindow
{
    string assetSavePath = "Assets/Prefabs/Destructibles/";
    GameObject damageFeedbackPrefab;
    string scriptableObjectPath = "Assets/ScriptableObjects/Destructibles/";
    GameObject selectedPrefab;

    void OnGUI()
    {
        GUILayout.Label("Destructible Minable Setup", EditorStyles.boldLabel);

        selectedPrefab =
            EditorGUILayout.ObjectField("Rock/Minable Prefab", selectedPrefab, typeof(GameObject), false) as GameObject;

        damageFeedbackPrefab = EditorGUILayout.ObjectField(
            "Damage Feedback Prefab", damageFeedbackPrefab, typeof(GameObject), false) as GameObject;

        assetSavePath = EditorGUILayout.TextField("Prefab Save Path", assetSavePath);
        scriptableObjectPath = EditorGUILayout.TextField("ScriptableObject Save Path", scriptableObjectPath);

        if (GUILayout.Button("Setup Destructible")) SetupDestructible();
    }

    [MenuItem("Tools/Setup Destructible Minable")]
    public static void ShowWindow()
    {
        GetWindow<DestructibleMineableSetupWindow>("Destructible Setup");
    }

    void SetupDestructible()
    {
        if (selectedPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a prefab to setup.", "OK");
            return;
        }

        // Create working instance
        var instance = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;
        Undo.RegisterCreatedObjectUndo(instance, "Create Destructible Instance");

        // Remove LOD Group and .001 child if they exist
        var lodGroup = instance.GetComponent<LODGroup>();
        if (lodGroup != null) DestroyImmediate(lodGroup);

        var extraChild = instance.transform.Find(instance.name + ".001");
        if (extraChild != null) DestroyImmediate(extraChild.gameObject);

        // Create parent object
        var parentObject = new GameObject(instance.name + "_Destructible");
        Undo.RegisterCreatedObjectUndo(parentObject, "Create Parent Object");

        // Setup parent-child relationship
        parentObject.transform.position = instance.transform.position;
        instance.transform.SetParent(parentObject.transform);

        // Copy MeshCollider to parent
        var originalCollider = instance.GetComponent<MeshCollider>();
        if (originalCollider != null)
        {
            var parentCollider = parentObject.AddComponent<MeshCollider>();
            EditorUtility.CopySerializedIfDifferent(originalCollider, parentCollider);
        }

        // Add DestructableMineable component
        var destructable = parentObject.AddComponent<DestructableMineable>();
        destructable.UniqueID = Guid.NewGuid().ToString();

        // Create and setup Destructable ScriptableObject
        var destructableData = CreateInstance<Destructable>();
        destructableData.prefabIntact = selectedPrefab;
        destructableData.maxHealth = 30f; // Default value, adjust as needed

        // Ensure directory exists
        if (!Directory.Exists(scriptableObjectPath)) Directory.CreateDirectory(scriptableObjectPath);

        var scriptableObjectName = $"{instance.name}_Destructable";
        AssetDatabase.CreateAsset(destructableData, $"{scriptableObjectPath}{scriptableObjectName}.asset");
        destructable.destructable = destructableData;

        // Add Health component
        var health = parentObject.AddComponent<Health>();
        health.InitialHealth = destructableData.maxHealth;
        health.MaximumHealth = destructableData.maxHealth;

        // Setup damage feedback
        if (damageFeedbackPrefab != null)
        {
            var feedbackInstance =
                PrefabUtility.InstantiatePrefab(damageFeedbackPrefab, parentObject.transform) as GameObject;

            var feedbacks = feedbackInstance.GetComponent<MMFeedbacks>();
            if (feedbacks != null) health.DamageMMFeedbacks = feedbacks;
        }

        // Disable original child (it will spawn at runtime)
        instance.SetActive(false);

        // Center everything at 0,0,0
        CenterAtOrigin(parentObject);

        // Create prefabs
        if (!Directory.Exists(assetSavePath)) Directory.CreateDirectory(assetSavePath);

        // Save the prefab
        var prefabPath = $"{assetSavePath}{parentObject.name}.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(parentObject, prefabPath);

        // Cleanup
        DestroyImmediate(parentObject);

        EditorUtility.DisplayDialog(
            "Success",
            "Destructible setup completed successfully!\n" +
            $"Prefab saved at: {prefabPath}\n" +
            $"ScriptableObject saved at: {scriptableObjectPath}{scriptableObjectName}.asset",
            "OK");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void CenterAtOrigin(GameObject obj)
    {
        // Find the bounds of all mesh renderers
        var renderers = obj.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return;

        var bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);

        // Calculate offset to center
        var centerOffset = -bounds.center;

        // Apply offset to parent and adjust child positions
        obj.transform.position += centerOffset;
        foreach (Transform child in obj.transform) child.position -= centerOffset;
    }
}
