using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float SpeedMax;
    public float AccelSpeed;
    public float RotationSpeed;

    private Rigidbody2D Body;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float Speed = Vector2.Dot(Body.velocity, transform.up);
        Vector2 Acceleration = transform.up * Time.fixedDeltaTime * AccelSpeed * Input.GetAxis("Vertical");
        float Rotation = Speed * RotationSpeed * Time.fixedDeltaTime * RotationSpeed * Input.GetAxis("Horizontal");
        bool AccelerationIsPositive = Vector2.Dot(Acceleration, Body.velocity) > 0;

        //If speed is less than speedMax, accelerate the car
        if ((Speed >= 0 && (Speed < SpeedMax || !AccelerationIsPositive)) ||
            (Speed <= 0 && (Speed > -SpeedMax || !AccelerationIsPositive)))
            Body.AddForce(Acceleration);

        //Natural Deceleration
        if (Acceleration.magnitude < 0.1 || !AccelerationIsPositive)
            Body.velocity = Body.velocity * 0.99f;

        //rotate
        Body.MoveRotation(Body.rotation - Rotation);

        //anti-drift
        if (Input.GetAxisRaw("Brake") == 0)
            if (Speed > 0)
                Body.velocity = Body.velocity.magnitude * transform.up;
            else
                Body.velocity = Body.velocity.magnitude * -transform.up;
    }
}
