using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;
using static Grenade;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; set; }

    public AudioSource shootingSound;

    public AudioClip M4A1ShootingAudio;

    public AudioClip FN57ShootingAudio;

    public AudioClip M4A1Reload;
    public AudioClip FN57Reload;

    public GameObject soundPrefab;

    public AudioClip explosionAudio;
    public AudioClip smokeAudio;
    public AudioClip flashbangAudio;
    public AudioClip flashbeepAudio;

    public AudioClip fbiSound;

    // Start is called before the first frame update
    void Start()
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

    public void PlayShootingSound(WeaponModel model)
    {
        switch (model)
        {
            case WeaponModel.M4A1:
                shootingSound.PlayOneShot(M4A1ShootingAudio);
                break;
            case WeaponModel.FN57:
            case WeaponModel.Baretta:
                shootingSound.PlayOneShot(FN57ShootingAudio);
                break;
        }
    }

    public void PlayFlashbeep()
    {
        shootingSound.PlayOneShot(flashbeepAudio);
    }

    public void PlayExplosionSound(Grenade throwable)
    {
        GameObject soundThrowable = Instantiate(soundPrefab, throwable.gameObject.transform.position, throwable.gameObject.transform.rotation);
        switch (throwable.throwableType)
        {
            case Throwable.Grenade:
                soundThrowable.GetComponent<AudioSource>().PlayOneShot(explosionAudio);
                StartCoroutine(Destroy3DSound(soundThrowable, explosionAudio.length));
                break;
            case Throwable.Smoke:
                soundThrowable.GetComponent<AudioSource>().PlayOneShot(smokeAudio);
                StartCoroutine(Destroy3DSound(soundThrowable, smokeAudio.length));
                break;
            case Throwable.Flashbang:
                soundThrowable.GetComponent<AudioSource>().PlayOneShot(flashbangAudio);
                StartCoroutine(Destroy3DSound(soundThrowable, flashbangAudio.length));
                break;
        }
    }

    public IEnumerator Destroy3DSound(GameObject soundObject, float delay) 
    {
        yield return new WaitForSeconds(delay);
        Destroy(soundObject);
    }

    internal void KnockSound()
    {
        shootingSound.PlayOneShot(fbiSound);
    }

    internal void PlayReloadingSound(WeaponModel model)
    {
        switch (model)
        {
            case WeaponModel.M4A1:
                shootingSound.PlayOneShot(M4A1Reload);
                break;
            case WeaponModel.FN57:
            case WeaponModel.Baretta:
                shootingSound.PlayOneShot(FN57Reload);
                break;
        }
    }

    internal void pauseSounds()
    {
        shootingSound.Pause();
    }

    internal void resumeSounds()
    {
        shootingSound.UnPause();
    }

    internal void PlayEnemyShootingSound(Weapon weapon)
    {
        GameObject soundWeapon = Instantiate(soundPrefab, weapon.gameObject.transform.position, weapon.gameObject.transform.rotation);
        switch (weapon.model)
        {
            case WeaponModel.M4A1:
                soundWeapon.GetComponent<AudioSource>().PlayOneShot(M4A1ShootingAudio);
                StartCoroutine(Destroy3DSound(soundWeapon, explosionAudio.length));
                break;
            case WeaponModel.FN57:
            case WeaponModel.Baretta:
                soundWeapon.GetComponent<AudioSource>().PlayOneShot(FN57ShootingAudio);
                StartCoroutine(Destroy3DSound(soundWeapon, explosionAudio.length));
                break;
        }
    }
}
