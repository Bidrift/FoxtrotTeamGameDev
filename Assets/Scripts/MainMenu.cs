using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    string levelOneScene = "Level1";
    string levelTwoScene = "Level2";
    string levelThreeScene = "Level3";
    // Start is called before the first frame update
    void Start()
    {
        float highScore = SaveLoadManager.instance.LoadHighScore();
        highScoreUI.text = $"Shortest Clearance Time: {highScore}s";
        Time.timeScale = 1.0f;

    }

    public void StartLevelOne()
    {
        SceneManager.LoadScene(levelOneScene);
    }

    public void StartLevelTwo()
    {
        SceneManager.LoadScene(levelTwoScene);
    }

    public void StartLevelThree()
    {
        SceneManager.LoadScene(levelThreeScene);
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
