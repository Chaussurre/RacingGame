using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float SpeedMax;
    public float AccelSpeed;
    public float RotationSpeed;

    private Rigidbody2D Body;

    float Speed = 0;
    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 Acceleration = transform.up * Time.fixedDeltaTime * AccelSpeed * Input.GetAxis("Vertical");
        float Rotation = Speed * RotationSpeed * Time.fixedDeltaTime * RotationSpeed * Input.GetAxis("Horizontal");
        bool AccelerationIsPositive = Vector2.Dot(Acceleration, Body.velocity) > 0;

        //rotate
        Body.MoveRotation(Body.rotation - Rotation);

        //If speed is less than speedMax, accelerate the car
        if ((Speed >= 0 && (Speed < SpeedMax || !AccelerationIsPositive)) ||
            (Speed <= 0 && (Speed > -SpeedMax || !AccelerationIsPositive)))
            Body.AddForce(Acceleration);

        //Natural Deceleration
        if (Acceleration.magnitude < 0.1 || !AccelerationIsPositive)
            Body.velocity = Body.velocity * 0.8f;

        Speed = Vector2.Dot(Body.velocity, transform.up);

        //anti-drift
        Body.velocity = transform.up * Speed;
    }
}
