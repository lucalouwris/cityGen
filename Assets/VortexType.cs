using System;
using UnityEngine;

public class VortexType : MonoBehaviour
{
    public Vector2 VortexCenter;
    [Range(0.0f, 8f)]
    public float FallOff;
    public float Radius = 20f;
    
    internal virtual void Update()
    {
        VortexCenter.x = transform.position.x;
        VortexCenter.y = transform.position.y;
    }

    /// <param name="position"></param>
    /// <returns>x & y = direction, z = strength</returns>
    public virtual Vector3 CalculateVortex(Vector2 position)
    {
        return Vector3.zero;
    }

    internal virtual float CalculateStrength(float sqrDistance)
    {
        // If the sqrDistance is greater than the radius, return zero strength
        if (sqrDistance > Radius * Radius)
        {
            return 0f;
        }

        // Calculate the strength using the inverse square law
        return Mathf.Pow(1-(sqrDistance/(Radius*Radius)),FallOff); //(FallOff * (1 - sqrDistance / Radius));
    }
}