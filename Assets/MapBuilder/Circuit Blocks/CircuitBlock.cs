using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBlock : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    public Vector2Int Orientation { get; private set; }
    [HideInInspector]
    public CircuitBlock NextBlock;
    [HideInInspector]
    public CircuitBlock PreviousBlock;

    public void Init(Vector2Int GridPosition, Vector2Int Orientation)
    {
        this.GridPosition = GridPosition;
        this.Orientation = Orientation;

        transform.position = new Vector2(GridPosition.x, GridPosition.y) * MapBuilder.Instance.BlockSize;
        transform.rotation = Quaternion.Euler(0, 0, MapBuilder.DirectionToAngle(Orientation));
    }

    public virtual float GetProgress(Vector3 Position, out Vector2 Normal)
    {
        Vector2 relativPos = Position - transform.position;

        float progress = Vector2.Dot(relativPos, Orientation);
        Vector2 projection = (Vector2.zero + Orientation) * progress;
        Normal = Vector2.Perpendicular(projection);

        return progress;
    }
}
