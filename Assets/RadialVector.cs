using Unity.Mathematics;
using UnityEngine;

public class RadialVector : VectorType
{
    public float radius = 20f;
    
    public override float4 CalculateVortex(float2 position, out float strength)
    {
        // Calculate direction for major
        float2 majorDirection = math.normalize(center - position);
        // Calculate direction for minor
        float2 minorDirection = new float2(-majorDirection.y, majorDirection.x);
        
        // Calculate strength based on distance to center and fallout
        strength = CalculateStrength(position);
        return new float4(majorDirection, minorDirection);
    }

    /// <summary>
    /// This calculates the strength of the vector, using the radius of the wanted field and the multiplier
    /// </summary>
    /// <param name="sqrDistance"></param>
    /// <returns></returns>
    internal override float CalculateStrength(float2 position)
    {
        // Calculate distance to center
        float sqrDistance = math.distancesq(position, center);
        
        // If the sqrDistance is greater than the radius, return zero strength
        if (sqrDistance > radius * radius)
        {
            return 0f;
        }

        // Calculate the strength using the inverse square law
        return Mathf.Pow(1-(sqrDistance/(radius*radius)),fallOff) * multiplier;
    }
}