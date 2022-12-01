using ArkadiumTest.Movement;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Grid3D = ArkadiumTest.Logic.Grid3D;

[CustomEditor(typeof(Grid3D))]
public class GridEditor : Editor
{
    private const string VAR_SCRIPT = "m_Script";
    private const string VAR_TRANSFORMS = "_transforms";
    private const string VAR_DIMENSIONS = "_dimensions";
    private const string VAR_SELECTED = "_selected";

    private const string PREFAB_PATH = "Assets/Bundle/Prefabs/Cube.prefab";
    private GameObject prefab;


    private FieldInfo _selectedField;
    private Type _selectedFieldType;

    private void OnEnable()
    {
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH);

        Type grid = typeof(Grid3D);
        _selectedField = grid.GetField(VAR_SELECTED, BindingFlags.Instance | BindingFlags.NonPublic);

        _selectedFieldType = typeof(Transform);
    }

    public override void OnInspectorGUI()
    {
        SerializedProperty iterator = serializedObject.GetIterator();

        iterator.NextVisible(true);

        do
        {
            switch (iterator.name)
            {
                case VAR_SCRIPT:
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(iterator);
                    EditorGUI.EndDisabledGroup();
                    break;
                case VAR_TRANSFORMS:
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(iterator);
                    EditorGUI.EndDisabledGroup();
                    break;
                default:
                    EditorGUILayout.PropertyField(iterator);
                    break;
            }
        } while (iterator.NextVisible(false));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(new GUIContent("EDITOR"), EditorStyles.boldLabel);

        Transform value = _selectedField.GetValue(target) as Transform;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField(new GUIContent("Selected"), value, _selectedFieldType, true);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(10);

        prefab = EditorGUILayout.ObjectField(new GUIContent("Prefab"), prefab, typeof(GameObject), false) as GameObject;

        if (prefab != null)
        {
            if (GUILayout.Button(new GUIContent("Build grid")))
                BuildGrid();
        }
        else
        {
            EditorGUILayout.LabelField(new GUIContent($"No prefab found at [{PREFAB_PATH}].\nPlease, correct path or assign by hand."), EditorStyles.miniLabel, GUILayout.Height(25));
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void BuildGrid()
    {
        Transform parent = (target as Grid3D).transform;
        Vector3Int dimensions = serializedObject.FindProperty(VAR_DIMENSIONS).vector3IntValue;
        SerializedProperty transforms = serializedObject.FindProperty(VAR_TRANSFORMS);

        for (int i = 0; i < transforms.arraySize; i++)
            DestroyImmediate((transforms.GetArrayElementAtIndex(i).objectReferenceValue as Transform).gameObject);

        transforms.arraySize = dimensions.x * dimensions.y * dimensions.z;

        float zStart = (float)(dimensions.z - 1) / 2;
        int count = 0;
        for (int z = 0; z < dimensions.z; z++)
        {
            float yStart = (float)(dimensions.y - 1) / 2;
            for (int y = 0; y < dimensions.y; y++)
            {
                float xStart = (float)(dimensions.x - 1) / 2;
                for (int x = 0; x < dimensions.x; x++)
                {
                    Transform instance = (PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject).transform;
                    instance.localPosition = new Vector3(x - xStart, y - yStart, z - zStart);
                    instance.name = $"{x} - {y} - {z}";
                    transforms.GetArrayElementAtIndex(count++).objectReferenceValue = instance;
                }
            }
        }

        EditorUtility.SetDirty(target);
    }
}