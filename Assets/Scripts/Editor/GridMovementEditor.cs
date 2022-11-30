using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ArkadiumTest.Movement
{
    [CustomEditor(typeof(GridMovement))]
    public class GridMovementEditor : Editor
    {
        private FieldInfo _selectedField;
        private Type _selectedFieldType;

        private void OnEnable()
        {
            Type gridMovement = typeof(GridMovement);
            _selectedField = gridMovement.GetField("_selected", BindingFlags.Instance | BindingFlags.NonPublic);
            
            _selectedFieldType = typeof(Transform);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Transform value = _selectedField.GetValue(target) as Transform;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(new GUIContent("Selected"), value, _selectedFieldType, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}