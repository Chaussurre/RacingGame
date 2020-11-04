using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : CarControllerInput
{
    override protected void Start()
    {
        base.Start();
        GameManager.Instance.SetMainPlayer(car);
    }

    protected override Vector2 GetTargetPoint()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
    }

    protected override bool GetTurbo()
    {
        return Input.GetMouseButton(0) && Input.GetMouseButton(1);
    }

    protected override float GetForward()
    {
        if (Input.GetMouseButton(0))
            return 1;
        if (Input.GetMouseButton(1))
            return -1;
        return 0;
    }
}
