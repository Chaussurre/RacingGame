using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public int numberBlock;

    public GameObject StraightLinePrefab;
    public GameObject TurnPrefab;
    public float BlockSize;

    private Vector2Int CurrentPosition = Vector2Int.zero;
    private Vector2Int CurrentOrientation = Vector2Int.up;

    private readonly Dictionary<Vector2Int, GameObject> Circuit = new Dictionary<Vector2Int, GameObject>();

    private void Start()
    {
        for (int i = 0; i < numberBlock; i++)
            createBlock();
    }

    void createBlock()
    {
        int choice = Random.Range(0, 3);
        switch(choice)
        {
            case 0:
                createStraight();
                return;
            case 1:
                createTurn(true);
                return;
            case 2:
                createTurn(false);
                return;
        }
    }
    void createStraight()
    {
        if(Circuit.ContainsKey(CurrentPosition)) //Trying to override an existing block
        {
            Debug.LogError("MapBuilder.createStraight : Circuit block already exists at coordinates : " + CurrentPosition);
            return;
        }

        Vector2 pos = new Vector2(CurrentPosition.x , CurrentPosition.y) * BlockSize;

        Quaternion angle = Quaternion.Euler(0, 0, 90 * CurrentOrientation.x);

        GameObject block = Instantiate(StraightLinePrefab, pos, angle, transform);
        Circuit.Add(CurrentPosition, block);
        CurrentPosition += CurrentOrientation;
    }

    void createTurn(bool ToLeft)
    {
        if (Circuit.ContainsKey(CurrentPosition)) //Trying to override an existing block
        {
            Debug.LogError("MapBuilder.createTurn : Circuit block already exists at coordinates : " + CurrentPosition);
            return;
        }

        Vector2 pos = new Vector2(CurrentPosition.x , CurrentPosition.y) * BlockSize;

        Quaternion angle = Quaternion.Euler(0, 0, -90 * CurrentOrientation.x);
        if (CurrentOrientation.y == -1)
            angle = Quaternion.Euler(0, 0, 180);


        GameObject block = Instantiate(TurnPrefab, pos, angle, transform);
        Circuit.Add(CurrentPosition, block);

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
        CurrentPosition += CurrentOrientation;
    }
}
