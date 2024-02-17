using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VectorVisualization : MonoBehaviour
{
    public VectorType[] allTypes;
    public float4[] field;
    public int2 gridSize;
    public float resolution = 1f;
    
    private float2 _currentPos;

    private void Start()
    {
        allTypes = GetComponentsInChildren<VectorType>();
    }

    private void OnValidate()
    {
        field = new float4[(int)(gridSize.x * gridSize.y / resolution )];
        
        float4[] directionList = new float4[allTypes.Length];
        for (int index = 0; index < field.Length; index++)
        {
            _currentPos.x = (index % gridSize.x) * resolution;
            _currentPos.y = (index / gridSize.x) * resolution;
            for (var i = 0; i < allTypes.Length; i++)
            {
                float4 vectorData = allTypes[i].CalculateVortex(_currentPos, out float vectorStrength);
                if (vectorStrength == 0)
                {
                    directionList[i] = float4.zero;
                    continue;
                }
                vectorData *= vectorStrength;
                directionList[i] = vectorData;
            }
            field[index] = CombineDirections(directionList);
        }
    }

    private float4 CombineDirections(float4[] directionList)
    {
        float4 combinedDirection = default;
        for (int i = 0; i < directionList.Length; i++)
        {
            combinedDirection += directionList[i];
        }

        combinedDirection.xy = math.normalize(combinedDirection.xy);
        combinedDirection.zw = math.normalize(combinedDirection.zw);
        return combinedDirection;
    }
    
    private void OnDrawGizmos()
    {
        float2 pos = default;
        for (int index = 0; index < field.Length; index++)
        {
            Gizmos.color = Color.white;
            pos.x = (index % gridSize.x) * resolution;
            pos.y = (index / gridSize.x) * resolution;
            float4 vector = field[index];
            float2 direction = default;
            if (vector.Equals(float4.zero))
            {
                Gizmos.color = Color.blue;
                direction = new float2(vector.x, vector.y);
                Gizmos.DrawLine((Vector2) pos, (Vector2)(pos) + (Vector2) direction); 
                continue;
            }
            direction = new float2(vector.x, vector.y);
            Gizmos.DrawLine((Vector2) pos, (Vector2)(pos) + (Vector2) direction); 
            Gizmos.color = Color.red;
            direction = new float2(vector.z, vector.w);
            Gizmos.DrawLine((Vector2) pos, (Vector2)(pos) + (Vector2) direction); 
        }
    }
}