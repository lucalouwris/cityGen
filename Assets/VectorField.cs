using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class VectorField : MonoBehaviour
{
    public VectorType[] allTypes;
    public NativeArray<float4> field;
    public int2 gridSize;
    public float scale = 1f;
    
    private int2 _currentPos;

    private void Start()
    {
        field = new NativeArray<float4>(gridSize.x * gridSize.y, Allocator.Persistent);
        allTypes = GetComponentsInChildren<VectorType>();
    }

    private void OnDestroy()
    {
        field.Dispose();
    }

    private void Update()
    {
        ProcessArrayJob job = new ProcessArrayJob
        {
            dataArray = field,
            gridSize = gridSize,
            size = gridSize,
            rotation = new float2(.3f, .4f),
            center = gridSize/2,
            fallOff = 1,
            multiplier = 1
        };
        JobHandle handle = job.Schedule(gridSize.x * gridSize.y, 32);
        handle.Complete();
        // float4[] directionList = new float4[allTypes.Length];
        // for (int index = 0; index < field.Length; index++)
        // {
        //     _currentPos.x = index % gridSize.x;
        //     _currentPos.y = index / gridSize.y;
        //     for (var i = 0; i < allTypes.Length; i++)
        //     {
        //         float4 vectorData = allTypes[i].CalculateVortex(_currentPos, out float vectorStrength);
        //         if (vectorStrength == 0)
        //         {
        //             directionList[i] = float4.zero;
        //             continue;
        //         }
        //         vectorData *= vectorStrength;
        //         directionList[i] = vectorData;
        //     }
        //     field[index] = CombineDirections(directionList);
        // }
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
            pos.x = index % gridSize.x;
            pos.y = index / gridSize.y;
            float4 vector = field[index];
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
    
    [BurstCompile]
    struct ProcessArrayJob : IJobParallelFor
    {
        public NativeArray<float4> dataArray;
        public int2 gridSize;
        public int2 _currentPos;
        public float2 size;
        public float2 rotation; 
        public float2 center;
        public float fallOff;
        public float multiplier;
        public void Execute(int index)
        {
            _currentPos.x = index % gridSize.x;
            _currentPos.y = index / gridSize.y;
            
            float4 vectorData = CalculateVortex(_currentPos, out float vectorStrength);
            if (vectorStrength == 0)
            {
                return;
            }
            vectorData *= vectorStrength;

            dataArray[index] = vectorData;
        }
        
        public float4 CalculateVortex(float2 position, out float strength)
        {
            if(!InBounds(position))
            {
                strength = 0;
                return float4.zero;
            }
        
            // Calculate direction for major
            float2 majorDirection = math.normalize(rotation);
            // Calculate direction for minor
            float2 minorDirection = new float2(-majorDirection.y, majorDirection.x);
        
            // Calculate strength based on distance to center and fallout
            strength = CalculateStrength(position);
            return new float4(majorDirection, minorDirection);
        }

        /// <summary>
        /// This calculates the strength of the vector, running a distance check to each border and calculating the percentage from the center
        /// </summary>
        /// <param name="sqrDistance"></param>
        /// <returns></returns>
        internal float CalculateStrength(float2 position)
        {
            float2 corner = new float2(center.x + size.x / 2, center.y + size.y / 2);
            // Calculate distance to center
            float sqrDistance = math.distancesq(position, center);
            float sqrDistanceCorner = math.distancesq(corner, center);
            // If the sqrDistance is greater than the radius, return zero strength
            if (sqrDistance > sqrDistanceCorner)
            {
                return 0f;
            }

            // Calculate the strength using the inverse square law
            return Mathf.Pow(1-(sqrDistance/(sqrDistanceCorner)),fallOff) * multiplier;
        }
    
        private bool InBounds(float2 position)
        {
            if (center.x - size.x / 2f < position.x &&
                center.y - size.y / 2f < position.y &&
                center.x + size.x / 2f > position.x &&
                center.y + size.y / 2f > position.y)
            {
                return true;
            }
            return false;
        }
    }
}