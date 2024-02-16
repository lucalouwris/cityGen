using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct CalculateFieldFromPosition : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<VectorField> _fields;
    // Jobs declare all data that will be accessed in the job
    // By declaring it as read only, multiple jobs are allowed to access the data in parallel
    [ReadOnly]
    public NativeArray<float2> postions;

    // By default containers are assumed to be read & write
    public NativeArray<float4> result;

    public void Execute(int index)
    {
        result[index] = CalculateDirection(index);
    }
    
    public float4 CalculateDirection(int posIndex)
    {
        float4 TotalDirection = float4.zero;
        for (var fieldIndex = 0; fieldIndex < _fields.Length; fieldIndex++)
        { 
            if (!InBounds())
                continue;
            switch (_fields[fieldIndex].type)
            {
                case VectorTypes.Grid:
                    TotalDirection += CalculateGridDirection(fieldIndex, posIndex);
                    break;
                case VectorTypes.Radial:
                    break;
                case VectorTypes.Spline:
                    break;
            }
        }

        TotalDirection.xy = math.normalize(TotalDirection.xy);
        TotalDirection.zw = math.normalize(TotalDirection.zw);
        return TotalDirection;
    }

    private float4 CalculateGridDirection(int fieldIndex, int posIndex)
    {
        // Calculate direction for major
        float2 majorDirection = math.normalize(_fields[fieldIndex].rotation);
        // Calculate direction for minor
        float2 minorDirection = new float2(-majorDirection.y, majorDirection.x);
        float strength = CalculateStrength(fieldIndex, posIndex );
        return new float4(majorDirection, minorDirection) * strength;
    }

    private float CalculateStrength(int fieldIndex, int posIndex)
    {
        // Calculate distance to center
        float sqrDistance = math.distancesq( postions[posIndex], _fields[fieldIndex].center);
        float radius = _fields[fieldIndex].radius;
        // If the sqrDistance is greater than the radius, return zero strength
        if (sqrDistance > radius * radius)
        {
            return 0f;
        }

        // Calculate the strength using the inverse square law
        return math.pow(1-sqrDistance/(radius*radius),_fields[fieldIndex].fallOff) * _fields[fieldIndex].multiplier;
    }

    private bool InBounds()
    {
        return false;
    }
}
