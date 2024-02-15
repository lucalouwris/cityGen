using System;
using Unity.Mathematics;
using UnityEngine;

public class VectorType : MonoBehaviour
{
    public float2 center;
    [Range(0.0f, 3f)]
    public float fallOff;
    public float radius = 20f;
    internal virtual void Update()
    {
        center.x = transform.position.x;
        center.y = transform.position.y;
    }

    /// <param name="position"></param>
    /// <param name="strength"></param>
    /// <returns>x & y = direction, z = strength</returns>
    public virtual float4 CalculateVortex(float2 position, out float strength)
    {
        strength = 0;
        return float4.zero;
    }

    internal virtual float CalculateStrength(float sqrDistance)
    {
        return 1f;
    }
}