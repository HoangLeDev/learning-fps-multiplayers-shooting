using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public LayerMask shootingMask;
    public GameObject bulletImpactEffect;

    public void ShootExecute()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        Camera cam = Camera.main;
        //half x, half y, zero z (Middle of Camera)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit, shootingMask))
        {
            Debug.Log("We hit " + hit.collider.gameObject.name);
            var bulletImpactEffectInstance = Instantiate(bulletImpactEffect, hit.point + (hit.normal * 0.02f),
                Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(bulletImpactEffectInstance, 1f);
        }
    }
}