

using UnityEngine;

public interface IEnemyDamageable
{
    // Methods
    void TakeDamage(int damage, Collider collider, GameObject player);
    void Heal(int amount);
    void Die();
}
