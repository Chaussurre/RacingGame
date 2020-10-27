﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float SpeedMax;
    public float AccelSpeed;
    public float RotationSpeed;
    public float SlowRotationSpeed;
    public float DriftFactor;

    public Color color;

    public Vector2 EffectivePosition { get; private set; } //The car's position in the circuit that takes bumper into account

    [SerializeField]
    private Transform CarFont;
    [SerializeField]
    private SpriteRenderer ColorCarRenderer;
    private Rigidbody2D Body;
    public Bumper Bumped { get; private set; } = null;

    Vector2 PreviousSpeed;

    float Speed = 0;
    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        ColorCarRenderer.color = color;
    }

    private void Update()
    {
        if (Bumped == null)
            EffectivePosition = CarFont.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PreviousSpeed = Body.velocity;

        //If mid-air, stop here
        if (Bumped != null)
            return;


        Rotate();
        Accelerate();
        Drift();
    }

    void Rotate()
    {
        float RotationSpeed = this.RotationSpeed;
        if (Body.velocity.magnitude < 4)
            RotationSpeed = SlowRotationSpeed;
        float Rotation = Speed * RotationSpeed * Time.fixedDeltaTime * RotationSpeed * Input.GetAxis("Horizontal");
        Body.MoveRotation(Body.rotation - Rotation);

    }

    void Accelerate()
    { 
        Vector2 Acceleration = transform.up * Time.fixedDeltaTime * AccelSpeed * Input.GetAxis("Vertical");
        bool AccelerationIsPositive = Vector2.Dot(Acceleration, Body.velocity) > 0;
        //If speed is less than speedMax, accelerate the car
        if ((Speed >= 0 && (Speed < SpeedMax || !AccelerationIsPositive)) ||
            (Speed <= 0 && (Speed > -SpeedMax || !AccelerationIsPositive)))
            Body.AddForce(Acceleration);

        //Natural Deceleration
        if (Acceleration.magnitude < 0.1 || !AccelerationIsPositive)
            Body.velocity *= 0.95f;

    }

   void Drift()
    {
        Speed = Vector2.Dot(Body.velocity, transform.up);
        Vector3 DriftSpeed = new Vector3(Body.velocity.x, Body.velocity.y) - (transform.up * Speed);
        Body.velocity = transform.up * Speed + (DriftSpeed * DriftFactor);
    }
    void Bump(Bumper bumper)
    {
        if (Bumped != null)
            return;

        Bumped = bumper;
        Vector2 landingPosition = new Vector2(transform.position.x, transform.position.y) + 
            (Vector2.zero + bumper.StopingPoint - bumper.StartingPoint) * MapBuilder.Instance.BlockSize;

        transform.rotation = Quaternion.Euler(0, 0, MapBuilder.DirectionToAngle(bumper.Direction));

        StartCoroutine("BumpRoutine", landingPosition);
    }

    IEnumerator BumpRoutine(Vector2 landingPos)
    {
        float Maxtime = 0.8f;
        float delay = 0.1f;
        float timer = Maxtime;

        gameObject.layer = LayerMask.NameToLayer("IgnorePhysics");

        EffectivePosition = landingPos;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.TryGetComponent(out Bumper bumper))
            Bump(bumper);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 CollisionNormal = collision.contacts[0].normal;
        float CollisionAngle = Mathf.Abs(Vector2.Angle(PreviousSpeed, CollisionNormal));

        if (CollisionAngle > 130) //Orthogonal collision, number found from observation
            Body.velocity = -.8f * PreviousSpeed;


    }
}
