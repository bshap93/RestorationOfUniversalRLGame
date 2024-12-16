using UnityEditor;

namespace Project.Settings.Editor
{
    public static class GSpawnHotkeys
    {
        // Use your logged menu paths here, with proper escaping
        [MenuItem("GSpawn/Open Prefab Library Manager &%#L")] // ALT + CTRL + SHIFT + L
        public static void OpenPrefabLibraryManager()
        {
            // Replace with the exact path from your menu logging
            EditorApplication.ExecuteMenuItem("Tools/GSpawn (PRO)/Windows/Prefab Library Manager");
        }

        [MenuItem("GSpawn/Open Prefab Manager &%#P")] // ALT + CTRL + SHIFT + P
        public static void OpenPrefabManager()
        {
            // Replace with the exact path from your menu logging
            EditorApplication.ExecuteMenuItem("Tools/GSpawn (PRO)/Windows/Prefab Manager");
        }

        [MenuItem("GSpawn/Open Both Managers &%#G")] // ALT + CTRL + SHIFT + G
        public static void OpenBothManagers()
        {
            OpenPrefabLibraryManager();
            OpenPrefabManager();
        }
    }
}
