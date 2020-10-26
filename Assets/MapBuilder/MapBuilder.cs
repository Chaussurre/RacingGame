using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public static MapBuilder Instance;

    public int numberBlockAdvance; //number of blocks generated in front of 1st player
    public int numberBlockBehind; //number of blocks kept behind last player

    public CircuitBlock StraightLinePrefab;
    public CircuitBlock TurnPrefab;
    public Bumper BumperPrefab;
    public GameObject Blocker;

    public float BlockSize;

    private Vector2Int CurrentPosition = -Vector2Int.up;
    private CircuitBlock LastBlock = null;
    private Vector2Int CurrentOrientation = Vector2Int.up;

    private readonly Dictionary<Vector2Int, CircuitBlock> Circuit = new Dictionary<Vector2Int, CircuitBlock>();
    private readonly Dictionary<Vector2Int, Bumper> Bumpers = new Dictionary<Vector2Int, Bumper>();

    public static float DirectionToAngle(Vector2Int Direction)
    {
        float angle = -90 * Direction.x;
        if (Direction.y == -1)
            angle = 180;

        return angle;
    }
    public Vector2Int PositionToGrid(Vector2 Position)
    {
        return new Vector2Int(Mathf.RoundToInt(Position.x / BlockSize), Mathf.RoundToInt(Position.y / BlockSize));
    }

    private void Start()
    {
        Instance = this;
        CreateSection();
    }

    private void Update()
    {
        int dist = DistanceToEnd(PositionToGrid(GameManager.Instance.FindFirstPlayer().EffectivePosition));
        if (dist != -1 && dist < numberBlockAdvance)
                CreateSection();

        dist = DistanceToEnd(PositionToGrid(GameManager.Instance.FindLastPlayer().EffectivePosition), true);
        if (dist != -1)
            for (int i = dist; i > numberBlockBehind; i--)
                DestroyBlock();
    }

    public int DistanceToEnd(Vector2Int target, bool fromBegining = false) //How many circuit block between the start/the end of the circuit and target
    { 
        int count = 0;

        CircuitBlock SearchBlock = Circuit[CurrentPosition];
        if (fromBegining)
            SearchBlock = LastBlock;

        while (SearchBlock != null && SearchBlock.GridPosition != target && count < 1000)
        {
            if((!fromBegining && SearchBlock == SearchBlock.PreviousBlock) || (fromBegining && SearchBlock == SearchBlock.NextBlock))
            {
                Debug.LogError("DistanceToEnd : Loop at : " + SearchBlock.GridPosition);
                break;
            }

            if (fromBegining)
                SearchBlock = SearchBlock.NextBlock;
            else
                SearchBlock = SearchBlock.PreviousBlock;
            count++;
        }

        if (SearchBlock != null)
        {
            Debug.Log("DistanceToEnd : Searching for position : " + target + " Found position : " + SearchBlock.GridPosition + " in " + count + " steps");
            return count;
        }
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
        Destroy(destroyed.gameObject);

        Blocker.transform.position = LastBlock.transform.position;
        Vector2 Direction = LastBlock.GridPosition - destroyed.GridPosition;
        Vector2Int DirectionInt = new Vector2Int(Mathf.RoundToInt(Direction.x), Mathf.RoundToInt(Direction.y));
        Blocker.transform.rotation = Quaternion.Euler(0, 0, DirectionToAngle(DirectionInt));
        
        Circuit.Remove(destroyed.GridPosition);

        if (Bumpers.TryGetValue(destroyed.GridPosition, out Bumper bumper))
        {
            Bumpers.Remove(destroyed.GridPosition);
            Destroy(bumper.gameObject);
        }

        LastBlock.PreviousBlock = null;
        destroyed.NextBlock = null;

    }
    CircuitBlock CreateStraight()
    {
        if (CheckBlock(CurrentPosition)) //Trying to override an existing block
            CreateBumper();

        Vector2 pos = new Vector2(CurrentPosition.x , CurrentPosition.y) * BlockSize;

        Quaternion angle = Quaternion.Euler(0, 0, 90 * CurrentOrientation.x);

        CircuitBlock block = Instantiate(StraightLinePrefab, pos, angle, transform);
        return block;
    }

    CircuitBlock CreateTurn(bool ToLeft)
    {
        if (CheckBlock(CurrentPosition)) //Trying to override an existing block
        {
            CreateBumper();
            return CreateStraight(); //No turning block right after a bump
        }

        Vector2 pos = new Vector2(CurrentPosition.x , CurrentPosition.y) * BlockSize;

        Quaternion angle = Quaternion.Euler(0, 0, DirectionToAngle(CurrentOrientation));

        CircuitBlock block = Instantiate(TurnPrefab, pos, angle, transform);
        if (!ToLeft) //Flip the turn on the horizontal Axis if the turn is to the right;
        {
            Vector3 scale = block.transform.localScale;
            scale = new Vector3(-scale.x, scale.y, scale.z);
            block.transform.localScale = scale;
        }

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
}
