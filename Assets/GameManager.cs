using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public readonly List<CarController> Players = new List<CarController>();

    [SerializeField]
    private PositionLine PositionLine;

    public static GameManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Players.AddRange(FindObjectsOfType<CarController>());
        Debug.Log("Found " + Players.Count + " players!");

    }

    private void Update()
    {
        PositionLine.SetFollowedCar(FindFirstPlayer());
    }

    public CarController FindFirstPlayer()
    {
        HashSet<CarController> closestBlock = new HashSet<CarController>();
        CircuitBlock minBlock = null;

        foreach(CarController car in Players)
        {
            Vector2Int gridPos = MapBuilder.Instance.PositionToGrid(car.EffectivePosition);
            if(!MapBuilder.Instance.Circuit.TryGetValue(gridPos, out CircuitBlock block))
                continue;

            if (minBlock == null || block.Order <= minBlock.Order)
            {
                if (minBlock != null && block.Order < minBlock.Order)
                    closestBlock.Clear();

                minBlock = block;
                closestBlock.Add(car);
            }
        }

        float MaxProgress = float.MinValue;
        CarController firstCar = null;
        foreach (CarController car in closestBlock)
        {
            float progress = minBlock.GetProgress(car.EffectivePosition, out _);
            if (progress > MaxProgress)
            {
                firstCar = car;
                MaxProgress = progress;
            }
        }
        return firstCar;
    }

    public CarController FindLastPlayer()
    {
        HashSet<CarController> closestBlock = new HashSet<CarController>();
        CircuitBlock maxBlock = null;

        foreach (CarController car in Players)
        {
            Vector2Int gridPos = MapBuilder.Instance.PositionToGrid(car.EffectivePosition);
            if (!MapBuilder.Instance.Circuit.TryGetValue(gridPos, out CircuitBlock block))
                continue;

            if (maxBlock == null || block.Order >= maxBlock.Order)
            {
                if (maxBlock != null && block.Order > maxBlock.Order)
                    closestBlock.Clear();

                maxBlock = block;
                closestBlock.Add(car);
            }
        }

        float MinProgress = float.MaxValue;
        CarController lastCar = null;
        foreach (CarController car in closestBlock)
        {
            float progress = maxBlock.GetProgress(car.EffectivePosition, out _);
            if (progress < MinProgress)
            {
                lastCar = car;
                MinProgress = progress;
            }
        }
        return lastCar;
    }

    public void SetMainPlayer(CarController mainPlayer)
    {
        CameraFollowCar cameraFollow = Camera.main.GetComponent<CameraFollowCar>();
        if (cameraFollow != null)
            cameraFollow.SetFollowed(mainPlayer);

        PositionLine.SetFollowedCar(mainPlayer); //FIXME

        MapBuilder.Instance.miniMap.SetFollowedCar(mainPlayer);
    }
}
