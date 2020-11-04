using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoInput : CarControllerInput
{
    protected override bool GetTurbo()
    {
        return false;
    }

    protected override float GetForward()
    {
        return 0;
    }

    protected override Vector2 GetTargetPoint()
    {
        return car.transform.up;
    }
}
