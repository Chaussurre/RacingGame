using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : CarControllerInput
{
    private void Start()
    {
        GameManager.Instance.SetMainPlayer(GetComponentInParent<CarController>());
    }
    public override ControlInputData GetInput()
    {
        ControlInputData inputData = new ControlInputData();
        inputData.Forward = Input.GetAxis("Forward");
        inputData.Horizontal = Input.GetAxis("Horizontal");
        inputData.Turbo = Input.GetAxis("Turbo") > 0;

        return inputData;
    }
}
