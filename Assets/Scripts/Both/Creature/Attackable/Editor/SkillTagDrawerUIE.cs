using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/*namespace Assets.Scripts.Both.Creature.Attackable.Editor
{
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(SkillTag), true)]
    public class SkillTagDrawerUIE : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create property container element.
            var container = new VisualElement();

            // Create property fields.
            var tagField = new PropertyField(property.FindPropertyRelative("Tag"));

            var atkTagField = new PropertyField(property.FindPropertyRelative("Attack"), "Attack type");
            var effTagField = new PropertyField(property.FindPropertyRelative("Effect"), "Effect type");
            var speTagField = new PropertyField(property.FindPropertyRelative("Special"), "Special type");

            var durField = new PropertyField(property.FindPropertyRelative("Duration"));
            var numberField = new PropertyField(property.FindPropertyRelative("EffectNumber"), "Effect number");

            var aomField = new PropertyField(property.FindPropertyRelative("AddOrMultiple"), "+-T/x-F");

            // Add fields to the container.
            container.Add(tagField);
            container.Add(atkTagField);
            container.Add(effTagField);
            container.Add(speTagField);
            container.Add(durField);
            container.Add(numberField);
            container.Add(aomField);

            return container;
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

            // Calculate rects
            var amountRect = new Rect(position.x, position.y, position.width, position.height);
            var unitRect = new Rect(position.x, position.y + position.height, position.width, position.height);
            var nameRect = new Rect(position.x, position.y + position.height * 2, position.width, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("Tag"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("Attack"), GUIContent.none);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Effect"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            *//*var tagField = new PropertyField(property.FindPropertyRelative("Tag"));

            var atkTagField = new PropertyField(property.FindPropertyRelative("Attack"), "Attack type");
            var effTagField = new PropertyField(property.FindPropertyRelative("Effect"), "Effect type");
            var speTagField = new PropertyField(property.FindPropertyRelative("Special"), "Special type");

            var durField = new PropertyField(property.FindPropertyRelative("Duration"));
            var numberField = new PropertyField(property.FindPropertyRelative("EffectNumber"), "Effect number");

            var aomField = new PropertyField(property.FindPropertyRelative("AddOrMultiple"), "+-T/x-F");*//*
        }
    }
}*/