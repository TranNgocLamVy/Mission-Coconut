using Photon.Pun;
using UnityEngine;

public class ZombieDieState : EnemyState
{
    protected Zombie zombie;
    private float dieTimer = 3f;
    public ZombieDieState(Zombie enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        zombie = enemy;
    }

    public override void EnterState()
    {
        zombie.animator.speed = 1f;
        zombie.animator.SetTrigger("Die");
        zombie.photonView.RPC("RPC_PlayZombieSound", RpcTarget.All, 3);
        zombie.bodyCollider.enabled = false;
        zombie.agent.enabled = false;
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        dieTimer -= Time.deltaTime;
        if (dieTimer <= 0)
        {
            zombie.Die();
        }
    }
}
