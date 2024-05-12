using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{

    public static SaveLoadManager instance { get; set; }

    public string highScoreKey = "Shortest Clearance Time";
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

        DontDestroyOnLoad(this);
    }

    public void SaveHighScore(float score)
    {
        PlayerPrefs.SetFloat(highScoreKey, score);
    }

    public float LoadHighScore()
    {
        if (PlayerPrefs.HasKey(highScoreKey))
        return PlayerPrefs.GetFloat(highScoreKey);
        return 0f;
    }
}
