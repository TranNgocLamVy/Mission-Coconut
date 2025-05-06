using UnityEngine;

public class ZombieChaseState : EnemyState
{
    protected Zombie zombie;
    public ZombieChaseState(Zombie enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        zombie = enemy;
    }

    public override void EnterState()
    {
        zombie.Chase();
    }

    public override void ExitState()
    {
        zombie.StopChase();
    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        zombie.Chase();
        if (zombie.currentTarget)
        {
            float distance = Vector3.Distance(zombie.transform.position, zombie.currentTarget.transform.position);
            if (distance > zombie.enemy.stopAggroDistance)
            {
                zombie.RequestChangeState(nameof(ZombieIdleState));
            }
            else if (distance <= zombie.enemy.attackRange)
            {
                // Check if the player is in the attack collider
                Collider[] hitColliders = Physics.OverlapBox(zombie.detectCollider.bounds.center, zombie.detectCollider.bounds.extents, Quaternion.identity);
                foreach (Collider hitCollider in hitColliders)
                {
                    // Check if the collider is the current target
                    if (hitCollider.gameObject == zombie.currentTarget)
                    {
                        zombie.RequestChangeState(nameof(ZombieAttackState));
                        break;
                    }
                }

                // Slowly rotate to the player until the player is in the attack collider
                Vector3 direction = (zombie.currentTarget.transform.position - zombie.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, lookRotation, Time.deltaTime * 10f);
            }
        }
    }
}
