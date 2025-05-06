using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : Enemy
{
    [Header("Reference")]
    [SerializeField] public Animator animator;
    [SerializeField] public EnemySO enemy;
    [SerializeField] public Collider bodyCollider;
    [NonSerialized] private AudioSource audioSource;

    [Header("Health")]
    [HideInInspector] public int currentHp;

    [Header("Enemy AI Move")]
    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] public GameObject currentTarget;

    [Header("Enemy Attack")]
    [SerializeField] public BoxCollider attackCollider;
    [SerializeField] public BoxCollider detectCollider;
    [NonSerialized] public bool isAttacking = false;

    [Header("Special Hurt Box")]
    [SerializeField] private Collider headCollider;

    [Header("Behavior")]
    [SerializeField] public bool canRoam = true;
    [SerializeField] public bool canTriggerOtherEnemy = false;

    private float enemySoundTimer = 0;

    private EnemyStateMachine stateMachine;
    private ZombieIdleState idleState;
    private ZombieRoamState roamState;
    private ZombieChaseState chaseState;
    private ZombieAttackState attackState;
    private ZombieDieState dieState;

    public float randomSpeedMul;

    private void Awake()
    {
        stateMachine = new EnemyStateMachine();
        idleState = new ZombieIdleState(this, stateMachine);
        chaseState = new ZombieChaseState(this, stateMachine);
        attackState = new ZombieAttackState(this, stateMachine);
        dieState = new ZombieDieState(this, stateMachine);
        roamState = new ZombieRoamState(this, stateMachine);
    }

    private void Start()
    {
        currentHp = enemy.maxHp;
        agent = GetComponent<NavMeshAgent>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        if (animator != null && enemy != null && agent != null)
        {
            randomSpeedMul = (float)(UnityEngine.Random.Range(-2, 2) / 10) + enemy.speedMultiplier;
            animator.speed *= randomSpeedMul;
            agent.speed *= randomSpeedMul;
        }
        stateMachine.Initialize(idleState);

        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        stateMachine.currentState?.Update();

        if (currentTarget == null)
        {
            FindNewTarget();
        }
        enemySoundTimer -= Time.deltaTime;
        if (enemySoundTimer < 0)
        {
            photonView.RPC("RPC_PlayZombieSound", RpcTarget.All, 4);
            enemySoundTimer = UnityEngine.Random.Range(enemy.enemySoundInterval - 0.5f, enemy.enemySoundInterval + 0.5f);
        }
    }

    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        stateMachine.currentState?.FixedUpdate();
    }

    public override void FindNewTarget()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Only MasterClient handles target acquisition

        Vector3 position = transform.position;
        Vector3 forward = transform.forward;

        Collider[] playerColliders = Physics.OverlapSphere(position, enemy.aggroDistance, LayerMask.GetMask("Player"));

        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider playerCollider in playerColliders)
        {
            GameObject player = playerCollider.gameObject;
            if (player != null)
            {
                Vector3 directionToPlayer = (player.transform.position - position).normalized;
                float distanceToPlayer = Vector3.Distance(position, player.transform.position);

                float angleToPlayer = Vector3.Angle(forward, directionToPlayer);
                bool isInFront = angleToPlayer < enemy.viewAngle / 2f;
                bool isCloseEnough = distanceToPlayer <= enemy.aggroDistance;

                bool hasLineOfSight = !Physics.Raycast(position, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Default", "Ground", "Surrounding"));

                if (hasLineOfSight)
                {
                    if ((isInFront || distanceToPlayer <= enemy.closeDetectionDistance) && distanceToPlayer < closestDistance)
                    {
                        closestPlayer = player;
                        closestDistance = distanceToPlayer;
                    }
                }
            }
        }

        if (closestPlayer != null && closestPlayer.TryGetComponent(out PhotonView playerView))
        {
            // Sync target across all clients
            photonView.RPC("RPC_SetTargetByViewID", RpcTarget.All, playerView.ViewID);
        }
    }


    public override void TakeDamage(int damage, Collider collider, GameObject player)
    {
        int calculatedDamage = damage;

        if (collider == headCollider)
        {
            calculatedDamage *= enemy.headShotMultiplier;
            photonView.RPC("RPC_PlayZombieSound", RpcTarget.All, 2);
        }
        else
        {
            photonView.RPC("RPC_PlayZombieSound", RpcTarget.All, 1);
        }

        photonView.RPC("RPC_RequestTakeDamage", RpcTarget.All, calculatedDamage);

        // Set target and sync
        if (currentTarget == null && player.TryGetComponent(out PhotonView playerView))
        {
            photonView.RPC("RPC_SetTargetByViewID", RpcTarget.All, playerView.ViewID);
        }

        // Trigger nearby enemies if allowed
        if (canTriggerOtherEnemy && PhotonNetwork.IsMasterClient && player.TryGetComponent(out PhotonView attackerView))
        {
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, 20f, LayerMask.GetMask("Enemy"));

            foreach (var col in nearbyEnemies)
            {
                if (col.TryGetComponent(out Zombie otherZombie) && otherZombie != this && otherZombie.currentTarget == null)
                {
                    otherZombie.photonView.RPC("RPC_SetTargetByViewID", RpcTarget.All, attackerView.ViewID);
                }
            }
        }
    }


    [PunRPC]
    public void RPC_SetTargetByViewID(int playerViewID)
    {
        PhotonView playerView = PhotonView.Find(playerViewID);
        if (playerView != null)
        {
            currentTarget = playerView.gameObject;
        }
    }


    [PunRPC]
    public void RPC_RequestTakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0 && stateMachine.currentState != dieState)
        {
            stateMachine.ChangeState(dieState);
        }
    }

    public override void Die()
    {
        photonView.RPC("RequestDestroyEnemy", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void RequestDestroyEnemy()
    {
        // Only the MasterClient will handle the actual destruction
        if (PhotonNetwork.IsMasterClient)
        {
            DestroyEnemy();
        }
    }
    
    public void DestroyEnemy()
    {
        // Destroy the object only if the current client is the MasterClient
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public override void Chase()
    {
        if (currentTarget != null)
        {
            animator.SetBool("IsChasing", true);
            agent.isStopped = false;
            agent.SetDestination(currentTarget.transform.position);
        }
    }

    public override void AttackStart()
    {
        isAttacking = true;
    }

    public override void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public override void AttackEnd()
    {
        isAttacking = false;
    }

    public override void StopChase()
    {
        animator.SetBool("IsChasing", false);
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void RequestChangeState(string stateName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncStateByName", RpcTarget.All, stateName);
        }
    }

    [PunRPC]
    public void SyncStateByName(string stateName)
    {
        if (stateMachine == null) return;

        // Prevent unnecessary state changes
        if (stateMachine.currentState?.GetType().Name == stateName)
            return;

        switch (stateName)
        {
            case nameof(ZombieIdleState):
                stateMachine.ChangeState(idleState);
                break;
            case nameof(ZombieChaseState):
                stateMachine.ChangeState(chaseState);
                break;
            case nameof(ZombieAttackState):
                stateMachine.ChangeState(attackState);
                break;
            case nameof(ZombieDieState):
                stateMachine.ChangeState(dieState);
                break;
            case nameof(ZombieRoamState):
                stateMachine.ChangeState(roamState);
                break;
        }
    }

    private void ConfigureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource component was added automatically to the enmey.");
        }

        // Set up 3D sound properties
        audioSource.spatialBlend = 1f; // Full 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 100;
        audioSource.playOnAwake = false;
    }

    [PunRPC]
    public void RPC_PlayZombieSound(int type)
    {
        float volume = 0.25f;
        switch(type)
        {
            case 1:
                audioSource.PlayOneShot(enemy.hitSound, volume);
                break;
            case 2:
                audioSource.PlayOneShot(enemy.headShotSound, volume);
                break;
            case 3:
                audioSource.PlayOneShot(enemy.enemyDieSound, volume);
                break;
            case 4:
                if (enemy.enemySound.Count == 0) break;
                System.Random r = new System.Random();
                int randomClip = r.Next(0, enemy.enemySound.Count);
                audioSource.PlayOneShot(enemy.enemySound[randomClip], volume);
                break;
        }
    }
}
