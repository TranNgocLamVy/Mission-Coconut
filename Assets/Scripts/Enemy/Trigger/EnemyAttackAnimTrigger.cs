using UnityEngine;

public class EnemyAttackAnimTrigger : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    public void AttackStart()
    {
        enemy.AttackStart();
    }

    public void AttackEnd()
    {
        enemy.AttackEnd();
    }
}
