using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThirdPersonMover
{

    public float moveSpeed = 10;
    public float rotateSpeed = 90;
    readonly Rigidbody rb;
    readonly Transform transform;

    public ThirdPersonMover(GameObject obj)
    {
        rb = obj.GetComponent<Rigidbody>();
        transform = obj.transform;
    }

    public void Move(Vector2 moveInput)
    {
        Vector3 moveForce = Vector3.forward * (moveInput.x * moveSpeed);
        float turnForce = moveInput.y * rotateSpeed;

        if (rb != null)
        {
            rb.AddRelativeForce(moveForce * Time.deltaTime);
        }
        if (transform != null)
        {
            transform.Rotate(Vector3.up, turnForce * Time.deltaTime);
        }
    }
}
