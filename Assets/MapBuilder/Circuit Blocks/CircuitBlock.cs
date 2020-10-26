using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBlock
{
    public GameObject block;
    public Vector2Int GridPosition;
    public CircuitBlock NextBlock;
    public CircuitBlock PreviousBlock;
}
