using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public enum CryptoState
{
    IDLE,
    RUN,
    JUMP,
    KICK
}


public class CryptoBehaviour : MonoBehaviour
{
    [Header("Line of Sight")]
    public bool HasLOS = false;

    public GameObject player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Attack")]
    public float Distance;
    public PlayerBehaviour playerHealth;
    public float damageDelay = 1.0f;
    public bool isAttacking = false;
    public float kickForce = 0.01f;
    public float distanceToPlayer; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = FindObjectOfType<PlayerBehaviour>();
;    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (HasLOS)
        {
            agent.SetDestination(player.transform.position);
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        }


        if (HasLOS && distanceToPlayer < Distance && !isAttacking)
        {
            // could be an attack
            animator.SetInteger("AnimState", (int)CryptoState.KICK);
            transform.LookAt(transform.position - player.transform.forward);

            DoKickDamage();
            isAttacking = true;

            if (agent.isOnOffMeshLink)
            {
                animator.SetInteger("AnimState", (int)CryptoState.JUMP);
            }
        }
        else if (HasLOS && distanceToPlayer > Distance)
        {
            animator.SetInteger("AnimState", (int)CryptoState.RUN);
            isAttacking = false;
        }
        else
        {
            animator.SetInteger("AnimState", (int)CryptoState.IDLE);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HasLOS = true;
            player = other.transform.gameObject;
        }
    }

    public void DoKickDamage()
    {
        playerHealth.TakeDamage(20);
        StartCoroutine(KickDamage());
    }

    public IEnumerator KickDamage()
    {
        yield return new WaitForSeconds(0.6f);
        isAttacking = true;
        var direaction = Vector3.Normalize(player.transform.position - transform.position);
        playerHealth.controller.Move(direaction * kickForce);
        StopCoroutine(KickDamage());
    }

}

