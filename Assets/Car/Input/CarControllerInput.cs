using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarControllerInput : MonoBehaviour
{
    abstract public ControlInputData GetInput();
}
