using UnityEngine;

public class RotatePlanet : RotateObject
{
    [SerializeField] private GameManager gameManager;

    protected override Vector3 GetRotationSpeed(float dt)
    {
        return base.GetRotationSpeed(dt) * gameManager.TimeController.GetSpeed();
    }
}
