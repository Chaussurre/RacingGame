using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : CarControllerInput
{
    override protected void Start()
    {
        base.Start();
        GameManager.Instance.SetMainPlayer(car);
    }
    protected override Vector2 GetTargetPoint()
    {
        throw new System.NotImplementedException(); //Should never be called
    }

    protected override float GetForward()
    {
        return Input.GetAxis("Forward");
    }

    protected override float GetHorizontal()
    {
        return Input.GetAxis("Horizontal");
    }

    protected override bool GetTurbo()
    {
        return Input.GetAxisRaw("Turbo") > 0;
    }
}
