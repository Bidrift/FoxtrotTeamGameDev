using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFiringState : StateMachineBehaviour
{
    GameObject player;
    NavMeshAgent navAgent;

    public float stopAttackingDistance = 2.5f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        navAgent = animator.GetComponent<NavMeshAgent>();
        animator.GetComponent<Enemy>().Attack();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        float distanceFromPlayer = Vector3.Distance(player.transform.position, animator.transform.position);
        if (distanceFromPlayer > stopAttackingDistance || player.GetComponent<Player>().isDead)
        {
            animator.SetBool("isFiring", false);
        }
    }

}
