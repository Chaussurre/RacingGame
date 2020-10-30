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
        
        if (car.Bumped != null)
            return inputData;

        Vector3 RelativeTarget = GetTarget() - transform.position;

        inputData.Horizontal = 1;
        if (Vector2.Dot(RelativeTarget, transform.right) < 0)
            inputData.Horizontal = -1;

        inputData.Forward = 1;
        inputData.Turbo = false;

        return inputData;
    }

    Vector3 GetTarget()
    {

        Vector2Int GridPos = MapBuilder.Instance.PositionToGrid(car.EffectivePosition);
        if (MapBuilder.Instance.Circuit.TryGetValue(GridPos, out CircuitBlock block))
            return block.NextBlock.transform.position;

        return car.transform.position + car.transform.up;
    }
}
