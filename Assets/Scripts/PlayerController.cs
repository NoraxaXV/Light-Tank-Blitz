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

    [Header("Bullet Properties")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] float bulletSpeed = 1000;
    [SerializeField] float bulletLifeTime = 10;
    [SerializeField] Transform gunTransform;

    Rigidbody rb;
    CinemachineVirtualCameraBase mainCamera;

    Vector2 moveInput;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public override void OnStartLocalPlayer()
    {
        
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCameraBase>();
        mainCamera.Follow = this.transform;
        mainCamera.LookAt = this.transform;

    }

    void Update()
    {
        if (isLocalPlayer)
        {
            moveInput = new Vector2(
                Input.GetAxis("Vertical"),
                Input.GetAxis("Horizontal"));
            bool wantToFire = Input.GetButtonDown("Fire1");

            if (wantToFire) {
                CmdFire();
            }
        }
    }

    void FixedUpdate()
    {
        if (moveInput.x != 0 || moveInput.y != 0)
        { 
            Vector3 moveForce = Vector3.forward * (moveInput.x * moveSpeed);
            float turnForce = moveInput.y * rotateSpeed;
            Move(moveForce, turnForce);
            CmdMove(moveForce, turnForce);
        }
    }

    [Command]
    void CmdFire()
    {
        GameObject bullet = Instantiate(
            bulletPrefab,
            bulletSpawnPos.position,
            bulletSpawnPos.rotation);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddRelativeForce(Vector3.forward * bulletSpeed, ForceMode.VelocityChange);
        Destroy(bullet, bulletLifeTime);

        NetworkServer.Spawn(bullet);
        RpcAcknowlegdeFire(bullet);

    }
    void Move(Vector3 moveForce, float turnForce)
    {
        rb.AddRelativeForce(moveForce * Time.deltaTime);
        transform.RotateAround(Vector3.up, turnForce * Time.deltaTime);
    }

    [Command]
    void CmdMove(Vector3 moveForce, float turnForce)
    {
        if (!isServer) return;
        Move(moveForce, turnForce);
        RpcSyncMoveOnClients(moveForce, turnForce);
    }

    [ClientRpc]
    void RpcSyncMoveOnClients(Vector3 moveForce, float turnForce)
    {
        if (isServer || isLocalPlayer) return;
        Move(moveForce, turnForce);
    }

    [ClientRpc]
    void RpcAcknowlegdeFire(GameObject bullet)
    {
        if (isServer) return;
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddRelativeForce(Vector3.forward * bulletSpeed, ForceMode.VelocityChange);
        gunTransform.LookAt(bullet.transform);
    }
}
