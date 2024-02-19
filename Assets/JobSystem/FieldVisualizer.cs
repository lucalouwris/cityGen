using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class FieldVisualizer : MonoBehaviour
{
    public VectorField[] fieldTypes;

    public float2 dimensions = new(1f,1f);
    [Min(1)]
    public float resolution;

    public NativeArray<float4> _fieldDirections;
    public NativeArray<float2> _fieldPositions;

    public UnityEvent generationCompleted;

    private void OnValidate()
    {
        if (!Application.isPlaying || dimensions.x <= 0 || dimensions.y <= 0)
            return;
        
        if(_fieldDirections.IsCreated)
            _fieldDirections.Dispose();
        if (_fieldPositions.IsCreated)
            _fieldPositions.Dispose();

        ValidateFieldTypes();
        
        _fieldPositions = CalculatePostions();
        _fieldDirections = new NativeArray<float4>(_fieldPositions.Length, Allocator.Persistent);
        NativeArray<VectorField> tempFieldTypes = new NativeArray<VectorField>(fieldTypes, Allocator.TempJob);
        
        CalculateFieldFromPosition job = new CalculateFieldFromPosition
        {
            fields = tempFieldTypes,
            positions = _fieldPositions,
            result = _fieldDirections
        };
        JobHandle jobHandler = job.Schedule(_fieldPositions.Length, 64);

        
        jobHandler.Complete();
        
        tempFieldTypes.Dispose();
        _fieldDirections = job.result;

        if (generationCompleted == null)
        {
            generationCompleted = new UnityEvent();
        }
        generationCompleted.Invoke();
    }

    private void ValidateFieldTypes()
    {
        for (int index = 0; index < fieldTypes.Length; index++)
        {
            VectorField field = fieldTypes[index];
            switch (field.type)
            {
                case VectorTypes.Base:
                    break;
                case VectorTypes.Grid:
                    field.radius = field.dimensions.x > field.dimensions.y ? field.dimensions.x : field.dimensions.y;
                    break;
                case VectorTypes.Radial:
                    field.dimensions = new float2(2 * field.radius,2 * field.radius);
                    break;
                case VectorTypes.Spline:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            fieldTypes[index] = field;
        }
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
        for (int col = 0; col < (dimensions.x + 1)/resolution; col++)
        {
            for (int row = 0; row < (dimensions.y + 1)/resolution; row++)
            {
                // Calculate the position based on the grid, spacing, and offset
                pos.x = col * resolution;
                pos.y = row * resolution;
                tempPosList.Add(pos);
            }
        }

        return tempPosList.ToNativeArray(Allocator.Persistent);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube((Vector2)dimensions/2f, (Vector2)dimensions);
        
        // Draw info field types
        foreach (var VARIABLE in fieldTypes)
        {
            Gizmos.DrawWireCube((Vector2)VARIABLE.center, Vector3.one * .5f);
            if(VARIABLE.type == VectorTypes.Radial)
                Gizmos.DrawWireSphere((Vector2)VARIABLE.center, VARIABLE.radius);
            if (VARIABLE.type == VectorTypes.Grid)
            {
                Gizmos.DrawWireCube((Vector2)VARIABLE.center, (Vector2)VARIABLE.dimensions);
            }
        }
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
