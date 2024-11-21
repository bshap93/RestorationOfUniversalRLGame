using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor.Utilities
{
    public class PrefabSummary : UnityEditor.Editor
    {
        [MenuItem("Tools/Generate Prefab Summary")]
        public static void GeneratePrefabSummary()
        {
            var prefab = Selection.activeObject as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning("Select a prefab first!");
                return;
            }

            var summary = new StringBuilder();
            foreach (var child in prefab.GetComponentsInChildren<Transform>(true))
            {
                summary.AppendLine("GameObject: " + child.name);
                foreach (var component in child.GetComponents<Component>())
                    summary.AppendLine("  - " + component.GetType().Name);
            }

            File.WriteAllText("Assets/prefab_summary.txt", summary.ToString());
            Debug.Log("Prefab summary generated! Check Assets/prefab_summary.txt");
        }
    }
}
