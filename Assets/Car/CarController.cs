using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float SpeedMax;
    public float AccelSpeed;
    public float RotationSpeed;

    private Rigidbody2D Body;
    private Bumper Bumped = null;

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

        //If mid-air, stop here
        if (Bumped != null)
            return;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.TryGetComponent(out Bumper bumper))
            Bump(bumper);
    }

    void Bump(Bumper bumper)
    {
        if (Bumped != null)
            return;

        Bumped = bumper;
        Vector2 landingPosition = new Vector2(transform.position.x, transform.position.y) + 
            (Vector2.zero + bumper.StopingPoint - bumper.StartingPoint) * MapBuilder.instance.BlockSize;

        StartCoroutine("BumpRoutine", landingPosition);
    }

    IEnumerator BumpRoutine(Vector2 landingPos)
    {
        float Maxtime = 0.8f;
        float delay = 0.1f;
        float timer = Maxtime;

        gameObject.layer = LayerMask.NameToLayer("IgnorePhysics");

        while (timer > 0)
        {
            Body.velocity = (landingPos - new Vector2(transform.position.x, transform.position.y)) / timer;
            timer -= delay;
            yield return new WaitForSeconds(delay);
        }

        transform.position = landingPos;
        Body.velocity = (Vector2.zero + Bumped.Direction) * SpeedMax;
        gameObject.layer = LayerMask.NameToLayer("Default");
        Bumped = null;
    }
}
