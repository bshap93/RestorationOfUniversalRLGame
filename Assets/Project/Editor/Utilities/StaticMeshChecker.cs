using UnityEditor;
using UnityEngine;

namespace Project.Editor.Utilities
{
    public class StaticMeshChecker : EditorWindow
    {
        void OnGUI()
        {
            if (GUILayout.Button("Check Mesh Static Status")) CheckMeshes();
        }
        [MenuItem("Debug/Check Static Meshes")]
        public static void ShowWindow()
        {
            GetWindow<StaticMeshChecker>("Static Mesh Checker");
        }

        static void CheckMeshes()
        {
            var allObjects = FindObjectsOfType<GameObject>();

            Debug.Log("=== Static Meshes ===");
            foreach (var obj in allObjects)
                if (obj.isStatic && obj.GetComponent<MeshRenderer>())
                    Debug.Log(obj.name + " is Static", obj);

            Debug.Log("=== Dynamic Meshes ===");
            foreach (var obj in allObjects)
                if (!obj.isStatic && obj.GetComponent<MeshRenderer>())
                    Debug.Log(obj.name + " is Dynamic", obj);
        }
    }
}
