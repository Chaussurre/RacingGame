using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector]
    public readonly List<CarController> Players = new List<CarController>();

    [SerializeField]
    private PositionLine PositionLine;

    [SerializeField]
    private float GhostTimerOnRespawn;
    [SerializeField]
    private float ScoreTimerMax;
    private float ScoreTimer;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Players.AddRange(FindObjectsOfType<CarController>());
        Debug.Log("Found " + Players.Count + " players!");
        ScoreTimer = ScoreTimerMax;
    }

    private void FixedUpdate()
    {
        PositionLine.SetFollowedCar(FindFirstPlayer());


        ScoreFirstPlayer();
        FadePositionLine();
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
            float progress = minBlock.GetProgress(car.EffectivePosition);
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
            float progress = maxBlock.GetProgress(car.EffectivePosition);
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

    public void ScoreFirstPlayer()
    {
        if (ScoreTimer > 0)
        {
            ScoreTimer -= Time.fixedDeltaTime;
            return;
        }

        CarController first = FindFirstPlayer();
        first.Score(GhostTimerOnRespawn);

        ScoreTimer = ScoreTimerMax;
    }
    void FadePositionLine()
    {
        SpriteRenderer renderer = PositionLine.GetComponent<SpriteRenderer>();
        float alpha = 0;

        if (ScoreTimer < ScoreTimerMax * .5f && ScoreTimer > ScoreTimerMax * 0.2f)
            alpha = (ScoreTimerMax * .5f - ScoreTimer) / (ScoreTimerMax / 3f); //Linear value between 50% and 80%
        else if (ScoreTimer <= 0.2f * ScoreTimerMax)
            alpha = 1;

        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b , alpha);
    }
}
