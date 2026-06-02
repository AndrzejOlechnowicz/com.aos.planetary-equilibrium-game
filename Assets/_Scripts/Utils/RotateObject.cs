using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] protected Vector3 rotationSpeed = new Vector3(1, 5, 1);

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(GetRotationSpeed(Time.deltaTime));
    }

    protected virtual Vector3 GetRotationSpeed(float dt)
    {
        return rotationSpeed * dt;
    }
}
