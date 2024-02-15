using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineVector : VectorType
{
    private SplineContainer splineContainer;
    private Spline _guidespline;
    public float radius = 5f;

    private void OnValidate()
    {
        splineContainer = GetComponent<SplineContainer>();
        _guidespline = splineContainer.Splines[0];
        center = _guidespline.EvaluatePosition(0.5f).xy + ((float3) transform.position).xy;
    }

    public override float4 CalculateVortex(float2 position, out float strength)
    {
        float3 position3 = new float3(position, 0);
        float3 nearest;
        float percentage;
        SplineUtility.GetNearestPoint(_guidespline, position3, out nearest, out percentage);
        float2 majorTangent = math.normalize(nearest.xy - position);
        float2 minorTangent = math.normalize(_guidespline.EvaluateTangent(percentage).xy);
        strength = CalculateStrength(math.distancesq(nearest.xy, position));
        return new float4(majorTangent,minorTangent);
    }
    
    private float CalculateStrength(float sqrDistance)
    {
        // If the sqrDistance is greater than the radius, return zero strength
        if (sqrDistance > radius * radius)
        {
            return 0f;
        }

        // Calculate the strength using the inverse square law
        return Mathf.Pow(1-(sqrDistance/(radius*radius)),fallOff) * multiplier;
    }
}