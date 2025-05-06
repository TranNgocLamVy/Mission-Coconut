using UnityEngine;
using UnityEngine.AI;

public class ZombieRoamState : EnemyState
{
    protected Zombie zombie;
    private Vector3 roamTarget;
    private float roamRadius = 10f;
    private float roamTimer;
    private float maxRoamTime = 5f; // Maximum roaming time

    public ZombieRoamState(Zombie enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        zombie = enemy;
    }

    public override void EnterState()
    {
        zombie.animator.speed = zombie.randomSpeedMul;
        zombie.animator.SetBool("IsChasing", true); // Play moving animation
        zombie.agent.isStopped = false;

        PickNewRoamTarget();
        roamTimer = 0f;
    }

    public override void ExitState()
    {
        zombie.animator.SetBool("IsChasing", false); // Stop moving animation
        zombie.agent.ResetPath();
        zombie.agent.isStopped = true;
    }

    public override void FixedUpdate()
    {
        roamTimer += Time.fixedDeltaTime;

        // 1. If timer runs out
        if (roamTimer >= maxRoamTime)
        {
            zombie.RequestChangeState(nameof(ZombieIdleState));
            return;
        }

        // 2. If reached destination
        if (!zombie.agent.pathPending && zombie.agent.remainingDistance <= zombie.agent.stoppingDistance)
        {
            zombie.RequestChangeState(nameof(ZombieIdleState));
            return;
        }
    }

    public override void Update()
    {
        // Look for player
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
            zombie.FindNewTarget();
        }
    }

    private void PickNewRoamTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += zombie.transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if (zombie.agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                roamTarget = hit.position;
                zombie.agent.SetDestination(roamTarget);
            }
            else
            {
                // Failed path, try picking a new one
                PickNewRoamTarget();
            }
        }
        else
        {
            // Failed finding any nearby point, retry
            PickNewRoamTarget();
        }
    }
}
