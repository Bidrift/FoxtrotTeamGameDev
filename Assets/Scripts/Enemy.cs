using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;

public class Enemy : MonoBehaviour
{

    [SerializeField] private int health = 100;

    private Animator animator;

    private GameObject player;

    [SerializeField] public Weapon weapon;

    public bool isDead = false;

    private NavMeshAgent navAgent;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (animator.GetBool("isFiring"))
        {
            LookAtPlayer();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - navAgent.transform.position;
        navAgent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = navAgent.transform.eulerAngles.y;
        navAgent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("hit");
        health -= damage;

        if (health <= 0)
        {
            animator.SetTrigger("DIE");
            isDead = true;
            EnemyManager.instance.killEnemy(gameObject);
        } else
        {
            animator.SetTrigger("DAMAGE");
        }
    }

    public void GetDizzy()
    {
        animator.SetTrigger("FLASHBANG");
    }

    public void Attack()
    {
        weapon.FireWeaponAsEnemy();
    }
}
