using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCar : MonoBehaviour
{
    public CarController MainCar;
    void Update()
    {
        if (MainCar != null)
            transform.position = MainCar.transform.position - Vector3.forward * 10;
    }
}
