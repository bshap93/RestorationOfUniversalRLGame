using UnityEditor;
using UnityEngine;

namespace Project.Tests
{
    public class AnimationBatchProcessor : EditorWindow
    {
        Object folder;

        void OnGUI()
        {
            GUILayout.Label("Batch Process Animation Clips", EditorStyles.boldLabel);
            folder = EditorGUILayout.ObjectField("Animation Folder", folder, typeof(Object), false);

            if (GUILayout.Button("Process Animations"))
            {
                if (folder != null)
                {
                    var folderPath = AssetDatabase.GetAssetPath(folder);
                    ProcessAnimations(folderPath);
                }
                else
                {
                    Debug.LogError("Please select a folder!");
                }
            }
        }
        [MenuItem("Tools/Animation Batch Processor")]
        public static void ShowWindow()
        {
            GetWindow<AnimationBatchProcessor>("Animation Batch Processor");
        }

        void ProcessAnimations(string folderPath)
        {
            var animationPaths = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });

            foreach (var guid in animationPaths)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                if (clip != null)
                {
                    var serializedClip = new SerializedObject(clip);
                    serializedClip.FindProperty("m_MotionNodeName").stringValue = "<None>";
                    serializedClip.FindProperty("m_AnimationClipSettings.m_RootTransformPositionXZBakeIntoPose")
                        .boolValue = true;

                    serializedClip.ApplyModifiedProperties();
                    Debug.Log($"Processed: {clip.name}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Animation processing complete!");
        }
    }
}
