using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Project.Editor.OdinAttributes.Stamina
{
    public class StaminaBarAttributeDrawer : OdinAttributeDrawer<StaminaBarAttribute, float>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Call the next drawer, which will draw the float field.
            CallNextDrawer(label);

            // Get a rect to draw the health-bar on.
            var rect = EditorGUILayout.GetControlRect();

            // Draw the health bar using the rect.
            var width = Mathf.Clamp01(ValueEntry.SmartValue / Attribute.MaxStamina);
            SirenixEditorGUI.DrawSolidRect(rect, new Color(0f, 0f, 0f, 0.3f), false);
            SirenixEditorGUI.DrawSolidRect(rect.SetWidth(rect.width * width), Color.blue, false);
            SirenixEditorGUI.DrawBorders(rect, 1);
        }
    }
}
