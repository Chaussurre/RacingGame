using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : CarControllerInput
{
    CarController car;

    private void Start()
    {
        car = GetComponentInParent<CarController>();
    }
    public override ControlInputData GetInput()
    {
        ControlInputData inputData = new ControlInputData();
        Vector3 RelativeTarget = GetTarget() - transform.position;


        inputData.Horizontal = 0;
        //inputData.Horizontal = 1;
        //if (Vector2.Dot(RelativeTarget, transform.right) < 0)
        //    inputData.Horizontal = -1;

        inputData.Forward = 1;
        inputData.Turbo = false;

        return inputData;
    }

    Vector3 GetTarget()
    {
        Vector2Int GridPos = MapBuilder.Instance.PositionToGrid(car.EffectivePosition);
        CircuitBlock block = MapBuilder.Instance.Circuit[GridPos];

        return block.NextBlock.transform.position;
    }
}
