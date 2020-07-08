// Ignores the obselete warnings for using Unity UNet
#pragma warning disable 0618
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 10;
    public float rotateSpeed = 90;

    Rigidbody rb;
    CinemachineClearShot mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public override void OnStartClient()
    {
        try
        {
            mainCamera = GameObject.FindObjectOfType<CinemachineClearShot>();
        }
        catch (Exception e) {
            Debug.LogError("Error finding main camera!");
            throw e;
        }

        mainCamera.Follow = this.transform;
        mainCamera.LookAt = this.transform;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        var forwardInput = Input.GetAxis("Vertical");
        var turnInput = Input.GetAxis("Horizontal");

        var moveForce = new Vector3(0, 0, forwardInput * moveSpeed * Time.deltaTime);
        var turnForce = new Vector3(0, turnInput * rotateSpeed * Time.deltaTime, 0);

        rb.AddRelativeTorque(turnForce);
        rb.AddRelativeForce(moveForce);
    }
}
