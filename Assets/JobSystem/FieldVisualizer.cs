using System;
using Unity.Collections;
using Unity.Jobs;
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
        NativeArray<VectorField> allFields = new NativeArray<VectorField>(_fields, Allocator.Temp);
        NativeArray<float2> fieldPositions = CalculatePostions();
        
        CalculateFieldFromPosition job = new CalculateFieldFromPosition
        {
            fields = allFields,
            positions = fieldPositions,
            result = default
        };

        JobHandle jobHandler = job.Schedule(fieldPositions.Length, 32);
        
        jobHandler.Complete();
        visualField = job.result.ToArray();

        allFields.Dispose();
        fieldPositions.Dispose();
    }
}
