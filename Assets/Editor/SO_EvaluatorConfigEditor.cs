using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_EvaluatorConfig))]
public class SO_EvaluatorConfigEditor : Editor
{
    private void DrawPropertySafe(string propertyName, string label = null)
    {
        SerializedProperty prop = serializedObject.FindProperty(propertyName);
        if (prop != null)
        {
            if (string.IsNullOrEmpty(label))
                EditorGUILayout.PropertyField(prop);
            else
                EditorGUILayout.PropertyField(prop, new GUIContent(label));
        }
        else
        {
            EditorGUILayout.HelpBox($"Property '{propertyName}' not found!", MessageType.Warning);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Export Options", EditorStyles.boldLabel);
        DrawPropertySafe("ExportOption");
        DrawPropertySafe("NumberOfIterations");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Base Seed", EditorStyles.boldLabel);
        DrawPropertySafe("BaseSeed");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Algorithms", EditorStyles.boldLabel);
        DrawPropertySafe("Algorithms");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Configuration", EditorStyles.boldLabel);

        // Grid size mode toggle
        SerializedProperty setCustomGridSize = serializedObject.FindProperty("SetCustomGridSize");
        if (setCustomGridSize != null)
        {
            EditorGUILayout.PropertyField(setCustomGridSize, new GUIContent("Use Custom Grid Sizes"));

            if (setCustomGridSize.boolValue)
            {
                // Show custom grid sizes list
                SerializedProperty gridSizes = serializedObject.FindProperty("GridSizes");
                if (gridSizes != null)
                {
                    EditorGUILayout.PropertyField(gridSizes, new GUIContent("Grid Sizes"), true);
                }
            }
            else
            {
                // Show grid size count
                DrawPropertySafe("GridSizeCount", "Number of Grid Sizes");
            }
        }

        // Display the last (largest) grid size as read-only
        SerializedProperty lastGridSize = serializedObject.FindProperty("lastGridSize");
        if (lastGridSize != null)
        {
            GUI.enabled = false;
            EditorGUILayout.IntField("Last Grid Size", lastGridSize.intValue);
            GUI.enabled = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Obstacle Density Ranges", EditorStyles.boldLabel);
        SerializedProperty densityRanges = serializedObject.FindProperty("ObstacleDensityRanges");
        if (densityRanges != null)
        {
            EditorGUILayout.PropertyField(densityRanges, new GUIContent("Density Ranges"), true);
        }
        else
        {
            EditorGUILayout.HelpBox("ObstacleDensityRanges property not found!", MessageType.Error);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Evaluation Settings", EditorStyles.boldLabel);
        DrawPropertySafe("BatchSize");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Noise Configuration", EditorStyles.boldLabel);
        DrawPropertySafe("NoiseScaleMin");
        DrawPropertySafe("NoiseScaleMax");
        DrawPropertySafe("NoiseScaleMultiplier");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Offset Configuration", EditorStyles.boldLabel);
        DrawPropertySafe("OffsetXMinRange");
        DrawPropertySafe("OffsetXMaxRange");
        DrawPropertySafe("OffsetYMinRange");
        DrawPropertySafe("OffsetYMaxRange");
        DrawPropertySafe("OffsetMultiplier");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Height Configuration", EditorStyles.boldLabel);
        DrawPropertySafe("HeightRange");
        DrawPropertySafe("MaxHeightDeviation");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
        DrawPropertySafe("AnimatePaths");
        DrawPropertySafe("PathAnimationDelay");

        serializedObject.ApplyModifiedProperties();
    }
}
