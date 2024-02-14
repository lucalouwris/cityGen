using System;
using UnityEngine;

public class RadialVortex : VortexType
{
    public override Vector3 CalculateVortex(Vector2 position)
    {
        Vector2 difference = position - VortexCenter;
        float distance = Vector2.SqrMagnitude(difference);

        Vector2 direction = difference.normalized;
        return new Vector3
        {
            x = direction.x,
            y = direction.y,
            z = CalculateStrength(distance)
        };
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
