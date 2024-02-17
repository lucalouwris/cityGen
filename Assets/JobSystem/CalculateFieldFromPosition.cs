using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct CalculateFieldFromPosition : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<VectorField> fields;
    // Jobs declare all data that will be accessed in the job
    // By declaring it as read only, multiple jobs are allowed to access the data in parallel
    [ReadOnly]
    public NativeArray<float2> positions;

    // By default containers are assumed to be read & write
    public NativeArray<float4> result;

    public void Execute(int index)
    {
        result[index] = CalculateDirection(index);
    }
    
    public float4 CalculateDirection(int posIndex)
    {
        float4 totalDirection = float4.zero;
        
        for (var fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
        { 
            if (!InBounds(fieldIndex, posIndex))
                continue;
            switch (fields[fieldIndex].type)
            {
                case VectorTypes.Grid:
                    totalDirection += CalculateGridDirection(fieldIndex, posIndex);
                    break;
                case VectorTypes.Radial:
                    totalDirection += CalculateRadialDirection(fieldIndex, posIndex);
                    break;
                case VectorTypes.Spline:
                    break;
            }
        }

        totalDirection.xy = math.normalize(totalDirection.xy);
        totalDirection.zw = math.normalize(totalDirection.zw);
        return totalDirection;
    }

    private float4 CalculateRadialDirection(int fieldIndex, int posIndex)
    {
        // Calculate direction for major
        float2 majorDirection = math.normalize(fields[fieldIndex].center - positions[posIndex]);
        
        // Calculate direction for minor
        float2 minorDirection = new float2(-majorDirection.y, majorDirection.x);
        
        float strength = CalculateStrength(fieldIndex, posIndex );
        return new float4(majorDirection * strength, minorDirection * strength);
    }

    private float4 CalculateGridDirection(int fieldIndex, int posIndex)
    {
        // Calculate direction for major
        float2 majorDirection = math.normalize(fields[fieldIndex].rotation);
        
        // Calculate direction for minor
        float2 minorDirection = new float2(-majorDirection.y, majorDirection.x);
        
        float strength = CalculateStrength(fieldIndex, posIndex );
        return new float4(majorDirection * strength, minorDirection * strength);
    }

    private float CalculateStrength(int fieldIndex, int posIndex)
    {
        // Calculate distance to center
        float sqrDistance = math.distancesq( positions[posIndex], fields[fieldIndex].center);
        float radius = fields[fieldIndex].radius;
        
        // If the sqrDistance is greater than the radius, return zero strength
        if (sqrDistance > radius * radius)
        {
            return 0f;
        }

        // Calculate the strength using the inverse square law
        return math.pow(1-sqrDistance/(radius*radius),fields[fieldIndex].fallOff) * fields[fieldIndex].multiplier;
    }

    private bool InBounds(int fieldIndex, int posIndex)
    {
        // Calculate the minimum and maximum boundaries
        float2 minBound = fields[fieldIndex].center - fields[fieldIndex].dimensions / 2;
        float2 maxBound = fields[fieldIndex].center + fields[fieldIndex].dimensions / 2;

        // Check if the position is within the boundaries
        return  positions[posIndex].x >= minBound.x &&  positions[posIndex].x <= maxBound.x &&
                positions[posIndex].y >= minBound.y &&  positions[posIndex].y <= maxBound.y;
    }
}
