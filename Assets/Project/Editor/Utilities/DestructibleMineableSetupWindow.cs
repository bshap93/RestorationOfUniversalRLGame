using System;
using System.IO;
using Gameplay.ItemManagement.InventoryTypes.Destructables;
using Gameplay.ItemsInteractions;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEditor;
using UnityEngine;

namespace Project.Editor.Utilities
{
    public class DestructibleMineableSetupWindow : EditorWindow
    {
        string assetSavePath = "Assets/Prefabs/Environment/Mine/";
        string customPrefabName = "";
        string customScriptableObjectName = "";
        GameObject damageFeedbackPrefab;
        bool isSceneObject;
        int mineableLayer;
        string scriptableObjectPath = "Assets/Resources/Crafting/Destructables/";
        GameObject selectedObject;

        void OnEnable()
        {
            mineableLayer = LayerMask.NameToLayer("Minable");
            if (mineableLayer == -1)
                Debug.LogWarning("'Mineable' layer not found. Please create it in your project's Layer settings.");
        }

        void OnGUI()
        {
            GUILayout.Label("Destructible Minable Setup", EditorStyles.boldLabel);

            // Track the previous selection to detect changes
            var previousSelection = selectedObject;

            // Allow scene objects to be dragged in
            selectedObject =
                EditorGUILayout.ObjectField(
                    "Rock/Minable Object", selectedObject, typeof(GameObject), true) as GameObject;

            // Check if selection changed
            if (selectedObject != previousSelection)
            {
                isSceneObject = PrefabUtility.GetPrefabAssetType(selectedObject) == PrefabAssetType.NotAPrefab;
                if (selectedObject != null)
                {
                    customPrefabName = selectedObject.name + "_Destructible";
                    customScriptableObjectName = selectedObject.name + "_Destructable";
                }
            }

            damageFeedbackPrefab = EditorGUILayout.ObjectField(
                "Damage Feedback Prefab", damageFeedbackPrefab, typeof(GameObject), false) as GameObject;

            GUILayout.Space(10);
            GUILayout.Label("Naming", EditorStyles.boldLabel);

            customPrefabName = EditorGUILayout.TextField("New Prefab Name", customPrefabName);
            customScriptableObjectName = EditorGUILayout.TextField(
                "New ScriptableObject Name", customScriptableObjectName);

            GUILayout.Space(10);
            GUILayout.Label("Save Paths", EditorStyles.boldLabel);
            assetSavePath = EditorGUILayout.TextField("Prefab Save Path", assetSavePath);
            scriptableObjectPath = EditorGUILayout.TextField("ScriptableObject Save Path", scriptableObjectPath);

            if (mineableLayer == -1)
                EditorGUILayout.HelpBox(
                    "'Mineable' layer not found. Please create it in your project's Layer settings.",
                    MessageType.Warning);

            GUI.enabled = selectedObject != null;
            if (GUILayout.Button("Setup Destructible")) SetupDestructible();
            GUI.enabled = true;
        }

        [MenuItem("Tools/Setup Destructible Minable")]
        public static void ShowWindow()
        {
            GetWindow<DestructibleMineableSetupWindow>("Destructible Setup");
        }

        void SetupDestructible()
        {
            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an object to setup.", "OK");
                return;
            }

            // Create working instance
            GameObject instance;
            if (isSceneObject)
            {
                // If it's a scene object, create a duplicate to work with
                instance = Instantiate(selectedObject);
                instance.name = selectedObject.name; // Remove "(Clone)"
            }
            else
            {
                // If it's a prefab, instantiate it
                instance = PrefabUtility.InstantiatePrefab(selectedObject) as GameObject;
            }

            Undo.RegisterCreatedObjectUndo(instance, "Create Destructible Instance");

            // Remove LOD Group and .001 child if they exist
            var lodGroup = instance.GetComponent<LODGroup>();
            if (lodGroup != null) DestroyImmediate(lodGroup);

            var extraChild = instance.transform.Find(instance.name + ".001");
            if (extraChild != null) DestroyImmediate(extraChild.gameObject);

            // Create parent object with custom name
            var parentObject = new GameObject(
                string.IsNullOrEmpty(customPrefabName) ? instance.name + "_Destructible" : customPrefabName);

            Undo.RegisterCreatedObjectUndo(parentObject, "Create Parent Object");

            // Setup parent-child relationship
            parentObject.transform.position = instance.transform.position;
            instance.transform.SetParent(parentObject.transform);

            // Set layer to Mineable
            if (mineableLayer != -1)
            {
                parentObject.layer = mineableLayer;
                instance.layer = mineableLayer;
            }

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
            destructableData.maxHealth = 30f; // Default value

            // If working with a scene object, we need to create a prefab for the original object first
            var originalPrefabPath = $"{assetSavePath}Original_{instance.name}.prefab";
            GameObject originalPrefab;

            if (isSceneObject)
            {
                // Create a temporary object to make the prefab from
                var tempObject = Instantiate(selectedObject);
                tempObject.name = selectedObject.name;
                originalPrefab = PrefabUtility.SaveAsPrefabAsset(tempObject, originalPrefabPath);
                DestroyImmediate(tempObject);
            }
            else
            {
                originalPrefab = selectedObject;
            }

            destructableData.prefabIntact = originalPrefab;

            // Ensure directories exist
            if (!Directory.Exists(scriptableObjectPath)) Directory.CreateDirectory(scriptableObjectPath);

            var scriptableObjectName = string.IsNullOrEmpty(customScriptableObjectName)
                ? $"{instance.name}_Destructable"
                : customScriptableObjectName;

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

            // If this was a scene object, optionally remove the original
            if (isSceneObject)
                if (EditorUtility.DisplayDialog(
                        "Remove Original?",
                        "Do you want to remove the original object from the scene?",
                        "Yes", "No"))
                    DestroyImmediate(selectedObject);

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
}
