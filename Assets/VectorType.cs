using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class VectorType : MonoBehaviour
{
    public float2 center;
    [Range(0.0f, 3f)]
    public float fallOff;
    public float multiplier = 1f;
    internal virtual void Update()
    {
        center.x = transform.position.x;
        center.y = transform.position.y;
    }
    
    /// <summary>
    /// This function calculates all the necessary data to display the vector on the vector field
    /// </summary>
    /// <param name="position">What position to check for</param>
    /// <param name="strength">Returns calculated strength</param>
    /// <returns>float 4 which contains the majorDirection (xy) and minorDirection(zw)</returns>
    public virtual float4 CalculateVortex(float2 position, out float strength)
    {
        strength = 0;
        return float4.zero;
    }

    internal virtual float CalculateStrength(float2 sqrDistance)
    {
        return 1f;
    }
}