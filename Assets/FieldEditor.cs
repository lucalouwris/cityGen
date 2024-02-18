using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldVisualizer))]
public class FieldEditor : Editor
{
    private SerializedProperty fieldTypesProperty;
    private const float handleSize = 0.04f;
    
    private void OnSceneGUI()
    {
        FieldVisualizer visualizer = (FieldVisualizer)target;
        
        for (int i = 0; i < visualizer.fieldTypes.Length; i++)
        {
            Vector3 position = (Vector2)visualizer.fieldTypes[i].center;
            Quaternion rotation = Quaternion.identity;
            float size = HandleUtility.GetHandleSize(position) * handleSize;

            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(position, rotation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(visualizer, "Move Point");
                visualizer.fieldTypes[i].center = (Vector2) newPosition;
                EditorUtility.SetDirty(visualizer);
            }
        }
    }
}
