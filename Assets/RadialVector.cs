using Unity.Mathematics;
using UnityEngine;

public class RadialVector : VectorType
{
    /// <summary>
    /// This function calculates all the necessary data to display the vector on the vector field
    /// </summary>
    /// <param name="position">What position to check for</param>
    /// <param name="strength">Returns calculated strength</param>
    /// <returns>float 4 which contains the majorDirection (xy) and minorDirection(zw)</returns>
    public override float4 CalculateVortex(float2 position, out float strength)
    {
        // Calculate direction for major
        float2 majorDirection = math.normalize(center - position);
        // Calculate direction for minor
        float2 minorDirection = new float2(-majorDirection.y, majorDirection.x);
        
        // Calculate distance to center
        float sqrDistance = math.distancesq(position, center);
        
        // Calculate strength based on distance to center and fallout
        strength = CalculateStrength(sqrDistance);
        return new float4(majorDirection, minorDirection);
    }

    /// <summary>
    /// This calculates the strength of the vector to use
    /// </summary>
    /// <param name="sqrDistance"></param>
    /// <returns></returns>
    internal override float CalculateStrength(float sqrDistance)
    {
        // If the sqrDistance is greater than the radius, return zero strength
        if (sqrDistance > radius * radius)
        {
            return 0f;
        }

        // Calculate the strength using the inverse square law
        return Mathf.Pow(1-(sqrDistance/(radius*radius)),fallOff);
    }
}