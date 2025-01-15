// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(AddInventoryEngineItemQuestAction))]
    public class AddInventoryEngineItemQuestActionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_inventoryName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_itemName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_amount"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}