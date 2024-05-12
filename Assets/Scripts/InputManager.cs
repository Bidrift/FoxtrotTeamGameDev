using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; set; }

    public bool jumping = true;
    public bool crouching = true;
    public bool running = true;
    public bool walking = true;
    public bool shooting = true;
    public bool reloading = true;
    public bool interacting = true;
    public bool movingCamera = true;
    public bool enemyShooting = true;
    public bool isADSable = true;
    public bool isSwitchable = true;
    public bool isThrowable = true;
    public bool isPausable = true;

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
            jumping = true;
            crouching = true;
            running = true;
            walking = true;
            shooting = true;
            reloading = true;
            interacting = true;
            movingCamera = true;
            enemyShooting = true;
            isADSable = true;
            isSwitchable = true;
            isThrowable = true;
            isPausable = true;
        }
    }

    public void togglePause()
    {
        isPausable = !isPausable;
    }

    public void toggleThrowable()
    {
        isThrowable = !isThrowable;
    }

    public void toggleADS()
    {
        isADSable = !isADSable;
    }

    public void toggleSwitch()
    {
        isSwitchable = !isSwitchable;
    }

    public void toggleInteracting()
    {
        interacting = !interacting;
    }

    public void toggleJumping()
    {
        jumping = !jumping;
    }

    public void toggleCrouching()
    {
        crouching = !crouching;
    }

    public void toggleRunning()
    {
        running = !running;
    }

    public void toggleWalking()
    {
        walking = !walking;
    }
    
    public void toggleShooting()
    {
        shooting = !shooting;
    }

    public void toggleReloading()
    {
        reloading = !reloading;
    }

    public void toggleMovingCamera()
    {
        movingCamera = !movingCamera;
    }

    internal void pauseInputs()
    {
        toggleCrouching();
        toggleJumping();
        toggleRunning();
        toggleMovingCamera();
        toggleReloading();
        toggleShooting();
        toggleWalking();
        toggleInteracting();
        toggleEnemyShooting();
        toggleADS();
        toggleThrowable();
    }

    private void toggleEnemyShooting()
    {
        enemyShooting = !enemyShooting;
    }

    internal void OnPlayerDeath()
    {
        toggleCrouching();
        toggleJumping();
        toggleRunning();
        toggleMovingCamera();
        toggleReloading();
        toggleShooting();
        toggleWalking();
        toggleInteracting();
        toggleADS();
        toggleThrowable();
        togglePause();
        StartCoroutine(ResetInput());
    }

    private IEnumerator ResetInput()
    {
        yield return new WaitForSeconds(GlobalRef.instance.respawnTime);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        toggleCrouching();
        toggleJumping();
        toggleRunning();
        toggleMovingCamera();
        toggleReloading();
        toggleShooting();
        toggleWalking();
        toggleInteracting();
        toggleADS();
        toggleThrowable();
        togglePause();
    }

    internal void OnPlayerWin()
    {
        toggleCrouching();
        toggleJumping();
        toggleRunning();
        toggleMovingCamera();
        toggleReloading();
        toggleShooting();
        toggleWalking();
        toggleInteracting();
        toggleADS();
        toggleThrowable();
        togglePause();
        StartCoroutine(ResetInput());
    }
}
