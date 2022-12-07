using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public LayerMask shootingMask;
    public GameObject bulletImpactEffect;

    public float timeBetweenShot;
    private float shotCounter;

    public float maxHeat = 10f, heatPerShot = 1f, coolRate = 4f, overHeatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated;

    public void ShootExecute()
    {
        if (overHeated)
        {
            heatCounter -= overHeatCoolRate * Time.deltaTime;
            if (heatCounter <= 0)
            {
                heatCounter = 0;
                overHeated = false;
            }

            return;
        }

        if (heatCounter > 0)
            heatCounter -= coolRate * Time.deltaTime;
        else heatCounter = 0;

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetMouseButton(0))
        {
            shotCounter -= Time.deltaTime;

            if (shotCounter <= 0)
            {
                Shoot();
            }
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

        //Reset ShotCounter
        shotCounter = timeBetweenShot;

        heatCounter += heatPerShot;
        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;
            overHeated = true;
        }
    }
}