using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class VectorField : MonoBehaviour
{
    private VectorType[] activeFields;
    private VectorValues[] allTypes;
    public float4[] result;
    public int2 gridSize;
    public float scale = 1f;
    private int2 _currentPos;

    private void Start()
    {
        activeFields = GetComponentsInChildren<VectorType>();
        allTypes = new VectorValues[activeFields.Length];
    }

    private void Update()
    {
        JobUpdate();
    }

    private void JobUpdate()
    {
        NativeArray<float4> field = new NativeArray<float4>(gridSize.x * gridSize.y, Allocator.Persistent);

        CalculateField job = new CalculateField
        {
            dataArray = field,
            gridSize = gridSize,
        };
        
        for (int i = 0; i < activeFields.Length; i++)
        {
            switch (activeFields[i])
            {
                case GridVector gridVector:
                    FillValues(i);
                    FillGridValues(i);
                    break;
                case RadialVector radialVector:
                    FillRadialValues(i);
                    break;
                case SplineVector splineVector:
                    FillSplineValues(i);
                    break;
                default:
                    continue;
            }
        }
        
        
        JobHandle handle = job.Schedule(gridSize.x * gridSize.y, 32);
        handle.Complete();

        result = field.ToArray();
        field.Dispose();
    }

    private void FillValues(int i)
    {
        allTypes[i].center = activeFields[i].center;
        allTypes[i].fallOff = activeFields[i].fallOff;
        allTypes[i].multiplier = activeFields[i].multiplier;
    }

    private void FillGridValues(int i)
    {
        allTypes[i].type = VectorTypes.Grid;
        allTypes[i].size = ((GridVector)activeFields[i]).size;
        allTypes[i].rotation = ((GridVector)activeFields[i]).rotation;
    }

    private void FillSplineValues(int i)
    {
        allTypes[i].type = VectorTypes.Spline;
        allTypes[i].spline = ((SplineVector)activeFields[i]).mainSpline;
        allTypes[i].radius = ((SplineVector)activeFields[i]).radius;
    }

    private void FillRadialValues(int i)
    {
        allTypes[i].type = VectorTypes.Radial;
        allTypes[i].radius = ((RadialVector) activeFields[i]).radius;
    }

    private void OnDrawGizmos()
    {
        float2 pos = default;
        for (int index = 0; index < result.Length; index++)
        {
            Gizmos.color = Color.white;
            pos.x = index % gridSize.x;
            pos.y = index / gridSize.y;
            float4 vector = result[index];
            float2 direction = default;
            if (vector.Equals(float4.zero))
            {
                Gizmos.color = Color.blue;
                direction = new float2(vector.x, vector.y);
                Gizmos.DrawLine((Vector2) pos*scale, (Vector2)(pos * scale) + (Vector2) direction); 
                continue;
            }
            direction = new float2(vector.x, vector.y);
            Gizmos.DrawLine((Vector2) pos*scale, (Vector2)(pos * scale) + (Vector2) direction); 
            Gizmos.color = Color.red;
            direction = new float2(vector.z, vector.w);
            Gizmos.DrawLine((Vector2) pos*scale, (Vector2)(pos * scale) + (Vector2) direction); 
        }
    }
}

[Serializable]
public struct VectorValues
{
    public VectorTypes type;
    public float2 center;
    [Range(0.0f, 3f)]
    public float fallOff;
    public float multiplier;
    
    // Grid info
    public float2 size;
    public float2 rotation;
    
    // Spline info
    public Spline spline;
    
    // Radial and spline info
    public float radius;
}

public enum VectorTypes
{
    Basic,
    Grid,
    Radial,
    Spline
}