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

    public CarController FindFirstPlayer()
    {
        return Players[0]; //FIXME
    }

    public CarController FindLastPlayer()
    {
        return Players[0]; //FIXME
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
