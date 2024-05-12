using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private bool hasHit = false;
    public int bulletDamage;
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.velocity /= 50;
        if (collision.gameObject.CompareTag("Target"))
        {
            Destroy(gameObject);
            Debug.Log("hit " + collision.gameObject.name);
            createBulletImpactEffect(collision);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            createBulletImpactEffect(collision);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!collision.gameObject.GetComponent<Enemy>().isDead)
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            }

            CreateBloodSprayEffect(collision);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponent<Player>().isDead)
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(bulletDamage);
            }

            CreateBloodSprayEffect(collision);
            Destroy(gameObject);
        }

    }

    private void CreateBloodSprayEffect(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;
            ContactPoint contactPoint = collision.contacts[0];

            GameObject bloodSprayPrefab = Instantiate(GlobalRef.instance.bloodsprayEffect, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));

            bloodSprayPrefab.transform.SetParent(collision.gameObject.transform);
        }
    }

    private void createBulletImpactEffect(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;
            ContactPoint contactPoint = collision.contacts[0];

            GameObject hole = Instantiate(GlobalRef.instance.bulletImpactEffectPrefab, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));

            hole.transform.SetParent(collision.gameObject.transform);
        }
    }
}
