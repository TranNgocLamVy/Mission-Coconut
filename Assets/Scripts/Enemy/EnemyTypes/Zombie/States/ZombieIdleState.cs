using UnityEngine;

public class ZombieIdleState : EnemyState
{
    protected Zombie zombie;
    private float idleTimer = 0f;
    private float timeToRoam = 8f; // Wait 10 seconds before starting to roam

    public ZombieIdleState(Zombie enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        zombie = enemy;
    }

    public override void EnterState()
    {
        zombie.currentTarget = null;
        zombie.animator.speed = 1;
        //zombie.animator.applyRootMotion = true;
        idleTimer = 0f;
    }

    public override void ExitState()
    {
        zombie.animator.speed = zombie.randomSpeedMul;
        zombie.animator.applyRootMotion = false;
    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        if (zombie.currentTarget)
        {
            float distance = Vector3.Distance(zombie.transform.position, zombie.currentTarget.transform.position);
            if (distance <= zombie.enemy.aggroDistance)
            {
                zombie.RequestChangeState(nameof(ZombieChaseState));
                return;
            }
        }
        else
        {
            zombie.FindNewTarget(); // Keep looking for player

            if (zombie.canRoam)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= timeToRoam)
                {
                    zombie.RequestChangeState(nameof(ZombieRoamState));
                }
            }
        }
    }
}
