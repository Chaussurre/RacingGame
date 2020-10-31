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

    abstract public ControlInputData GetInput();
}
