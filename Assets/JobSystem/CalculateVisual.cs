using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct CalculateVisual : IJobParallelFor
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
        result[index] = CalculateDirection(postions[index]);
    }
    
    public float4 CalculateDirection(float2 position)
    {
        float4 TotalDirection = float4.zero;
        int fieldsInReach = 0;
        foreach (VectorField field in _fields)
        {
            if (!InBounds())
                continue;
            fieldsInReach++;
            switch (field.type)
            {
                case VectorTypes.Grid:
                    break;
                case VectorTypes.Radial:
                    break;
                case VectorTypes.Spline:
                    break;
            }
        }
        // // Calculate direction for major
        // float2 majorDirection = math.normalize(rotation);
        // // Calculate direction for minor
        // float2 minorDirection = new float2(-majorDirection.y, majorDirection.x);
        //
        // // Calculate strength based on distance to center and fallout
        // strength = CalculateStrength(position);
        // return new float4(majorDirection, minorDirection);
    }

    private bool InBounds()
    {
        return false;
    }
}
