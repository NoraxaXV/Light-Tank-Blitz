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

    Rigidbody rb;
    CinemachineVirtualCameraBase mainCamera;

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

            float forwardInput = Input.GetAxis("Vertical");
            float turnInput = Input.GetAxis("Horizontal");
            bool wantToFire = Input.GetButtonDown("Fire1");

            float moveForce = forwardInput * moveSpeed * Time.deltaTime;
            float turnForce = turnInput * rotateSpeed * Time.deltaTime;

            rb.AddRelativeForce(Vector3.forward * moveForce);
            transform.RotateAround(Vector3.up, turnForce);

            if (wantToFire) {
                CmdFire();
            }
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

    [ClientRpc]
    void RpcAcknowlegdeFire(GameObject bullet)
    {
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddRelativeForce(Vector3.forward * bulletSpeed, ForceMode.VelocityChange);
    }
}
