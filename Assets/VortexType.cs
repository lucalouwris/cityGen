using System;
using UnityEngine;

public class VortexType : MonoBehaviour
{
    public Vector2 VortexCenter;
    public float FallOff;
    
    public void Update()
    {
        VortexCenter.x = transform.position.x;
        VortexCenter.y = transform.position.y;
    }

    public virtual Vector3 CalculateVortex(Vector2 position)
    {
        return Vector3.zero;
    }
}