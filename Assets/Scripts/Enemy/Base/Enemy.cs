using Photon.Pun;
using UnityEngine;

public class Enemy : MonoBehaviourPun, IEnemyDamageable, IEnemyMoveableAI, IEnemyAttackable
{
    public virtual void FindNewTarget() { }

    public virtual void TakeDamage(int damage, Collider collider, GameObject player) { }

    public virtual void Heal(int amount) { }

    public virtual void Die() { }

    public virtual void Chase() { }

    public virtual void AttackStart() { }

    public virtual void Attack() { }

    public virtual void AttackEnd() { }

    public virtual void StopChase() { }
}
