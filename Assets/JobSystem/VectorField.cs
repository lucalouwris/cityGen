using System;
using Unity.Mathematics;

[Serializable]
public struct VectorField
{
    public VectorTypes type;
    public float2 center;
    public float radius;
    public float2 dimensions;
    public float fallOff;
}