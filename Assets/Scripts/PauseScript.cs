using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    private bool isPaused = false;
    private float delay = 1f;
    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && InputManager.instance.isPausable)
        {
            togglePause();
        }
    }

    private void togglePause()
    {
        GlobalRef.instance.blurEffect.SetActive(!GlobalRef.instance.blurEffect.activeSelf);
        HUDManager.instance.togglePause();
        InputManager.instance.pauseInputs();
        isPaused = !isPaused;
        if (isPaused)
        {
            SoundManager.instance.pauseSounds();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else
        {
            SoundManager.instance.resumeSounds();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Time.timeScale = (Time.timeScale == 0) ? 1 : 0;

        InputManager.instance.togglePause();
        StartCoroutine(reEnablePause());
    }

    private IEnumerator reEnablePause()
    {
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log("Reset");
        InputManager.instance.togglePause();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
