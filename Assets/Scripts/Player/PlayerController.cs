// Ignores the obselete warnings for using Unity UNet
#pragma warning disable 0618

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;


public class PlayerController : NetworkBehaviour
{
    public ThirdPersonMover thirdPersonMover;
    public GunController gunControl;

    CinemachineVirtualCameraBase mainCamera;

    Vector2 moveInput;
    
    public override void OnStartLocalPlayer()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCameraBase>();
        mainCamera.Follow = this.transform;
        mainCamera.LookAt = this.transform;

        thirdPersonMover = new ThirdPersonMover(gameObject);
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
            thirdPersonMover.Move(moveInput);
        }
    }

    [Command]
    void CmdFire()
    {
        gunControl.Fire(isServer);
        Destroy(bullet, bulletLifeTime);
        NetworkServer.Spawn(bullet);
        RpcAcknowlegdeFire(bullet);

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
