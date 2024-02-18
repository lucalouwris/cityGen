using System;
using Unity.Mathematics;
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

        float2 fieldTopLeft = visualizer.dimensions;

        EditorGUI.BeginChangeCheck();
        float3 fieldNewTopLeft = Handles.FreeMoveHandle((Vector2)fieldTopLeft, HandleUtility.GetHandleSize((Vector2)fieldTopLeft) * handleSize, Vector3.zero, Handles.DotHandleCap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(visualizer, "Change Rectangle");
            float2 newDimensions = math.abs(fieldNewTopLeft.xy);
            visualizer.dimensions = newDimensions; 
        }
        
        
        for (int i = 0; i < visualizer.fieldTypes.Length; i++)
        {
            Vector2 position = visualizer.fieldTypes[i].center;
            Quaternion rotation = Quaternion.identity;
            
            float size = HandleUtility.GetHandleSize(position) * handleSize;

            EditorGUI.BeginChangeCheck();
            float3 newPosition = Handles.PositionHandle(position, rotation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(visualizer, "Move Point");
                visualizer.fieldTypes[i].center = newPosition.xy;
                EditorUtility.SetDirty(visualizer);
            }

            switch (visualizer.fieldTypes[i].type)
            {
                case VectorTypes.Grid:
                    float2 rectPosition = visualizer.fieldTypes[i].center;
                    float2 topLeft = rectPosition + new float2(-visualizer.fieldTypes[i].dimensions.x / 2f, visualizer.fieldTypes[i].dimensions.y / 2f);
                    float2 bottomRight = rectPosition + new float2(visualizer.fieldTypes[i].dimensions.x / 2f, -visualizer.fieldTypes[i].dimensions.y / 2f);

                    EditorGUI.BeginChangeCheck();
                    float3 newTopLeft = Handles.FreeMoveHandle((Vector2)topLeft, HandleUtility.GetHandleSize((Vector2)rectPosition) * handleSize, Vector3.zero, Handles.DotHandleCap);
                    float3 newBottomRight = Handles.FreeMoveHandle((Vector2)bottomRight, HandleUtility.GetHandleSize((Vector2)rectPosition) * handleSize, Vector3.zero, Handles.DotHandleCap);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(visualizer, "Change Rectangle");
                        float2 newCenter = (newTopLeft.xy + newBottomRight.xy) / 2f;
                        float2 newDimensions = math.abs(newTopLeft.xy - newBottomRight.xy);
                        visualizer.fieldTypes[i].center = newCenter;
                        visualizer.fieldTypes[i].dimensions = newDimensions; 
                    }
                    break;
                case VectorTypes.Radial:
                    EditorGUI.BeginChangeCheck();
                    float newRadius = Handles.RadiusHandle(rotation,(Vector2)position, visualizer.fieldTypes[i].radius);
        
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(visualizer, "Change Radius");
                        visualizer.fieldTypes[i].radius = newRadius;
                        EditorUtility.SetDirty(visualizer);
                    }
                    break;
                case VectorTypes.Spline:
                    break;
            }
            
        }
    }
}
