#if UNITY_EDITOR
using System;
using System.IO;
using Project.Core.CharacterCreation;
using UnityEditor;
using UnityEngine;

namespace Project.Core.Editor.Utilities
{
    public static class ScriptableObjectBackupPaths
    {
        public const string BASE_BACKUP_PATH = "Assets/Resources/Backups/";
        public const string TRAITS_BACKUP_PATH = BASE_BACKUP_PATH + "Traits/";
        public const string CLASSES_BACKUP_PATH = BASE_BACKUP_PATH + "Classes/";
        public const string OTHER_BACKUP_PATH = BASE_BACKUP_PATH + "Other/";
    }

    [CustomEditor(typeof(CharacterTrait))]
    public class CharacterTraitEditor : UnityEditor.Editor
    {
        bool showBackupOptions;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var trait = (CharacterTrait)target;

            EditorGUILayout.Space(10);
            showBackupOptions = EditorGUILayout.Foldout(showBackupOptions, "Backup Options");

            if (showBackupOptions)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Show trait category
                EditorGUILayout.LabelField("Category: " + GetTraitCategory(trait));

                if (GUILayout.Button("Create Backup")) BackupTrait(trait);

                using (new EditorGUI.DisabledScope(!HasBackup(trait)))
                {
                    if (GUILayout.Button("Restore from Backup"))
                        if (EditorUtility.DisplayDialog(
                                "Restore Trait",
                                "Are you sure you want to restore from backup? This will override current values.",
                                "Restore", "Cancel"))
                            RestoreTraitFromBackup(trait);
                }

                EditorGUILayout.EndVertical();
            }
        }

        string GetTraitCategory(CharacterTrait trait)
        {
            if (!trait.isClassSpecific) return "General";

            // If trait is available for only one class, use that as category
            if (trait.availableForClasses.Count == 1) return trait.availableForClasses[0].ToString();

            return "Multiple";
        }

        string GetBackupPath(CharacterTrait trait)
        {
            var category = GetTraitCategory(trait);
            var path = Path.Combine(
                ScriptableObjectBackupPaths.TRAITS_BACKUP_PATH,
                category,
                $"{category}Traits_Backups");

            // Ensure directory exists
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return path;
        }

        void BackupTrait(CharacterTrait trait)
        {
            var backupPath = GetBackupPath(trait);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{trait.name}_{timestamp}.json";
            var filePath = Path.Combine(backupPath, fileName);

            // Serialize and save
            var json = JsonUtility.ToJson(trait, true);
            File.WriteAllText(filePath, json);

            Debug.Log($"Trait backed up to: {filePath}");
            AssetDatabase.Refresh();
        }

        bool HasBackup(CharacterTrait trait)
        {
            var backupPath = GetBackupPath(trait);
            if (!Directory.Exists(backupPath)) return false;

            var backups = Directory.GetFiles(backupPath, $"{trait.name}_*.json");
            return backups.Length > 0;
        }

        void RestoreTraitFromBackup(CharacterTrait trait)
        {
            var backupPath = GetBackupPath(trait);
            var backups = Directory.GetFiles(backupPath, $"{trait.name}_*.json");

            if (backups.Length == 0) return;

            // Get most recent backup
            Array.Sort(backups);
            var mostRecentBackup = backups[backups.Length - 1];

            if (File.Exists(mostRecentBackup))
            {
                var json = File.ReadAllText(mostRecentBackup);
                JsonUtility.FromJsonOverwrite(json, trait);
                EditorUtility.SetDirty(trait);
                AssetDatabase.SaveAssets();
                Debug.Log($"Trait restored from backup: {mostRecentBackup}");
            }
        }
    }

    // Similar editor for StartingClass
    [CustomEditor(typeof(StartingClass))]
    public class StartingClassEditor : UnityEditor.Editor
    {
        // Similar implementation but using ScriptableObjectBackupPaths.CLASSES_BACKUP_PATH
        bool showBackupOptions;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var startingClass = (StartingClass)target;

            EditorGUILayout.Space(10);
            showBackupOptions = EditorGUILayout.Foldout(showBackupOptions, "Backup Options");

            if (showBackupOptions)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Show class name
                EditorGUILayout.LabelField("Class: " + startingClass.className);

                if (GUILayout.Button("Create Backup")) BackupClass(startingClass);

                using (new EditorGUI.DisabledScope(!HasBackup(startingClass)))
                {
                    if (GUILayout.Button("Restore from Backup"))
                        if (EditorUtility.DisplayDialog(
                                "Restore Class",
                                "Are you sure you want to restore from backup? This will override current values.",
                                "Restore", "Cancel"))
                            RestoreClassFromBackup(startingClass);
                }

                EditorGUILayout.EndVertical();
            }
        }

        string GetBackupPath(StartingClass startingClass)
        {
            var path = Path.Combine(
                ScriptableObjectBackupPaths.CLASSES_BACKUP_PATH,
                $"{startingClass.className}Classes_Backups");

            // Ensure directory exists
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return path;
        }

        void BackupClass(StartingClass startingClass)
        {
            var backupPath = GetBackupPath(startingClass);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{startingClass.className}_{timestamp}.json";
            var filePath = Path.Combine(backupPath, fileName);

            // Serialize and save
            var json = JsonUtility.ToJson(startingClass, true);
            File.WriteAllText(filePath, json);

            Debug.Log($"Class backed up to: {filePath}");
            AssetDatabase.Refresh();
        }

        bool HasBackup(StartingClass startingClass)
        {
            var backupPath = GetBackupPath(startingClass);
            if (!Directory.Exists(backupPath)) return false;

            var backups = Directory.GetFiles(backupPath, $"{startingClass.className}_*.json");
            return backups.Length > 0;
        }

        void RestoreClassFromBackup(StartingClass startingClass)
        {
            var backupPath = GetBackupPath(startingClass);
            var backups = Directory.GetFiles(backupPath, $"{startingClass.className}_*.json");

            if (backups.Length == 0) return;

            // Get most recent backup
            Array.Sort(backups);
            var mostRecentBackup = backups[backups.Length - 1];

            if (File.Exists(mostRecentBackup))
            {
                var json = File.ReadAllText(mostRecentBackup);
                JsonUtility.FromJsonOverwrite(json, startingClass);
                EditorUtility.SetDirty(startingClass);
                AssetDatabase.SaveAssets();
                Debug.Log($"Class restored from backup: {mostRecentBackup}");
            }
        }
    }

#endif
}
