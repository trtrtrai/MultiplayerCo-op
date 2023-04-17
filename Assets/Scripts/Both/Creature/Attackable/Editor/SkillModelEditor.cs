using Assets.Scripts.Both.Scriptable;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.Editor
{
    [CustomEditor(typeof(SkillModel))]
    public class SkillModelEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var sModel = (SkillModel)target;
            sModel.SkillName = (SkillName)EditorGUILayout.EnumPopup(new GUIContent("Skill Name"), sModel.SkillName);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Description", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                sModel.Description = EditorGUILayout.TextArea(sModel.Description, EditorStyles.textArea, GUILayout.Height(100));
            }
            EditorGUILayout.EndHorizontal();
            sModel.Range = EditorGUILayout.FloatField(new GUIContent("Range"), sModel.Range);
            sModel.CastDelay = EditorGUILayout.FloatField(new GUIContent("Cast Delay"), sModel.CastDelay);
            sModel.Cooldown = EditorGUILayout.FloatField(new GUIContent("Cooldown"), sModel.Cooldown);
            sModel.SkillIcon = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Skill Icon"), sModel.SkillIcon , typeof(Sprite), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SkillTags"), true);
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}