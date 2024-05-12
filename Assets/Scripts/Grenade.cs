using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] public float delay = 3f;
    [SerializeField] public float damageRadius = 20f;
    [SerializeField] public float explosionForce = 1200f;

    private float countDown;

    private bool hasExploded = false;
    public bool hasThrown = false;
    [SerializeField] private float destroyDelay;

    private Camera cam;

    public enum Throwable
    {
        None,
        Grenade,
        Smoke,
        Flashbang
    }

    public Throwable throwableType;
    // Start is called before the first frame update
    void Start()
    {
        countDown = 3f;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasThrown)
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0 && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    void Explode()
    {
        GetThrowableEffect();
        StartCoroutine(DestroyGrenade());
    }

    public IEnumerator DestroyGrenade()
    {
        switch (throwableType)
        {
            case Throwable.Smoke:
                destroyDelay = 40f;
                break;
            default:
                destroyDelay = 0f;
                break;

        }
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
    void GetThrowableEffect()
    {
        switch(throwableType)
        {
            case Throwable.Grenade:
                GrenadeEffect();
                break;
            case Throwable.Smoke:
                SmokeEffect();
                break;
            case Throwable.Flashbang:
                FlashbangEffect();
                break;
        }
    }

    private void FlashbangEffect()
    {
        GameObject flashbangEffect = GlobalRef.instance.flashbangEffect;
        Instantiate(flashbangEffect, transform.position, new Quaternion(0, 0, 0, 0));

        SoundManager.instance.PlayExplosionSound(this);

        // For player
        if (checkVisibility())
        {
            SoundManager.instance.PlayFlashbeep();
            HUDManager.instance.SetFlashbangUI();
        }

        // For enemy
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                collider.gameObject.GetComponent<Enemy>().GetDizzy();
            }
        }
    }

    private bool checkVisibility()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        var point = transform.position;

        foreach (var p in planes)
        {
            if (p.GetDistanceToPoint(point) > 0)
            {
                Ray ray = new Ray(cam.transform.position, transform.position - cam.transform.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    return hitObject.transform.gameObject == this.gameObject;
                }
                else return false;
            }
            else return false;
        }

        return false;
    }

    void SmokeEffect()
    {
        GameObject smokeEffect = GlobalRef.instance.smokeExplosionEffect;
        Instantiate(smokeEffect, transform.position, new Quaternion(0, 0, 0, 0));

        SoundManager.instance.PlayExplosionSound(this);

        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Enemy
            }
        }
    }

    void GrenadeEffect()
    {
        GameObject explosionEffect = GlobalRef.instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, new Quaternion(0, 0, 0, 0));

        SoundManager.instance.PlayExplosionSound(this);

        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            if (collider.gameObject.CompareTag("Enemy"))
            {
                collider.gameObject.GetComponent<Enemy>().TakeDamage(100);
            }
        }
    }
}
