using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public readonly List<CarController> Players = new List<CarController>();

    public static GameManager Instance;

    // Start is called before the first frame update
    void Start()
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
}
