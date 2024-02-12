using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VortexField : MonoBehaviour
{
    public VortexType[] AllTypes;
    public Vector2Int TestPos;
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
        
    }

    private void OnDrawGizmos()
    {
        List<Vector2> directionList = new List<Vector2>();
        for (int index = 0; index < Field.Length; index++)
        {
            directionList.Clear();
            int x = index % GridSize.x;
            int y = index / GridSize.y;
            TestPos.x = x;
            TestPos.y = y;
            foreach (VortexType vortex in AllTypes)
            {
                Vector3 vortexData = vortex.CalculateVortex(TestPos);
                if(vortexData.z <= 0) continue;
                Vector2 direction = Vector2.zero;
                direction.x = vortexData.x * vortexData.z;
                direction.y = vortexData.y * vortexData.z;
                directionList.Add(direction);
            }

            Field[index] = CombineDirections(directionList.ToArray());
            Gizmos.DrawLine((Vector2)TestPos,TestPos + Field[index]);
        }
        //
        //
        // Vector2 direction = Field[index];
        // Vector3 vortexData = AllTypes[0].CalculateVortex(TestPos);
        // Vector2 direction = Vector2.zero;
        // direction.x = vortexData.x * vortexData.z;
        // direction.y = vortexData.y * vortexData.z;
        // Gizmos.DrawLine(TestPos,TestPos + direction);
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