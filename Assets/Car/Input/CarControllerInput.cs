using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarControllerInput : MonoBehaviour
{
    protected CarController car;

    virtual protected void Start()
    {
        car = GetComponentInParent<CarController>();
    }

    public ControlInputData GetInput()
    {
        ControlInputData inputData = new ControlInputData();
        inputData.Forward = GetForward();
        inputData.Horizontal = GetHorizontal();
        inputData.Turbo = GetTurbo();

        return inputData;
    }
    virtual protected float GetHorizontal()
    {
        Vector3 target = GetTargetPoint();
        Vector2 relativePos = target - car.transform.position;

        float Angle = 0;
        if (relativePos != Vector2.zero)  
            Angle = (Vector2.Angle(transform.right, relativePos) - 90) * -1;
        if (Mathf.Abs(Angle) > 10)
            return Mathf.Sign(Angle) * Mathf.Sign(Vector2.Dot(car.GetSpeed(), car.transform.up));
        return 0;
    }
    abstract protected Vector2 GetTargetPoint(); //Where is the car headed
    abstract protected bool GetTurbo();
    abstract protected float GetForward();
}
