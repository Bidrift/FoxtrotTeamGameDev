using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance { get; set; }

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables")]
    public float throwForce = 10f;
    public GameObject objectSpawn;
    public float forceMultiplier = 0;

    [Header("Lethal")]
    public GameObject grenadePrefab;
    public int lethalsCount = 0;
    public int maxLethal = 2;
    public Grenade.Throwable equippedLethalType;

    [Header("Tactical")]
    public GameObject smokePrefab;
    public GameObject flashbangPrefab;
    public int tacticalsCount = 0;
    public int maxTactical = 2;
    public Grenade.Throwable equippedTacticalType;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        equippedLethalType = Grenade.Throwable.None;
        equippedTacticalType = Grenade.Throwable.None;
    }
    // Update is called once per frame
    void Update()
    {
        foreach (GameObject weapon in weaponSlots)
        {
            if (weapon == activeWeaponSlot)
            {
                weapon.SetActive(true);
            } else
            {
                weapon.SetActive(false);
            }
        }

        if (InputManager.instance.isSwitchable)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchActiveSlot(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchActiveSlot(1);
            }
        }
        if (InputManager.instance.isThrowable)
        {
            if (Input.GetKey(KeyCode.G))
            {
                forceMultiplier += Time.deltaTime;
                forceMultiplier = Math.Clamp(forceMultiplier, 1f, 4f);
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                if (lethalsCount > 0) ThrowLethal();
                forceMultiplier = 0;
            }

            if (Input.GetKey(KeyCode.T))
            {
                forceMultiplier += Time.deltaTime;
                forceMultiplier = Math.Clamp(forceMultiplier, 1f, 4f);
            }
            if (Input.GetKeyUp(KeyCode.T))
            {
                if (tacticalsCount > 0) ThrowTactical();
                forceMultiplier = 0;
            }
        } else
        {
            forceMultiplier = 0;
        }
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);
        GameObject throwable = Instantiate(tacticalPrefab, objectSpawn.transform.position, Camera.main.transform.rotation);
        throwable.GetComponent<CapsuleCollider>().enabled = false;
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Grenade>().hasThrown = true;
        StartCoroutine(startCollider(throwable));

        tacticalsCount -= 1;

        if (tacticalsCount <= 0)
        {
            equippedTacticalType = Grenade.Throwable.None;

        }
        HUDManager.instance.UpdateThrowables();
    }

    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);
        GameObject throwable = Instantiate(lethalPrefab, objectSpawn.transform.position, Camera.main.transform.rotation);
        throwable.GetComponent<CapsuleCollider>().enabled = false;
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Grenade>().hasThrown = true;
        StartCoroutine(startCollider(throwable));

        lethalsCount -= 1;

        if (lethalsCount <= 0)
        {
            equippedLethalType = Grenade.Throwable.None;

        }
        HUDManager.instance.UpdateThrowables();
    }

    private GameObject GetThrowablePrefab(Grenade.Throwable type)
    {
        switch(type)
        {
            case Grenade.Throwable.Grenade:
                return grenadePrefab;
            case Grenade.Throwable.Smoke:
                return smokePrefab;
            case Grenade.Throwable.Flashbang:
                return flashbangPrefab;
        }
        return null; 
    }

    private IEnumerator startCollider(GameObject throwable)
    {
        yield return new WaitForSeconds(0.2f);
        throwable.GetComponent<Collider>().enabled = true;
    }

    public void PickupWeapon(GameObject pickedUpWeapon)
    {
        GameObject chosenSlot = getTargetSlot(pickedUpWeapon);
        AddWeaponIntoSlot(pickedUpWeapon, chosenSlot);
    }

    private void AddWeaponIntoSlot(GameObject pickedUpWeapon, GameObject chosenWeaponSlot)
    {
        DropCurrentWeapon(pickedUpWeapon, chosenWeaponSlot);
        pickedUpWeapon.transform.SetParent(chosenWeaponSlot.transform, false);
        Weapon weapon = pickedUpWeapon.GetComponent<Linker>().weapon.GetComponent<Weapon>();
        if (weapon.isArms)
        {
            weapon.arms.SetActive(true);
        }
        pickedUpWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);
        pickedUpWeapon.GetComponent<Linker>().weapon.GetComponent<Outline>().enabled = false;
        if (chosenWeaponSlot == activeWeaponSlot)
        {
            weapon.isActiveWeapon = true;
            pickedUpWeapon.GetComponent<Animator>().enabled = true;
        }
    }

    private void DropCurrentWeapon(GameObject weapon, GameObject chosenWeaponSlot)
    {
        if (chosenWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = chosenWeaponSlot.transform.GetChild(0).gameObject;
            weaponToDrop.GetComponent<Linker>().weapon.GetComponent<Weapon>().isActiveWeapon = false;
            if (weaponToDrop.GetComponent<Linker>().weapon.GetComponent<Weapon>().isArms)
            {
                weaponToDrop.GetComponent<Linker>().weapon.GetComponent<Weapon>().arms.SetActive(false);
            }
            weaponToDrop.transform.SetParent(weapon.transform.parent);
            weaponToDrop.transform.localPosition = weapon.transform.localPosition;
            weaponToDrop.transform.localRotation = weapon.transform.localRotation;
            weaponToDrop.GetComponent<Animator>().enabled = false;
            weaponToDrop.GetComponent<Linker>().weapon.GetComponent<Outline>().enabled = false;
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetComponentInChildren<Weapon>();
            currentWeapon.isActiveWeapon = false;
            activeWeaponSlot.transform.GetChild(0).GetComponent<Animator>().enabled = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetComponentInChildren<Weapon>();
            newWeapon.isActiveWeapon = true;
            activeWeaponSlot.transform.GetChild(0).GetComponent<Animator>().enabled = true;
        }
    }

    public GameObject getTargetSlot(GameObject weapon)
    {
        return weaponSlots[(int)weapon.GetComponent<Linker>().weapon.GetComponent<Weapon>().type];   
    }

    internal void PickupAmmo(Ammo hoveredAmmo)
    {
        int amount = hoveredAmmo.ammoAmount;
        switch (hoveredAmmo.ammoType)
        {
            case Ammo.AmmoType.PistolAmmo:
                totalPistolAmmo += amount;
                break;
            case Ammo.AmmoType.RifleAmmo:
                totalRifleAmmo += amount;
                break;
            default:
                break;
        }
    }

    internal void DecreaseTotalAmmo(Weapon.WeaponModel model, int amount)
    {
        switch (model)
        {
            case Weapon.WeaponModel.M4A1:
                totalRifleAmmo += amount;
                break;
            case Weapon.WeaponModel.FN57:
            case Weapon.WeaponModel.Baretta:
                totalPistolAmmo += amount;
                break;
            default:
                break;
        }
    }

    public int CheckAmmoLeft(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.FN57:
            case Weapon.WeaponModel.Baretta:
                return totalPistolAmmo;
            case Weapon.WeaponModel.M4A1:
                return totalRifleAmmo;
            default:
                return 0;
        }
    }

    internal bool PickupGrenade(Grenade hoveredGrenade)
    {
        switch (hoveredGrenade.throwableType)
        {
            case Grenade.Throwable.Grenade:
                return PickupGrenadeAsLethal(hoveredGrenade);
            case Grenade.Throwable.Smoke:
                return PickupGrenadeAsTactical(hoveredGrenade);
            case Grenade.Throwable.Flashbang:
                return PickupGrenadeAsTactical(hoveredGrenade);
            default:
                break;
        }
        return false;
    }

    private bool PickupGrenadeAsLethal(Grenade hoveredGrenade)
    {
        if (equippedLethalType == hoveredGrenade.throwableType || equippedLethalType == Grenade.Throwable.None)
        {
            equippedLethalType = hoveredGrenade.throwableType;
            if (lethalsCount < maxLethal)
            {
                lethalsCount += 1;
                Destroy(hoveredGrenade);
                HUDManager.instance.UpdateThrowables();
                return true;
            } else
            {
                Debug.Log("Limit lethal reached");
            }
        } else
        {
            // Swap lethal
        }
        return false;
    }

    private bool PickupGrenadeAsTactical(Grenade hoveredGrenade)
    {
        if (equippedTacticalType == hoveredGrenade.throwableType || equippedTacticalType == Grenade.Throwable.None)
        {
            equippedTacticalType = hoveredGrenade.throwableType;
            if (tacticalsCount < maxTactical)
            {
                tacticalsCount += 1;
                Destroy(hoveredGrenade);
                HUDManager.instance.UpdateThrowables();
                return true;
            }
            else
            {
                Debug.Log("Limit tactical reached");
            }
        }
        else
        {
            // Swap lethal
        }
        return false;
    }

    public void OnPlayerDeath()
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetComponentInChildren<Weapon>();
            currentWeapon.isActiveWeapon = false;
            activeWeaponSlot.transform.GetChild(0).GetComponent<Animator>().enabled = false;
        }
    }
}
