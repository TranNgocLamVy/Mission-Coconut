using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackState : EnemyState
{
    protected Zombie zombie;
    private float attackCooldownTimer;
    private List<PlayerHealth> playerHit = new List<PlayerHealth>();

    public ZombieAttackState(Zombie enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        zombie = enemy;
    }

    public override void EnterState()
    {
        zombie.StopChase();
        zombie.Attack();
        attackCooldownTimer = zombie.enemy.attackCooldown / zombie.enemy.speedMultiplier;
    }

    public override void ExitState()
    {
        zombie.isAttacking = false;
        playerHit.Clear();
    }

    public override void FixedUpdate()
    {
        if (zombie.isAttacking)
        {
            // Check for collisions with the attack collider
            Collider[] hitColliders = Physics.OverlapBox(zombie.attackCollider.bounds.center, zombie.attackCollider.bounds.extents, Quaternion.identity);
            foreach (Collider hitCollider in hitColliders)
            {
                PlayerHealth playerHealth = hitCollider.GetComponentInParent<PlayerHealth>();
                if (playerHealth != null && !playerHit.Contains(playerHealth))
                {
                    playerHit.Add(playerHealth);
                    playerHealth.TakeDamage(zombie.enemy.damage);
                }
            }
        }
    }

    public override void Update()
    {
        attackCooldownTimer -= Time.deltaTime;
        if (attackCooldownTimer < 0)
        {
            zombie.RequestChangeState(nameof(ZombieChaseState));
        }
    }
}
