using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FieldVisualizer : MonoBehaviour
{
    [SerializeField]
    private VectorField[] fieldTypes;

    [SerializeField]
    private int2 dimensions;
    [SerializeField]
    private int resolution;

    private NativeArray<float4> _fieldDirections;
    private NativeArray<float2> _fieldPositions;

    private void OnValidate()
    {
        if (!Application.isPlaying || resolution <= 0 || dimensions.x <= 0 || dimensions.y <= 0)
            return;
        
        
        if(_fieldDirections.IsCreated)
            _fieldDirections.Dispose();
        if (_fieldPositions.IsCreated)
            _fieldPositions.Dispose();
        
        _fieldPositions = CalculatePostions();
        _fieldDirections = new NativeArray<float4>(_fieldPositions.Length, Allocator.Persistent);
        NativeArray<VectorField> tempFieldTypes = new NativeArray<VectorField>(fieldTypes, Allocator.TempJob);
        
        CalculateFieldFromPosition job = new CalculateFieldFromPosition
        {
            fields = tempFieldTypes,
            positions = _fieldPositions,
            result = _fieldDirections
        };
        JobHandle jobHandler = job.Schedule(_fieldPositions.Length, 32);

        
        jobHandler.Complete();
        
        tempFieldTypes.Dispose();
        _fieldDirections = job.result;
    }

    private void OnDisable()
    {
        if(_fieldDirections.IsCreated)
            _fieldDirections.Dispose();
        if (_fieldPositions.IsCreated)
            _fieldPositions.Dispose();
    }

    private NativeArray<float2> CalculatePostions()
    {
        List<float2> tempPosList = new List<float2>();
        float2 pos = new float2();
        for (int i = 0; i < dimensions.x * dimensions.y; i ++)
        {
            pos.x = i % dimensions.x;
            pos.y = i / dimensions.x;
            tempPosList.Add(pos);
        }

        return tempPosList.ToNativeArray(Allocator.Persistent);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw info field types
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        for (var index = 0; index < _fieldPositions.Length; index++)
        {
            Gizmos.DrawLine((Vector2)_fieldPositions[index], (Vector2)(_fieldPositions[index] + _fieldDirections[index].xy)); ;
        }
    }
}
