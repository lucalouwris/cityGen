﻿using System;
using UnityEngine;

public class VortexType : MonoBehaviour
{
    public Vector2 VortexCenter;
    public float FallOff;
    public float Radius = 20f;
    public AnimationCurve fallOffCurve = AnimationCurve.EaseInOut(0,.5f,1,1);
    
    internal virtual void Update()
    {
        VortexCenter.x = transform.position.x;
        VortexCenter.y = transform.position.y;
    }

    /// <param name="position"></param>
    /// <returns>x & y = direction, z = strength</returns>
    public virtual Vector3 CalculateVortex(Vector2 position)
    {
        return Vector3.zero;
    }

    internal virtual float CalculateStrength(float distance)
    {
        // If the distance is greater than the radius, return zero strength
        if (distance > Radius * Radius)
            return 0f;

        // Calculate the strength using the inverse square law
        return fallOffCurve.Evaluate(Mathf.Pow(1 - distance / (Radius * Radius), FallOff));
    }
}