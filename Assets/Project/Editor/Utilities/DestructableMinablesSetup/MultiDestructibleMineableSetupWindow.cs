using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gameplay.ItemManagement.InventoryTypes.Destructables;
using Gameplay.ItemsInteractions;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Project.Editor.Utilities.DestructableMinablesSetup
{
    public class MultiDestructibleMineableSetupWindow : EditorWindow
    {
        readonly List<GameObject> selectedObjects = new();
        string assetSavePath = "Assets/Prefabs/Destructibles/";
        GameObject damageFeedbackPrefab;
        int mineableLayer;
        ReorderableList objectList;
        bool processCurrentSelection;
        string scriptableObjectPath = "Assets/ScriptableObjects/Destructibles/";
        Vector2 scrollPosition;
        string suffixForPrefabs = "_Destructible";
        string suffixForScriptableObjects = "_Destructable";

        void OnEnable()
        {
            mineableLayer = LayerMask.NameToLayer("Mineable");
            InitializeReorderableList();
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Destructible Minables Batch Setup", EditorStyles.boldLabel);

            // Selection processing toggle
            processCurrentSelection = EditorGUILayout.Toggle("Process Current Selection", processCurrentSelection);
            if (processCurrentSelection)
                EditorGUILayout.HelpBox("Will process all currently selected objects in the scene.", MessageType.Info);

            GUILayout.Space(10);

            // Manual object list
            if (!processCurrentSelection) objectList.DoLayoutList();

            GUILayout.Space(10);

            // Common settings
            GUILayout.Label("Common Settings", EditorStyles.boldLabel);
            damageFeedbackPrefab = EditorGUILayout.ObjectField(
                "Damage Feedback Prefab", damageFeedbackPrefab, typeof(GameObject), false) as GameObject;

            suffixForPrefabs = EditorGUILayout.TextField("Prefab Name Suffix", suffixForPrefabs);
            suffixForScriptableObjects = EditorGUILayout.TextField(
                "ScriptableObject Name Suffix", suffixForScriptableObjects);

            GUILayout.Space(5);

            assetSavePath = EditorGUILayout.TextField("Prefab Save Path", assetSavePath);
            scriptableObjectPath = EditorGUILayout.TextField("ScriptableObject Save Path", scriptableObjectPath);

            if (mineableLayer == -1)
                EditorGUILayout.HelpBox(
                    "'Mineable' layer not found. Please create it in your project's Layer settings.",
                    MessageType.Warning);

            GUILayout.Space(10);

            // Process button
            GUI.enabled = (processCurrentSelection && Selection.gameObjects.Length > 0) ||
                          (!processCurrentSelection && selectedObjects.Any(x => x != null));

            if (GUILayout.Button("Process Objects")) ProcessObjects();
            GUI.enabled = true;

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Tools/Setup Multiple Destructible Minables")]
        public static void ShowWindow()
        {
            GetWindow<MultiDestructibleMineableSetupWindow>("Destructibles Setup");
        }

        void InitializeReorderableList()
        {
            objectList = new ReorderableList(selectedObjects, typeof(GameObject), true, true, true, true);

            objectList.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Objects to Convert"); };

            objectList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                selectedObjects[index] = (GameObject)EditorGUI.ObjectField(
                    rect, selectedObjects[index], typeof(GameObject), true);
            };

            objectList.onAddCallback = list => { selectedObjects.Add(null); };
        }

        void ProcessObjects()
        {
            var objectsToProcess = new List<GameObject>();

            if (processCurrentSelection)
                objectsToProcess.AddRange(Selection.gameObjects);
            else
                objectsToProcess.AddRange(selectedObjects.Where(x => x != null));

            if (objectsToProcess.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No objects selected to process.", "OK");
                return;
            }

            var successCount = 0;
            var errors = "";

            AssetDatabase.StartAssetEditing();

            try
            {
                foreach (var obj in objectsToProcess)
                    try
                    {
                        SetupDestructible(obj);
                        successCount++;
                    }
                    catch (Exception e)
                    {
                        errors += $"\n{obj.name}: {e.Message}";
                    }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            var message = $"Processed {successCount} objects successfully.";
            if (!string.IsNullOrEmpty(errors)) message += $"\n\nErrors occurred:{errors}";

            EditorUtility.DisplayDialog("Process Complete", message, "OK");
        }

        void SetupDestructible(GameObject selectedObject)
        {
            if (selectedObject == null) return;

            var isSceneObject = PrefabUtility.GetPrefabAssetType(selectedObject) == PrefabAssetType.NotAPrefab;

            // Create working instance
            GameObject instance;
            if (isSceneObject)
            {
                instance = Instantiate(selectedObject);
                instance.name = selectedObject.name; // Remove "(Clone)"
            }
            else
            {
                instance = PrefabUtility.InstantiatePrefab(selectedObject) as GameObject;
            }

            // Process the object
            ProcessSingleObject(instance, selectedObject, isSceneObject);

            // If this was a scene object, optionally remove the original
            if (isSceneObject) DestroyImmediate(selectedObject);
        }

        void ProcessSingleObject(GameObject instance, GameObject originalObject, bool isSceneObject)
        {
            // Remove LOD Group and .001 child if they exist
            var lodGroup = instance.GetComponent<LODGroup>();
            if (lodGroup != null) DestroyImmediate(lodGroup);

            var extraChild = instance.transform.Find(instance.name + ".001");
            if (extraChild != null) DestroyImmediate(extraChild.gameObject);

            // Create parent object
            var parentName = instance.name + suffixForPrefabs;
            var parentObject = new GameObject(parentName);

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
            destructableData.maxHealth = 30f;

            // If working with a scene object, create a prefab for the original object
            var originalPrefabPath = $"{assetSavePath}Original_{instance.name}.prefab";
            GameObject originalPrefab;

            if (isSceneObject)
            {
                var tempObject = Instantiate(originalObject);
                tempObject.name = originalObject.name;
                EnsureDirectoryExists(assetSavePath);
                originalPrefab = PrefabUtility.SaveAsPrefabAsset(tempObject, originalPrefabPath);
                DestroyImmediate(tempObject);
            }
            else
            {
                originalPrefab = originalObject;
            }


            // Save ScriptableObject
            EnsureDirectoryExists(scriptableObjectPath);
            var scriptableObjectName = instance.name + suffixForScriptableObjects;
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

            // Disable original child
            instance.SetActive(false);

            // Center everything at 0,0,0
            CenterAtOrigin(parentObject);

            // Save the final prefab
            var prefabPath = $"{assetSavePath}{parentObject.name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(parentObject, prefabPath);

            // Cleanup
            DestroyImmediate(parentObject);
        }

        void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        void CenterAtOrigin(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0) return;

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);

            var centerOffset = -bounds.center;

            obj.transform.position += centerOffset;
            foreach (Transform child in obj.transform) child.position -= centerOffset;
        }
    }
}
