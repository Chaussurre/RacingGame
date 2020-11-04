using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : CarControllerInput
{
    public float TimerGoBack; //How long does it go backward after hitting a wall
    float turnBack = 0;

    public override ControlInputData GetInput()
    {
        ControlInputData inputData = new ControlInputData();

        if (car.Bumped != null)
        {
            turnBack = 0;
            return inputData;
        }

        Vector3 RelativeTarget = GetTarget() - transform.position;

        inputData.Horizontal = 1;
        if (Vector2.Dot(RelativeTarget, transform.right) < 0)
            inputData.Horizontal = -1;

        inputData.Turbo = true;
        if (turnBack <= 0)
            inputData.Forward = 1;
        else //Just hit a wall
        {
            turnBack -= Time.fixedDeltaTime;
            inputData.Forward = -1;
            inputData.Horizontal = 0;
            inputData.Turbo = false;
        }

        return inputData;
    }

    Vector3 GetTarget()
    {

        Vector2Int GridPos = MapBuilder.Instance.PositionToGrid(car.EffectivePosition);
        if (MapBuilder.Instance.Circuit.TryGetValue(GridPos, out CircuitBlock block))
            return block.NextBlock.transform.position;

        return car.transform.position + car.transform.up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Circuit"))
            turnBack = TimerGoBack;
    }
}
