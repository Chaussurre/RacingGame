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

    public override float GetProgress(Vector3 Position, out Vector2 Normal)
    {
        Normal = (Position - origin.position).normalized;

        return 0f;
    }
}
