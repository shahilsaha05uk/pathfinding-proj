using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_EvaluatorConfig))]
public class SO_EvaluatorConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Export Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ExportOption"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberOfIterations"));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Algorithms"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Configuration", EditorStyles.boldLabel);

        // Grid size mode toggle
        SerializedProperty setCustomGridSize = serializedObject.FindProperty("SetCustomGridSize");
        EditorGUILayout.PropertyField(setCustomGridSize, new GUIContent("Use Custom Grid Sizes"));

        if (setCustomGridSize.boolValue)
        {
            // Show custom grid sizes list
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridSizes"), 
                new GUIContent("Grid Sizes"), true);
        }
        else
        {
            // Show grid size count
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridSizeCount"),
                new GUIContent("Number of Grid Sizes"));
        }

        // Display the last (largest) grid size as read-only
        SerializedProperty lastGridSize = serializedObject.FindProperty("lastGridSize");
        GUI.enabled = false;
        EditorGUILayout.IntField("Last Grid Size", lastGridSize.intValue);
        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Obstacle Configuration", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ObstacleDensities"), 
            new GUIContent("Obstacle Densities"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ObstacleDensityDeviation"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Evaluation Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("BatchSize"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Noise Configuration", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("NoiseScaleMin"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("NoiseScaleMax"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("NoiseScaleMultiplier"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Offset Configuration", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OffsetXMinRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OffsetXMaxRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OffsetYMinRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OffsetYMaxRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OffsetMultiplier"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Height Configuration", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("HeightRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxHeightDeviation"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimatePaths"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PathAnimationDelay"));

        serializedObject.ApplyModifiedProperties();
    }
}
