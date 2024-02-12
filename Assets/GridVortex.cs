﻿using System;
using UnityEngine;

public class GridVortex : VortexType
{
    public Vector2Int VortexSize = Vector2Int.one * 2;
    //public float rotation;
    public override Vector3 CalculateVortex(Vector2 position)
    {
        if(InBounds(position))
            return transform.forward;
        return Vector3.zero;
    }
    private bool InBounds(Vector2 position)
    {
        if (VortexCenter.x - VortexSize.x / 2f < position.x &&
            VortexCenter.y - VortexSize.y / 2f < position.y &&
            VortexCenter.x + VortexSize.x / 2f > position.x &&
            VortexCenter.y + VortexSize.y / 2f > position.y)
        {
            Debug.Log("Is in bounds");
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Vector2[] corners = new Vector2[4];
        var position = transform.position;
        corners[0] = new Vector2(position.x - VortexSize.x/2f, position.y - VortexSize.y/2f); // Bottom-left corner
        corners[1] = new Vector2(position.x + VortexSize.x/2f, position.y - VortexSize.y/2f); // Bottom-right corner
        corners[2] = new Vector2(position.x + VortexSize.x/2f, position.y + VortexSize.y/2f);
        corners[3] = new Vector2(position.x - VortexSize.x/2f, position.y + VortexSize.y/2f); // Top-left corner
        for (int index = 0; index < corners.Length; index++)
        {
            Gizmos.DrawLine(corners[index],corners[(index + 1) % corners.Length]);
        }
    }
}
