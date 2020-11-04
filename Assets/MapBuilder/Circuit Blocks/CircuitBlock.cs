using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBlock : MonoBehaviour
{
    public GameObject MiniMapVersion;

    public Vector2Int GridPosition { get; private set; }
    public Vector2Int Orientation { get; private set; }
    public int Order { get; private set; }
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

    public virtual float GetProgress(Vector3 Position)
    {
        Vector2 relativPos = Position - transform.position;

        
        return Vector2.Dot(relativPos, Orientation); ;
    }

    public virtual Vector2 GetNormal(Vector3 Position)
    {
        Vector2 projection = (Vector2.zero + Orientation) * GetProgress(Position);
        return Vector2.Perpendicular(projection);
    }

    public virtual Vector2 GetProjectedParrallel(Vector3 Position, out Vector3 Projection)
    {
        Projection = (Vector2.zero + Orientation) * GetProgress(Position);
        Projection += transform.position;
        return Orientation;
    }

    public void SetOrder(int Order)
    {
        this.Order = Order;
    }
}
