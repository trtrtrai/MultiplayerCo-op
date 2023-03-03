using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

                var atkIndex = property.FindPropertyRelative("Attack").enumValueIndex;

                if (atkIndex == 1)
                {
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("ObjName"))),
                    property.FindPropertyRelative("ObjName"), new GUIContent("ObjectName"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("IsNormal"))),
                    property.FindPropertyRelative("IsNormal"), new GUIContent("IsNormal"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("BulletAmount"))),
                    property.FindPropertyRelative("BulletAmount"), new GUIContent("BulletAmount"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("BulletRadian"))),
                    property.FindPropertyRelative("BulletRadian"), new GUIContent("BulletRadian"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Direction"))),
                    property.FindPropertyRelative("Direction"), new GUIContent("Direction"));
                    line++;
                }
                else if (atkIndex == 2)
                {
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("AreaType"))),
                    property.FindPropertyRelative("AreaType"), new GUIContent("AreaType"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("ObjName"))),
                    property.FindPropertyRelative("ObjName"), new GUIContent("ObjectName"));
                    line++;

                    var areaType = property.FindPropertyRelative("AreaType").enumValueIndex;

                    if (areaType == 1)
                    {
                        EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("PerSeconds"))),
                        property.FindPropertyRelative("PerSeconds"), new GUIContent("PerSeconds"));
                        line++;
                    }
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("StatsType"))),
                    property.FindPropertyRelative("StatsType"), new GUIContent("StatsType"));
                    line++;
                }
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
            else if (index == 2)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Special"))),
                    property.FindPropertyRelative("Special"),
                    new GUIContent("Special type"));
                line++;

                var speIndex = property.FindPropertyRelative("Special").enumValueIndex;

                if (speIndex == 0) //Summon
                {
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("SPlace"))),
                        property.FindPropertyRelative("SPlace"),
                        new GUIContent("Place"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("SummonCreature"))),
                        property.FindPropertyRelative("SummonCreature"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("SummonAmount"))),
                        property.FindPropertyRelative("SummonAmount"));
                    line++;
                }
                else if (speIndex == 1) //Teleport
                {
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("TPlace"))),
                        property.FindPropertyRelative("TPlace"),
                        new GUIContent("Place"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("AddOrMultiple"))),
                        property.FindPropertyRelative("Direction"));
                    line++;
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Distance"))),
                        property.FindPropertyRelative("Distance"));
                    line++;
                }
                else if (speIndex == 2) //Immortal
                {
                    EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Duration"))),
                        property.FindPropertyRelative("Duration"));
                    line++;
                }
            }

            if (index == 0 || index == 1)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Duration"))),
                property.FindPropertyRelative("Duration"));
                line++;
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("EffectNumber"))),
                    property.FindPropertyRelative("EffectNumber"));
                line++;
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        /*private void DrawProperty(string name, ref int line, string nameUI = "")
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.size.x, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Special"))),
            property.FindPropertyRelative("Special"),
                    new GUIContent("Special type"));
            line++;
        }*/
    }
}