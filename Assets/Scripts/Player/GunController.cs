using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunController
{
    public Transform gunTransform;

    [System.Serializable]
    public class BulletSettings
    {
        public GameObject prefab;
        public Transform spawnPos;
        public float speed = 1000;
        public float lifeTime = 10;
    }
    BulletSettings bulletSettings;

    public GameObject Fire(bool isServer)
    {
        GameObject bullet = GameObject.Instantiate(
           bulletSettings.prefab,
           bulletSettings.spawnPos.position,
           bulletSettings.spawnPos.rotation);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddRelativeForce(Vector3.forward * bulletSettings.speed, ForceMode.VelocityChange);

        return bullet;
    }

    
}
