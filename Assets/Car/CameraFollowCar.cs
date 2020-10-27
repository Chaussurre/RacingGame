using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCar : MonoBehaviour
{
    private CarController Followed;
    void Update()
    {
        if (Followed != null)
            transform.position = Followed.transform.position - Vector3.forward * 10;
    }

    public void SetFollowed(CarController Followed)
    {
        this.Followed = Followed;
    }
}
