using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBlock : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    [HideInInspector]
    public CircuitBlock NextBlock;
    [HideInInspector]
    public CircuitBlock PreviousBlock;

    private void Start()
    {
        float BlockSize = MapBuilder.Instance.BlockSize;
        int x = Mathf.RoundToInt(transform.position.x / BlockSize);
        int y = Mathf.RoundToInt(transform.position.y / BlockSize);

        GridPosition = new Vector2Int(x, y);
    }
}
