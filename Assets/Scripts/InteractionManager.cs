using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance { get; set; }

    public Weapon hoveredWeapon;
    public Ammo hoveredAmmo;
    public Grenade hoveredGrenade;
    public Grenade hoveredDoor;
    public float rangeOfInteraction = 5f;
    // Start is called before the first frame update
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

    // Change this into a more generic interaction.
    void Update()
    {
        if (InputManager.instance.interacting)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
            
                // Guns
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject.CompareTag("Gun") && !hitObject.GetComponent<Linker>().weapon.GetComponent<Weapon>().isActiveWeapon && isWithinDistance(hitObject))
                {
                    if (hoveredWeapon)
                    {
                        hoveredWeapon.GetComponent<Outline>().enabled = false;
                    }
                    hoveredWeapon = hitObject.GetComponent<Linker>().weapon.GetComponent<Weapon>();
                    hoveredWeapon.GetComponent<Outline>().enabled = true;
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        WeaponManager.instance.PickupWeapon(hitObject.gameObject);
                    }
                }
                else
                {
                    if (hoveredWeapon)
                    {
                        hoveredWeapon.GetComponent<Outline>().enabled = false;
                    }
                }

                // Ammmo
                if (hitObject.CompareTag("Ammo") && isWithinDistance(hitObject))
                {
                    if (hoveredAmmo)
                    {
                        hoveredAmmo.GetComponent<Outline>().enabled = false;
                    }
                    hoveredAmmo = hitObject.GetComponent<Ammo>();
                    hoveredAmmo.GetComponent<Outline>().enabled = true;
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        WeaponManager.instance.PickupAmmo(hoveredAmmo);
                        Destroy(hitObject.gameObject);
                    }
                }
                else
                {
                    if (hoveredAmmo)
                    {
                        hoveredAmmo.GetComponent<Outline>().enabled = false;
                    }
                }

                // Grenades
                if (hitObject.CompareTag("Grenade") && isWithinDistance(hitObject))
                {
                    if (hoveredGrenade)
                    {
                        hoveredGrenade.GetComponent<Outline>().enabled = false;
                    }
                    hoveredGrenade = hitObject.GetComponent<Grenade>();
                    hoveredGrenade.GetComponent<Outline>().enabled = true;
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (WeaponManager.instance.PickupGrenade(hoveredGrenade))
                        Destroy(hitObject.gameObject);
                    }
                }
                else
                {
                    if (hoveredGrenade)
                    {
                        hoveredGrenade.GetComponent<Outline>().enabled = false;
                    }
                }
            }
        }
    }

    private bool isWithinDistance(GameObject hitObject)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 targetPositon = hitObject.transform.position;
        Vector3 distanceVector = targetPositon - cameraPosition;
        float distance = distanceVector.magnitude;
        return distance < rangeOfInteraction;
    }
}
