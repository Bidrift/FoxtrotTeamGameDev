using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;

    public bool isDead = false;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            PlayerDead();

        } else
        {
            HitPlayer();
        }
    }



    private void HitPlayer()
    {
        Debug.Log("Player hit");
    }

    private void PlayerDead()
    {
        Debug.Log("Player Dead");
        InputManager.instance.OnPlayerDeath();
        HUDManager.instance.OnPlayerDeath();
        WeaponManager.instance.OnPlayerDeath();
        StartCoroutine(GoToMainMenu());
    }

    private IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(GlobalRef.instance.respawnTime+0.1f);
        gameObject.GetComponent<PauseScript>().GoToMainMenu();
    }

    internal void WinGame()
    {
        InputManager.instance.OnPlayerWin();
        HUDManager.instance.OnPlayerWin();
        StartCoroutine(GoToMainMenu());
    }
}
