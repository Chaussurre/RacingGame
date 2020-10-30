using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoInput : CarControllerInput
{
    public override ControlInputData GetInput()
    {
        ControlInputData inputData = new ControlInputData
        {
            Forward = 0,
            Horizontal = 0,
            Turbo = false
        };

        return inputData;
    }
}
