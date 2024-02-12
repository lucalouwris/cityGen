using System;
using UnityEngine;

public class RadialVortex : VortexType
{
    public float Radius;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns>x & y = direction, z = strength</returns>
    public override Vector3 CalculateVortex(Vector2 position)
    {
        Vector2 difference = position - VortexCenter;
        float distance = Vector2.SqrMagnitude(difference);

        Vector2 direction = difference.normalized;
        return new Vector3
        {
            x = direction.x,
            y = direction.y,
            z = CalculateStrength(distance)
        };
    }
    
    float CalculateStrength(float distance)
    {
        // If the distance is greater than the radius, return zero strength
        if (distance > Radius * Radius)
            return 0f;

        // Calculate the strength using the inverse square law
        return Mathf.Pow(1 - distance / (Radius * Radius), FallOff);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
