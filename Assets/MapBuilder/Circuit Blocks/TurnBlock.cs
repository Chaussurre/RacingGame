using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBlock : CircuitBlock
{
    [SerializeField]
    private Transform origin;
    public void SetToLeft(bool ToLeft)
    {
        if (!ToLeft) //Flip the turn on the horizontal Axis if the turn is to the right;
        {
            Vector3 scale = transform.localScale;
            scale = new Vector3(-scale.x, scale.y, scale.z);
            transform.localScale = scale;
        }
    }

    public override float GetProgress(Vector3 Position)
    {
        return -Vector2.Angle(Orientation, GetNormal(Position));
    }

    public override Vector2 GetNormal(Vector3 Position)
    {
        return (Position - origin.position).normalized;
    }

    public override Vector2 GetProjectedParrallel(Vector3 Position, out Vector3 Projection)
    {
        Vector3 Normal = GetNormal(Position);
        Projection = origin.position + Normal * MapBuilder.Instance.BlockSize / 2f;

        if (transform.localScale.x < 0)
            return Vector2.Perpendicular(Normal) * -1;

        return Vector2.Perpendicular(Normal);
    }
}
