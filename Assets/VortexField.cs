using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VortexField : MonoBehaviour
{
    public VortexType[] AllTypes;
    public Vector2Int CurrentPos;
    public Vector2[] Field;
    public Vector2Int GridSize = Vector2Int.one * 2;
    // Start is called before the first frame update
    void Start()
    {
        AllTypes = GetComponentsInChildren<VortexType>();
        Field = new Vector2[GridSize.x * GridSize.y];
        Debug.Log(Field[0]);
    }

    private void Update()
    {
        List<Vector2> directionList = new List<Vector2>();
        for (int index = 0; index < Field.Length; index++)
        {
            directionList.Clear();
            CurrentPos.x = index % GridSize.x;
            CurrentPos.y = index / GridSize.y;
            foreach (VortexType vortex in AllTypes)
            {
                Vector3 vortexData = vortex.CalculateVortex(CurrentPos);
                if (vortexData.z <= 0) continue;
                Vector2 direction = Vector2.zero;
                direction.x = vortexData.x * vortexData.z;
                direction.y = vortexData.y * vortexData.z;
                directionList.Add(direction);
            }

            Field[index] = CombineDirections(directionList.ToArray());
        }
    }

    private void OnDrawGizmos()
    {
        Vector2Int pos = Vector2Int.zero;
        for (int index = 0; index < Field.Length; index++)
        {
            pos.x = index % GridSize.x;
            pos.y = index / GridSize.y;
            Vector2 vertex = Field[index];
            Gizmos.DrawLine((Vector2)pos, pos + Field[index]);
        }
    }
    
    Vector2 CombineDirections(Vector2[] directions)
    {
        Vector2 combinedDirection = Vector2.zero;

        // Sum all direction vectors
        foreach (Vector2 direction in directions)
        {
            combinedDirection += direction;
        }

        // Normalize the combined direction to make it a unit vector
        combinedDirection.Normalize();

        return combinedDirection;
    }
}