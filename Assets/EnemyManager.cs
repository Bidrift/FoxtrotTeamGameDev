using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager instance { get; set; }

    GameObject[] enemiesList;

    public int killedEnemies;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        enemiesList = GameObject.FindGameObjectsWithTag("Enemy");
        killedEnemies = 0;
    }

    // Update is called once per frame
    public void killEnemy(GameObject enemy) 
    {
        killedEnemies++;
        if (killedEnemies == enemiesList.Length)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().WinGame();
        }
    }
}
