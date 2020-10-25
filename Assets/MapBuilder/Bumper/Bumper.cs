using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public GameObject BumperUpPrefab;
    public GameObject BumperDownPrefab;

    private Vector2Int StartingPoint;
    private Vector2Int StopingPoint;

    private GameObject BumperUp;
    private GameObject BumperDown;

    public Vector2Int Init(Vector2Int StartingPoint, Vector2Int Direction)
    {
        this.StartingPoint = StartingPoint;
        StopingPoint = StartingPoint;
        Vector2 pos = new Vector2(StartingPoint.x, StartingPoint.y) * MapBuilder.instance.BlockSize;

        Quaternion angle = Quaternion.Euler(0, 0, -90 * Direction.x);
        if (Direction.y == -1)
            angle = Quaternion.Euler(0, 0, 180);

        //Create depart point 
        GameObject BumperUp = Instantiate(BumperUpPrefab, pos, angle, transform);
        this.BumperUp = BumperUp;

        //Find arriving position
        while (MapBuilder.instance.CheckBlock(StopingPoint))
            StopingPoint += Direction;

        //Create arriving point
        pos = (new Vector2(StopingPoint.x, StopingPoint.y) - Direction) * MapBuilder.instance.BlockSize;

        GameObject BumperDown = Instantiate(BumperDownPrefab, pos, angle, transform);
        this.BumperDown = BumperDown;

        return StopingPoint;
    }
}
