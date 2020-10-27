using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public float BlockSize;
    public Transform MapDrawer;
    public SpriteRenderer CarCursor;

    private readonly Dictionary<Vector2Int, GameObject> circuit = new Dictionary<Vector2Int, GameObject>();

    private CarController FollowedCar;

    private void Start()
    {
        FollowedCar = FindObjectOfType<CarController>();
        CarCursor.color = FollowedCar.color;
    }

    private void LateUpdate()
    {
        float RealityToMapRatio = BlockSize / MapBuilder.Instance.BlockSize;
        MapDrawer.localPosition = FollowedCar.EffectivePosition * -RealityToMapRatio;
        CarCursor.transform.localPosition = FollowedCar.transform.position * RealityToMapRatio + MapDrawer.localPosition;
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
