using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{

    public bool isActiveWeapon;

    public int weaponDamage;

    public Camera playerCamera;
    public Camera weaponCamera;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 100;
    public float bulletPrefabLifeTime = 3f;
    // Start is called before the first frame update

    public bool isShooting = false;
    public float shootingDelay = 1f;
    public bool readyToShoot = true;
    public bool allowReset = true;

    public float firingRate = 0.1f;
    public bool isAuto = false;

    public int bulletPerBurst = 3;
    public int currBurst;

    public bool isArms;
    public GameObject arms;

    public float spreadIntensity = 0.01f;

    public float reloadingTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public enum WeaponModel
    {
        M4A1,
        FN57,
        Baretta,
        Grenade
    }

    public enum WeaponType
    {
        PRIMARY = 0,
        SECONDARY = 1,
        GRENADE = 2,
        SMOKE = 3,
        FLASHBANG = 4
    }

    public WeaponModel model;
    public WeaponType type;
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public Vector3 spawnPosition;
    public Vector3 spawnRotation; 

    public ShootingMode shootingMode;

    public GameObject muzzleEffect;

    public Animator animator;
    public Animator ADSAnimator;
    private bool isADSable;

    private void Awake()
    {
        readyToShoot = true;
        currBurst = bulletPerBurst;
        bulletsLeft = magazineSize;
        isADSable = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (isActiveWeapon)
        {

            if (isActiveWeapon)
            {
                foreach(Transform child in transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
                }
            }
            if (Input.GetMouseButtonDown(1) && isADSable && InputManager.instance.isADSable) {
                ToggleADS();
            }
            if (InputManager.instance.shooting)
            {
                if (shootingMode == ShootingMode.Auto)
                {
                    isShooting = Input.GetKey(KeyCode.Mouse0);
                    if (readyToShoot && isShooting && bulletsLeft != 0 && !isReloading)
                    {
                        if (!isAuto)
                        {
                            isAuto = true;
                            animator.SetBool("Shoot", true);
                            InvokeRepeating("FireWeapon", 0f, firingRate);
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.Mouse0) || bulletsLeft == 0)
                    {
                        CancelInvoke();
                        Invoke("ResetShot", shootingDelay);
                        animator.SetBool("Shoot", false);
                        isAuto = false;
                    }
                }
                else
                {
                    isShooting = Input.GetKeyDown(KeyCode.Mouse0);
                    if (readyToShoot && isShooting && bulletsLeft != 0 && !isReloading)
                    {
                        currBurst = bulletPerBurst;
                        animator.SetBool("Shoot", true);
                        FireWeapon();
                    }
                }
            }
            if (bulletsLeft == 0)
            {
                readyToShoot = true;
                isShooting = false;

            }
            if (InputManager.instance.reloading)
            {
                if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading && WeaponManager.instance.CheckAmmoLeft(model) > 0)
                {
                    Reload();
                }
            }
        } else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    private void ToggleADS()
    {
        isADSable = false;
        ADSAnimator.SetBool("IsADS", !ADSAnimator.GetBool("IsADS"));
        if (ADSAnimator.GetBool("IsADS"))
        {
            playerCamera.GetComponent<Camera>().fieldOfView = 40;
            weaponCamera.GetComponent<Camera>().fieldOfView = 40;
            HUDManager.instance.crosshair.SetActive(false);
            spreadIntensity /= 2;
        } else
        {
            playerCamera.GetComponent<Camera>().fieldOfView = 60;
            weaponCamera.GetComponent<Camera>().fieldOfView = 60;
            HUDManager.instance.crosshair.SetActive(true);
            spreadIntensity *= 2;
        }
        StartCoroutine(ResetADS());
    }

    private IEnumerator ResetADS()
    {
        yield return new WaitForSeconds(1);
        isADSable = true;
    }

    private void Reload()
    {
        isReloading = true;
        SoundManager.instance.PlayReloadingSound(model);
        animator.SetTrigger("Reload");
        StartCoroutine(ReloadCompleted(reloadingTime));
    }

    private IEnumerator ReloadCompleted(float delay)
    {
        yield return new WaitForSeconds(delay);
        isReloading = false;
        int oldBullet = bulletsLeft;
        bulletsLeft = Math.Min(magazineSize, oldBullet + WeaponManager.instance.CheckAmmoLeft(model));
        WeaponManager.instance.DecreaseTotalAmmo(model, oldBullet - bulletsLeft);
    }
    private void FireWeapon()
    {
        bulletsLeft--;
        readyToShoot = false;
        animator.SetTrigger("Fire");
        Vector3 shootingDirection = CalculateDirection().normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        BulletScript bul = bullet.GetComponent<BulletScript>();
        bul.bulletDamage = weaponDamage;
        
        bullet.transform.up = -shootingDirection;
        //bullet.transform.up = shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        SoundManager.instance.PlayShootingSound(model);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset && shootingMode != ShootingMode.Auto)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (shootingMode == ShootingMode.Burst && currBurst > 1 && bulletsLeft > 0)
        {
            currBurst--;
            bulletsLeft--;
            Invoke("FireWeapon", firingRate);
        }
    }

    // Bad function, only for Enemy, better combine it with normal firing.
    public void FireWeaponAsEnemy()
    {

        Vector3 shootingDirection = CalculateDirection().normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        BulletScript bul = bullet.GetComponent<BulletScript>();
        bul.bulletDamage = weaponDamage;

        bullet.transform.up = -shootingDirection;
        //bullet.transform.up = shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        // TODO: Add sound
        SoundManager.instance.PlayEnemyShootingSound(this);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    }
    private void ResetShot()
    {
        animator.SetBool("Shoot", false);
        readyToShoot = true;
        allowReset = true;
    }

    private Vector3 CalculateDirection()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 target;


        if (Physics.Raycast(ray, out hit))
        {
            target = hit.point;
        } else
        {
            target = ray.GetPoint(100);
        }

        Vector3 direction = target - bulletSpawn.position;
        if (Vector3.Angle(direction, -gameObject.transform.forward) > 10)
        {
            direction = -gameObject.transform.forward;
        }

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
    private IEnumerator resetShooting(float delay)
    {
        yield return new WaitForSeconds(delay);
        isShooting = true;
    }
}
