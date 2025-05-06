using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/Enemy/EnemyStats")]
public class EnemySO : ScriptableObject
{
    [Header("Health")]
    public int maxHp = 100;
    public int headShotMultiplier = 5;

    [Header("Enemy AI")]
    public float aggroDistance = 15f;
    public float stopAggroDistance = 25f;
    public float viewAngle = 120f;
    public float closeDetectionDistance = 3f;

    [Header("Enemy Attack")]
    public int damage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("Enemy Speed")]
    public float speedMultiplier = 1f;

    [Header("Enemy Sfxs")]
    public AudioClip hitSound;
    public AudioClip headShotSound;
    public AudioClip enemyDieSound;
    public List<AudioClip> enemySound = new List<AudioClip>();
    public float enemySoundInterval = 2f;
}
