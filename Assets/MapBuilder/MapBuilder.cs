﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public static MapBuilder Instance;

    public int numberBlockAdvance; //number of blocks generated in front of 1st player

    public GameObject StraightLinePrefab;
    public GameObject TurnPrefab;
    public Bumper BumperPrefab;

    public float BlockSize;

    private Vector2Int CurrentPosition = -Vector2Int.up;
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
        createBlock();
    }

    private void Update()
    {
        int dist = DistanceToEnd(PositionToGrid(GameManager.Instance.FindFirstPlayer().EffectivePosition));
        for (int i = dist; i < numberBlockAdvance; i++)
            createBlock();
    }

    public int DistanceToEnd(Vector2Int target, bool fromBegining = false) //How many circuit block between the start/the end of the circuit and target
    { 
        int count = 0;

        CircuitBlock SearchBlock = Circuit[CurrentPosition];
        if (fromBegining)
            SearchBlock = Circuit[Vector2Int.zero];

        while (SearchBlock != null && SearchBlock.GridPosition != target && count < 1000)
        {
            if((!fromBegining && SearchBlock == SearchBlock.PreviousGridPosition) || (fromBegining && SearchBlock == SearchBlock.NextGridPosition))
            {
                Debug.LogError("DistanceToEnd : Loop at : " + SearchBlock.GridPosition);
                break;
            }

            if (fromBegining)
                SearchBlock = SearchBlock.NextGridPosition;
            else
                SearchBlock = SearchBlock.PreviousGridPosition;
            count++;
        }

        Debug.Log("DistanceToEnd : Searching for position : " + target + " Found position : " + SearchBlock.GridPosition + " in " + count + " steps");
        return count;
    }

    public bool CheckBlock(Vector2Int pos) // return true if there is a circuit block at specified coordinates
    {
        return Circuit.ContainsKey(pos);
    }

    void createBlock()
    {
        Vector2Int PreviousPosition = CurrentPosition;
        CurrentPosition += CurrentOrientation;
        Debug.Log("createBlock : creating at " + CurrentPosition);

        CircuitBlock circuitBlock = new CircuitBlock();


        int choice = Random.Range(0, 3);
        switch(choice)
        {
            case 0:
                circuitBlock.block = createStraight();
                break;
            case 1:
                circuitBlock.block = createTurn(true);
                break;
            case 2:
                circuitBlock.block = createTurn(false);
                break;
        }

        if (Circuit.TryGetValue(PreviousPosition, out CircuitBlock previous))
        {
            previous.NextGridPosition = circuitBlock;
            circuitBlock.PreviousGridPosition = previous;
        }
        else
        {
            Debug.Log("createBlock : no previous block at " + PreviousPosition);
            circuitBlock.PreviousGridPosition = null;
        }

        circuitBlock.GridPosition = CurrentPosition;
        Circuit.Add(CurrentPosition, circuitBlock);
    }
    GameObject createStraight()
    {
        if (CheckBlock(CurrentPosition)) //Trying to override an existing block
            CreateBumper();

        Vector2 pos = new Vector2(CurrentPosition.x , CurrentPosition.y) * BlockSize;

        Quaternion angle = Quaternion.Euler(0, 0, 90 * CurrentOrientation.x);

        GameObject block = Instantiate(StraightLinePrefab, pos, angle, transform);
        return block;
    }

    GameObject createTurn(bool ToLeft)
    {
        if (CheckBlock(CurrentPosition)) //Trying to override an existing block
        {
            CreateBumper();
            return createStraight(); //No turning block right after a bump
        }

        Vector2 pos = new Vector2(CurrentPosition.x , CurrentPosition.y) * BlockSize;

        Quaternion angle = Quaternion.Euler(0, 0, DirectionToAngle(CurrentOrientation));

        GameObject block = Instantiate(TurnPrefab, pos, angle, transform);
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
