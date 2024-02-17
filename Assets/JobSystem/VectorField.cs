using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct VectorField
{
    public VectorTypes type;
    public float2 center;
    [Range(0f, 3f)]
    public float fallOff;
    [Range(0.1f, 3f)]
    public float multiplier;
    [Min(1)]
    public float radius;
    public float2 dimensions;
    public float2 rotation;
}