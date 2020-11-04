using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : CarControllerInput
{
    public float TimerGoBack; //How long does it go backward after hitting a wall
    float turnBack = 0;

    protected override Vector2 GetTargetPoint()
    {
        Vector2Int GridPos = MapBuilder.Instance.PositionToGrid(car.EffectivePosition);
        if (MapBuilder.Instance.Circuit.TryGetValue(GridPos, out CircuitBlock block))
            return block.NextBlock.transform.position;

        return car.transform.position + car.transform.up;
    }

    protected override bool GetTurbo()
    {
        return turnBack <= 0;
    }

    protected override float GetForward()
    {
        if (turnBack <= 0)
            return 1;
        turnBack -= Time.fixedDeltaTime;
        return -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (car.Bumped == null)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Circuit"))
                turnBack = TimerGoBack;
        }
    }
}
