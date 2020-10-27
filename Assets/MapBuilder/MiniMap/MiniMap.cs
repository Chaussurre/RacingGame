using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public float BlockSize;
    public Transform MapDrawer;
    public GameObject CarCursorPrefab;

    private readonly Dictionary<Vector2Int, GameObject> circuit = new Dictionary<Vector2Int, GameObject>();
    private readonly Dictionary<CarController, GameObject> Cursors = new Dictionary<CarController, GameObject>();

    private CarController FollowedCar;

    private void Start()
    {
        foreach(CarController car in GameManager.Instance.Players)
        {
            GameObject cursor = Instantiate(CarCursorPrefab, transform);
            cursor.GetComponent<SpriteRenderer>().color = car.color;
            Cursors.Add(car, cursor);
        }
    }

    public void SetFollowedCar(CarController FollowedCar)
    {
        this.FollowedCar = FollowedCar;
    }

    private void LateUpdate()
    {
        float RealityToMapRatio = BlockSize / MapBuilder.Instance.BlockSize;
        
        if (FollowedCar != null)
        MapDrawer.localPosition = FollowedCar.EffectivePosition * -RealityToMapRatio;

        foreach (CarController car in GameManager.Instance.Players)
            Cursors[car].transform.localPosition = car.transform.position * RealityToMapRatio + MapDrawer.localPosition;
    }

    public void CreateBlock(CircuitBlock block)
    {
        GameObject MiniBlock = Instantiate(block.MiniMapVersion, MapDrawer);
        
        MiniBlock.transform.localPosition = (Vector2.zero + block.GridPosition) * BlockSize;
        MiniBlock.transform.localRotation = block.transform.localRotation;
        MiniBlock.transform.localScale = block.transform.localScale;
        circuit.Add(block.GridPosition, MiniBlock);
    }

    public void DestroyBlock(CircuitBlock block)
    {
        if (!circuit.TryGetValue(block.GridPosition, out GameObject MiniBlock))
            return;

        circuit.Remove(block.GridPosition);
        Destroy(MiniBlock);
    }
}
