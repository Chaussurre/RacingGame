﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float SpeedMax;
    public float TurboSpeedMax;
    public float AccelSpeed;
    public float TurboAccelSpeed;
    public float RotationSpeed;
    public float SlowRotationSpeed;
    public float MinDriftFactor;
    public float MaxDriftFactor;
    public Color color;

    public Vector2 EffectivePosition { get; private set; } //The car's position in the circuit that takes bumper into account

    [SerializeField]
    private Transform CarFront;
    [SerializeField]
    private SpriteRenderer ColorCarRenderer;
    private Rigidbody2D Body;
    private CarControllerInput ControllerInput;
    public Bumper Bumped { get; private set; } = null;
    public int score { get; private set; } = 0;
    bool respawning = false;

    Vector2 PreviousSpeed;

    public float CarSize { get; private set; }

    float SpeedForward = 0;
    void Start()
    {
        CarSize = (CarFront.position - transform.position).magnitude * 2;
        Body = GetComponent<Rigidbody2D>();
        ControllerInput = GetComponentInChildren<CarControllerInput>();
        SetColor(color);
    }

    private void Update()
    {
        if (Bumped == null)
            EffectivePosition = CarFront.position;
    }

    void FixedUpdate()
    {
        PreviousSpeed = Body.velocity;
        ControlInputData inputData = ControllerInput.GetInput();

        //If mid-air or respawning, stop here
        if (Bumped != null || respawning)
            return;


        Rotate(inputData);
        Accelerate(inputData);
        Drift();
    }

    void Rotate(ControlInputData inputData)
    {
        float RotationSpeed = this.RotationSpeed;
        if (Body.velocity.magnitude < 4)
            RotationSpeed = SlowRotationSpeed;
        float Rotation = SpeedForward * RotationSpeed * Time.fixedDeltaTime * RotationSpeed * inputData.Horizontal;
        Body.MoveRotation(Body.rotation - Rotation);

    }

    void Accelerate(ControlInputData inputData)
    {
        float EffectiveAccelSpeed = AccelSpeed;
        float EffectiveSpeedMax = SpeedMax;

        if (inputData.Turbo)
        {
            EffectiveAccelSpeed = TurboAccelSpeed;
            EffectiveSpeedMax = TurboSpeedMax;
        }

        Vector2 Acceleration = transform.up * Time.fixedDeltaTime * EffectiveAccelSpeed * inputData.Forward;
        bool AccelerationIsPositive = Vector2.Dot(Acceleration, Body.velocity) > 0;
        //If speed is less than speedMax, accelerate the car
        if ((SpeedForward >= 0 && (SpeedForward < EffectiveSpeedMax || !AccelerationIsPositive)) ||
            (SpeedForward <= 0 && (SpeedForward > -EffectiveSpeedMax || !AccelerationIsPositive)))
            Body.AddForce(Acceleration);

        //Natural Deceleration
        if (Acceleration.magnitude < 0.1 || !AccelerationIsPositive || SpeedForward > EffectiveSpeedMax)
            Deccelerate();
    }

    void Deccelerate()
    {
        Body.velocity *= 0.95f;
    }

    void Drift()
    {
        SpeedForward = Vector2.Dot(Body.velocity, transform.up);

        float Speed = Body.velocity.magnitude;
        float driftFactor = MinDriftFactor;
        if (Speed > SpeedMax)
        {
            if (Speed >= TurboSpeedMax)
                driftFactor = MaxDriftFactor;
            else
            {
                float ratio = (Speed - SpeedMax) / (TurboSpeedMax - SpeedMax);
                driftFactor = ratio * (MaxDriftFactor - MinDriftFactor) + MinDriftFactor;
            }
        }

        Vector2 DriftSpeed = new Vector3(Body.velocity.x, Body.velocity.y) - (transform.up * SpeedForward);
        Body.velocity -= DriftSpeed * (1 - driftFactor) * 10 * Time.fixedDeltaTime;
    }
    void Bump(Bumper bumper)
    {
        if (Bumped != null)
            return;

        Bumped = bumper;
        Vector2 BumperDist = (Vector2.zero + bumper.StopingPoint - bumper.StartingPoint) * MapBuilder.Instance.BlockSize;
        Vector2 landingPosition = new Vector2(transform.position.x, transform.position.y) + BumperDist +
            ((Vector2.zero + bumper.Direction) * CarSize * 2f);

        transform.rotation = Quaternion.Euler(0, 0, MapBuilder.DirectionToAngle(bumper.Direction));

        StartCoroutine("BumpRoutine", landingPosition);
    }

    IEnumerator BumpRoutine(Vector2 landingPos)
    {
        float Maxtime = 0.8f;
        float timer = Maxtime;

        gameObject.layer = LayerMask.NameToLayer("IgnorePhysics");

        EffectivePosition = landingPos;

        while (timer > 0)
        {
            Debug.DrawLine(transform.position, landingPos, Color.cyan);
            Body.velocity = (landingPos - new Vector2(transform.position.x, transform.position.y)) / timer;
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.position = landingPos;
        Body.velocity = (Vector2.zero + Bumped.Direction) * SpeedMax;
        gameObject.layer = LayerMask.NameToLayer("Default");
        Bumped = null;
    }

    IEnumerator ScoreRespawnRoutine(float GhostTimer)
    {
        respawning = true;
        Bumped = null;
        gameObject.layer = LayerMask.NameToLayer("Default");

        //Dash fade part

        //GhostMode part
        CarController lastPlayer = GameManager.Instance.FindLastPlayer();
        Vector2Int GridPos = MapBuilder.Instance.PositionToGrid(lastPlayer.EffectivePosition);

        if (MapBuilder.Instance.Circuit.TryGetValue(GridPos, out CircuitBlock block))
            block = block.PreviousBlock;

        if (block == null)
            block = MapBuilder.Instance.LastBlock;

        transform.position = block.transform.position;

        gameObject.layer = LayerMask.NameToLayer("Ghost");
        Color faded = new Color(color.r, color.g, color.b, 0.5f);
        SetColor(faded);
        
        while (GhostTimer > 0)
        {
            Body.velocity = Vector2.zero;
            GhostMode();
            GhostTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        gameObject.layer = LayerMask.NameToLayer("Default");
        SetColor(color);
        respawning = false;
    }

    void GhostMode()
    {
        if (Bumped != null)
            return;

        Vector2Int GridPos = MapBuilder.Instance.PositionToGrid(transform.position);
        if (!MapBuilder.Instance.Circuit.TryGetValue(GridPos, out CircuitBlock block))
            return;

        Vector3 Parrallel = block.GetProjectedParrallel(transform.position, out Vector3 Projection);

        transform.rotation = Quaternion.Euler(0, 0, MapBuilder.DirectionToAngle(Parrallel));
        transform.position = Projection + Parrallel * SpeedMax * Time.fixedDeltaTime;

        Debug.DrawRay(transform.position, Parrallel, Color.red);
    }

    private void SetColor(Color color)
    {
        ColorCarRenderer.color = color;
        Color alphaWhite = new Color(Color.white.r, Color.white.g, Color.white.b, color.a);
        GetComponent<SpriteRenderer>().color = alphaWhite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.TryGetComponent(out Bumper bumper))
            Bump(bumper);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CarController _))
            return;

        Vector2 CollisionNormal = collision.contacts[0].normal;
        float CollisionAngle = Mathf.Abs(Vector2.Angle(transform.up, CollisionNormal));

        if (CollisionAngle > 150) //Orthogonal collision, number found from observation
            Body.velocity = -.8f * PreviousSpeed;


    }

    public void Score(float GhostTimer)
    {
        score++;
        Debug.Log("Scoring for car : " + color + " score = " + score);

        StopAllCoroutines();
        StartCoroutine("ScoreRespawnRoutine", GhostTimer);
    }

    public Vector2 GetSpeed()
    {
        return Body.velocity;
    }
}
