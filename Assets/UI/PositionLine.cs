using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLine : MonoBehaviour
{
    private CarController followedCar = null;
    [SerializeField]
    private LayerMask RaycastMask;
    private SpriteRenderer renderer;
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    private void LateUpdate()
    {
        if (followedCar == null || followedCar.Bumped != null)
        {
            Hide(true);
            return;
        }

        Vector2Int CarGridPosition = MapBuilder.Instance.PositionToGrid(followedCar.EffectivePosition);
        if (!MapBuilder.Instance.Circuit.TryGetValue(CarGridPosition, out CircuitBlock block))
        {
            Hide(true);
            return;
        }


        block.GetProgress(followedCar.EffectivePosition);
        Vector2 Normal = block.GetNormal(followedCar.EffectivePosition);

        Vector2 Begining = Physics2D.Raycast(followedCar.EffectivePosition, Normal, 10f, RaycastMask).point;
        Vector2 End = Physics2D.Raycast(followedCar.EffectivePosition, -Normal, 10f, RaycastMask).point;

        Vector3 scale = new Vector3((End - Begining).magnitude, transform.localScale.y, transform.localScale.z);
        if (scale.x < 1 || scale.x > 9) //An error occured
            return;

        Hide(false);
        transform.localScale = scale;

        transform.position = Vector2.Lerp(Begining, End, 0.5f);
        float angle = Mathf.Rad2Deg * Mathf.Acos((End - Begining).normalized.x); //Vector2.Angle(Vector2.right, End - Begining);
        if ((End - Begining).y < 0)
            angle *= -1;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetFollowedCar(CarController car)
    {
        followedCar = car;
    }

    public void ResetFollowedCar()
    {
        followedCar = null;
    }

    public void Hide(bool value)
    {
        renderer.enabled = !value;
    }
}
