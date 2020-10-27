using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoInput : CarControllerInput
{
    public override ControlInputData GetInput()
    {
        ControlInputData inputData = new ControlInputData();
        inputData.Forward = 0;
        inputData.Horizontal = 0;
        inputData.Turbo = false;

        return inputData;
    }
}
