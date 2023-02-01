using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.Editor
{
    [CustomPropertyDrawer(typeof(SkillTag))]
    public class SkillTagDrawer : PropertyDrawer
    {
        private int line;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * line;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Tag"))), property.FindPropertyRelative("Tag"));
            line = 1;
            var index = property.FindPropertyRelative("Tag").enumValueIndex;
            if (index == 0)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Attack"))),
                    property.FindPropertyRelative("Attack"));
                line++;
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("AddOrMultiple"))),
                    property.FindPropertyRelative("AddOrMultiple"), new GUIContent("Is Strength Scale"));
                line++;
            }
            else if (index == 1)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Effect"))),
                    property.FindPropertyRelative("Effect"),
                    new GUIContent("Effect type"));
                line++;
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("StatsType"))),
                    property.FindPropertyRelative("StatsType"),
                    new GUIContent("Stats type"));
                line++;
            }
            else
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Special"))),
                    property.FindPropertyRelative("Special"),
                    new GUIContent("Special type"));
                line++;
            }

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Duration"))),
                property.FindPropertyRelative("Duration"));
            line++;
            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("EffectNumber"))),
                property.FindPropertyRelative("EffectNumber"));
            line++;

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}