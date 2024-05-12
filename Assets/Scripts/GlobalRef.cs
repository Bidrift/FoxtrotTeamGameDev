using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalRef : MonoBehaviour
{

    public static GlobalRef instance {  get; set; }

    public GameObject bulletImpactEffectPrefab;

    public GameObject grenadeExplosionEffect;
    public GameObject smokeExplosionEffect;
    public GameObject flashbangEffect;
    public GameObject bloodsprayEffect;
    public GameObject blurEffect;
    internal float respawnTime = 5f;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        if (instance != null && instance != this)
        {
            Destroy(instance);
        } else
        {
            instance = this;
        }
    }
}
