using UnityEngine;
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

    public float muzzleDisplayTime;
    private float muzzleCounter;

    [Header("Recoil")]
    //Recoil
    [SerializeField]
    private bool isAiming;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float aimRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimRecoilZ;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    [Header("Guns")]
    //Guns
    public Gun[] allGuns;

    public int selectedGun;


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
        SetTargetRecoil();
        RecoilShoot();

        if (Physics.Raycast(ray, out RaycastHit hit, shootingMask))
        {
            // Debug.Log("We hit " + hit.collider.gameObject.name);
            PoolSpawn(bulletImpactEffect, hit.point + (hit.normal * 0.02f),
                Quaternion.LookRotation(hit.normal, Vector3.up));
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
        muzzleCounter = muzzleDisplayTime;
    }

    private void RecoilShoot()
    {
        // targetRotation = Vector3.Lerp(targetRotation, Vector3.up, returnSpeed * Time.deltaTime);
        // currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        // transform.localRotation = Quaternion.Euler(currentRotation);
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

    private void AmmoFill(float heatAmount)
    {
        UIController.I.weaponSlider.value = heatAmount / maxHeat;
    }
}