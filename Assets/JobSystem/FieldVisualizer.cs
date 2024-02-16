using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class FieldVisualizer : MonoBehaviour
{
    [SerializeField]
    private VectorField[] _fields;

    [SerializeField]
    private int2 dimensions;
    [SerializeField]
    private int resolution;
    
    private float4[] visualField;

    private void OnValidate()
    {
        visualField = new float4[(dimensions.x * dimensions.y) / resolution];
        NativeArray<float4> nativeField =
            new NativeArray<float4>(visualField.Length, Allocator.Temp);
        
        
        CalculateVisual job = new CalculateVisual
        {
            dataArray = field,
            gridSize = gridSize,
        };
        
    }
}
