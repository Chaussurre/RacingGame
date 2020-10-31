using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : CarControllerInput
{
    override protected void Start()
    {
        base.Start();
        GameManager.Instance.SetMainPlayer(car);
    }

    public override ControlInputData GetInput()
    {
        Vector2 Center = Camera.main.transform.position;

        Vector2 pointed = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        ControlInputData inputData = new ControlInputData();

        if (Input.GetMouseButton(0))
        {
            inputData.Forward = 1;
            if (Input.GetMouseButton(1))
                inputData.Turbo = true;
        }
        else if (Input.GetMouseButton(1))
            inputData.Forward = -1;

        float Angle = (Vector2.Angle(transform.right, pointed - Center) - 90) * -1;
        Debug.Log("going back : " + Mathf.Sign(Vector2.Dot(car.GetSpeed(), car.transform.up)));
        if (Mathf.Abs(Angle) > 10)
            inputData.Horizontal = Mathf.Sign(Angle) * Mathf.Sign(Vector2.Dot(car.GetSpeed(), car.transform.up));

        return inputData;
    }
}
