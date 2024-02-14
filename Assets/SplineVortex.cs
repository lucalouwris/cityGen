using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineVortex : VortexType
{
    [SerializeField]
    private SplineContainer splineContainer;
    
    private Spline _guidespline;

    private void OnValidate()
    {
        splineContainer = GetComponent<SplineContainer>();
        _guidespline = splineContainer.Splines[0];
        float3 centerPos = _guidespline.EvaluatePosition(0.5f);
        centerPos += (float3)transform.position;
        VortexCenter = new Vector2(centerPos.x, centerPos.y);
    }

    public override Vector3 CalculateVortex(Vector2 position)
    {
        float3 floatPosition = new float3(position.x, position.y, 0);
        float3 pointPosition;
        float percentage;
        SplineUtility.GetNearestPoint(_guidespline, floatPosition, out pointPosition, out percentage);
        float3 pointTangent = _guidespline.EvaluateTangent(percentage);
        Vector2 point2d = new Vector2(pointPosition.x, pointPosition.y);
        Vector2 difference = position - point2d;
        float distance = Vector2.SqrMagnitude(difference);
        
        pointTangent.z = CalculateStrength(distance);
        return pointTangent;
    }
}
