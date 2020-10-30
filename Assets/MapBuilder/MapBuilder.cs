using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public static MapBuilder Instance;

    public int numberBlockAdvance; //number of blocks generated in front of 1st player
    public int numberBlockBehind; //number of blocks kept behind last player

    public CircuitBlock StraightLinePrefab;
    public TurnBlock TurnPrefab;
    public Bumper BumperPrefab;
    public GameObject Blocker;

    public float BlockSize;

    public MiniMap miniMap { get; private set; }
    private Vector2Int CurrentPosition = -Vector2Int.up;
    public CircuitBlock LastBlock { get; private set; } = null;
    private Vector2Int CurrentOrientation = Vector2Int.up;

    public readonly Dictionary<Vector2Int, CircuitBlock> Circuit = new Dictionary<Vector2Int, CircuitBlock>();
    private readonly Dictionary<Vector2Int, Bumper> Bumpers = new Dictionary<Vector2Int, Bumper>();

    public static float DirectionToAngle(Vector2Int Direction)
    {
        /*
        float angle = -90 * Direction.x;
        if (Direction.y == -1)
            angle = 180;

        return angle;
        */
        Vector2 vec = Direction;
        return DirectionToAngle(vec);
    }
    public static float DirectionToAngle(Vector2 Direction)
    {
        float angle = Mathf.Acos(Direction.x) * Mathf.Rad2Deg - 90;
        if (Direction.y < 0)
        {
            angle *= -1;
            angle += 180;
        }

        return angle;
    }
    public Vector2Int PositionToGrid(Vector2 Position)
    {
        return new Vector2Int(Mathf.RoundToInt(Position.x / BlockSize), Mathf.RoundToInt(Position.y / BlockSize));
    }

    private void Awake()
    {
        Instance = this;
        miniMap = FindObjectOfType<MiniMap>();
        CreateSection();
    }

    private void Update()
    {
        int dist = DistanceToEnd(PositionToGrid(GameManager.Instance.FindFirstPlayer().EffectivePosition));
        if (dist != -1 && dist < numberBlockAdvance)
                CreateSection();

        dist = DistanceToBegining(PositionToGrid(GameManager.Instance.FindLastPlayer().EffectivePosition));
        if (dist != -1)
            for (int i = dist; i > numberBlockBehind; i--)
                DestroyBlock();
    }

    public int DistanceToEnd(Vector2Int target) //How many circuit block between the start/the end of the circuit and target
    { 
        if (Circuit.TryGetValue(target, out CircuitBlock block))
            return block.Order;

        return -1;
    }

    public int DistanceToBegining(Vector2Int target)
    {
        if (Circuit.TryGetValue(target, out CircuitBlock block))
            return LastBlock.Order - block.Order;

        return -1;
    }

    public bool CheckBlock(Vector2Int pos) // return true if there is a circuit block at specified coordinates
    {
        return Circuit.ContainsKey(pos);
    }

    void CreateSection()
    {
        int numberStraight = Random.Range(4, 7);
        int clusterSize = Random.Range(12, 21);

        for (int i = 0; i < numberStraight; i++)
            CreateBlock(true);
        for (int i = 0; i < clusterSize; i++)
            CreateBlock(false);

        ReorderBlocks();
    }

    void CreateBlock(bool straight)
    {
        Vector2Int PreviousPosition = CurrentPosition;
        CurrentPosition += CurrentOrientation;
        Debug.Log("createBlock : creating at " + CurrentPosition);


        CircuitBlock circuitBlock = ChooseBlock(straight);

        if (Circuit.TryGetValue(PreviousPosition, out CircuitBlock previous))
        {
            previous.NextBlock = circuitBlock;
            circuitBlock.PreviousBlock = previous;
        }
        else
        {
            Debug.Log("createBlock : no previous block at " + PreviousPosition);
            circuitBlock.PreviousBlock = null;
        }

        Circuit.Add(CurrentPosition, circuitBlock);
        miniMap.CreateBlock(circuitBlock);

        if (LastBlock == null)
            LastBlock = circuitBlock;
    }

    CircuitBlock ChooseBlock(bool straight)
    {
        if (straight)
            return CreateStraight();

        int choice = Random.Range(0, 2);
        switch (choice)
        {
            case 0:
                return CreateTurn(false);
            case 1:
                return CreateTurn(true);
        }
        return null;
    }

    void DestroyBlock()
    {
        CircuitBlock destroyed = LastBlock;
        LastBlock = destroyed.NextBlock;

        Blocker.transform.position = LastBlock.transform.position;
        Blocker.transform.rotation = LastBlock.transform.rotation;
        
        Circuit.Remove(destroyed.GridPosition);

        if (Bumpers.TryGetValue(destroyed.GridPosition, out Bumper bumper))
        {
            Bumpers.Remove(destroyed.GridPosition);
            Destroy(bumper.gameObject);
        }

        LastBlock.PreviousBlock = null;

        miniMap.DestroyBlock(destroyed);
        Destroy(destroyed.gameObject);
    }
    CircuitBlock CreateStraight()
    {
        if (CheckBlock(CurrentPosition)) //Trying to override an existing block
            CreateBumper();


        CircuitBlock block = Instantiate(StraightLinePrefab, transform);
        block.Init(CurrentPosition, CurrentOrientation);

        return block;
    }

    CircuitBlock CreateTurn(bool ToLeft)
    {
        if (CheckBlock(CurrentPosition)) //Trying to override an existing block
        {
            CreateBumper();
            return CreateStraight(); //No turning block right after a bump
        }

        TurnBlock block = Instantiate(TurnPrefab, transform);
        block.Init(CurrentPosition, CurrentOrientation);
        block.SetToLeft(ToLeft);

        if (ToLeft)
            CurrentOrientation = new Vector2Int(-CurrentOrientation.y, CurrentOrientation.x);
        else
            CurrentOrientation = new Vector2Int(CurrentOrientation.y, -CurrentOrientation.x);

        return block;
    }

    void CreateBumper()
    {
        Bumper bumper = Instantiate(BumperPrefab, transform);
        Bumpers.Add(CurrentPosition - CurrentOrientation, bumper);
        CurrentPosition = bumper.Init(CurrentPosition, CurrentOrientation);
    }

    void ReorderBlocks()
    {
        CircuitBlock block = Circuit[CurrentPosition];
        int count = 0;
        while(block != null)
        {
            block.SetOrder(count);
            block = block.PreviousBlock;
            count++;
        }
    }
}
