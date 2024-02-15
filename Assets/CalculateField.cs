using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
struct CalculateField : IJobParallelFor
{
    public NativeArray<float4> dataArray;
    public int2 gridSize;
    public VectorValues vectorFields;
    private int2 _currentPos;
    public void Execute(int index)
    {
        _currentPos.x = index % gridSize.x;
        _currentPos.y = index / gridSize.y;
            
        float4 vectorData = CalculateVortex(_currentPos, out float vectorStrength);
        if (vectorStrength == 0)
        {
            return;
        }
        vectorData *= vectorStrength;

        dataArray[index] = vectorData;
    }
        
    public float4 CalculateVortex(float2 position, out float strength)
    {
        if(!InBounds(position))
        {
            strength = 0;
            return float4.zero;
        }
        
        // Calculate direction for major
        float2 majorDirection = math.normalize(rotation);
        // Calculate direction for minor
        float2 minorDirection = new float2(-majorDirection.y, majorDirection.x);
        
        // Calculate strength based on distance to center and fallout
        strength = CalculateStrength(position);
        return new float4(majorDirection, minorDirection);
    }

    /// <summary>
    /// This calculates the strength of the vector, running a distance check to each border and calculating the percentage from the center
    /// </summary>
    /// <param name="sqrDistance"></param>
    /// <returns></returns>
    internal float CalculateStrength(float2 position)
    {
        float2 corner = new float2(center.x + size.x / 2, center.y + size.y / 2);
        // Calculate distance to center
        float sqrDistance = math.distancesq(position, center);
        float sqrDistanceCorner = math.distancesq(corner, center);
        // If the sqrDistance is greater than the radius, return zero strength
        if (sqrDistance > sqrDistanceCorner)
        {
            return 0f;
        }

        // Calculate the strength using the inverse square law
        return Mathf.Pow(1-(sqrDistance/(sqrDistanceCorner)),fallOff) * multiplier;
    }
    
    private bool InBounds(float2 position)
    {
        if (center.x - size.x / 2f < position.x &&
            center.y - size.y / 2f < position.y &&
            center.x + size.x / 2f > position.x &&
            center.y + size.y / 2f > position.y)
        {
            return true;
        }
        return false;
    }
}