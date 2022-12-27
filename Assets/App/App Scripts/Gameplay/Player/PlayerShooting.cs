using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerShooting : PoolManager
{
    public LayerMask shootingMask;
    public GameObject bulletImpactEffect;

    // public float timeBetweenShot;
    private float shotCounter;

    public float maxHeat = 10f, /*heatPerShot = 1f,*/ coolRate = 4f, overHeatCoolRate = 5f;
    public float heatCounter;
    private bool overHeated;

    //Effect Counter
    public float muzzleDisplayTime;
    private float muzzleCounter;
    public float bulletDisplayTime;
    private float bulletCounter;

    [SerializeField]
    private LineRenderer bulletLineRenderer;
    [SerializeField]
    private GameObject bulletLinePrefab;
    private GameObject currentPlayerBulletLine;

    public GameObject playerHitImpact;

    [Header("Recoil")]
    [SerializeField]
    private bool isAiming;
    private Vector3 initialGunPosition;

    private Vector3 currentRotation, targetRotation;
    private Vector3 currentPosition, targetPosition;

    [SerializeField]
    private float recoilX;
    [SerializeField]
    private float recoilY;
    [SerializeField]
    private float recoilZ;
    [SerializeField]
    private float kickBackZ;

    [SerializeField]
    private float aimRecoilX;
    [SerializeField]
    private float aimRecoilY;
    [SerializeField]
    private float aimRecoilZ;

    [SerializeField]
    private float snappiness;
    [SerializeField]
    private float returnSpeed;

    [Header("Guns")]
    public Gun[] allGuns;
    public int selectedGun;

    private void OnEnable()
    {
        if (photonView.IsMine)
        {
            initialGunPosition = allGuns[selectedGun].transform.position;
            InitBulletLine();
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            bulletLineRenderer = null;
            Destroy(currentPlayerBulletLine);
        }
    }

    #region SHOOT

    private void InitBulletLine()
    {
        currentPlayerBulletLine = Instantiate(bulletLinePrefab, Vector3.zero, Quaternion.identity);
        bulletLineRenderer = currentPlayerBulletLine.GetComponent<LineRenderer>();
    }

    public void ShootExecute()
    {
        if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;
            if (muzzleCounter <= 0)
            {
                muzzleCounter = 0;
                allGuns[selectedGun].muzzleFlash.SetActive(false);
            }
        }

        if (bulletLineRenderer.gameObject.activeInHierarchy)
        {
            bulletCounter -= Time.deltaTime;
            if (bulletCounter <= 0)
            {
                bulletCounter = 0;
                bulletLineRenderer.gameObject.SetActive(false);
            }
        }

        AmmoFill(heatCounter);
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
        else
        {
            heatCounter = 0;
            UIController.I.overheatedText.gameObject.SetActive(false);
        }

        //Counter between per shot
        shotCounter -= Time.deltaTime;

        //Mouse Shoot
        if (Input.GetMouseButtonDown(0))
        {
            if (shotCounter <= 0)
            {
                Shoot();
            }
        }

        if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
        {
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
        // SetTargetRecoil();
        // RecoilShoot();


        Vector3 hitPoint = Vector3.zero; // Cover when player shoot to the Sky

        if (Physics.Raycast(ray, out RaycastHit hit, shootingMask))
        {
            hitPoint = hit.point;
            // Debug.Log("We hit " + hit.collider.gameObject.name);

            if (hit.collider.gameObject.tag == "Player")
            {
                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName);
            }
            else
            {
                PoolSpawn(bulletImpactEffect, hit.point + (hit.normal * 0.02f),
                    Quaternion.LookRotation(hit.normal, Vector3.up));
            }

            StartRecoil();
        }
        else
        {
            /*
             * Note:
             * 1. transform.forward always return a const of the normalized vector3 to the z from
             * the 1st place of OBJ in World space, it will never change.
             * 2. To take the realtime transform.forward, need to add the OBJ.transform.position
             */

            // Cover when player shoot to the Sky
            hitPoint = cam.transform.position + (cam.transform.forward * 30f);
        }


        //Reset ShotCounter
        shotCounter = allGuns[selectedGun].timeBetweenShot;
        heatCounter += allGuns[selectedGun].heatPerShot;

        if (heatCounter >= maxHeat)
        {
            AmmoFill(heatCounter);
            UIController.I.overheatedText.gameObject.SetActive(true);
            heatCounter = maxHeat;
            overHeated = true;
        }

        allGuns[selectedGun].muzzleFlash.SetActive(true);

        //Effect
        bulletLineRenderer.gameObject.SetActive(true);
        bulletLineRenderer.SetPosition(0, allGuns[selectedGun].muzzleFlash.transform.position);
        bulletLineRenderer.SetPosition(1, hitPoint);
        muzzleCounter = muzzleDisplayTime;
        bulletCounter = bulletDisplayTime;
    }

    private void AmmoFill(float heatAmount)
    {
        UIController.I.weaponSlider.value = heatAmount / maxHeat;
    }

    #endregion

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snappiness);
        Camera.current.transform.localRotation = Quaternion.Euler(currentRotation);
        BackRecoil();
    }

    #region RECOIL

    private void RecoilShoot()
    {
        // targetRotation = Vector3.Lerp(targetRotation, Vector3.up, returnSpeed * Time.deltaTime);
        // currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        // transform.localRotation = Quaternion.Euler(currentRotation);
    }

    private void StartRecoil()
    {
        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilX, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    private void BackRecoil()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnSpeed);
        targetRotation = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        Camera.current.transform.localPosition = currentPosition;
    }

    private void SetTargetRecoil()
    {
        if (isAiming)
            targetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY),
                Random.Range(-aimRecoilZ, aimRecoilZ));
        else
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY),
                Random.Range(-recoilZ, recoilZ));
    }

    #endregion

    #region DAMAGE

    [PunRPC]
    public void DealDamage(string attacker)
    {
        TakeDamage(attacker);
    }

    public void TakeDamage(string attacker)
    {
        if (photonView.IsMine)
        {
            PlayerSpawner.I.PlayerDie(attacker);
        }
    }

    #endregion
}