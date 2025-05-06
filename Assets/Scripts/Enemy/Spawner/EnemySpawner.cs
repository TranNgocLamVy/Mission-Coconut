using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 5f;

    [Header("Spawn on Start")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int numberOfEnemy = 1;

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (spawnOnStart)
        {
            for (int i = 0; i < numberOfEnemy; i++)
            {
                SpawnEnemy();
            }
        }
    }


    public void SpawnEnemy()
    {
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = transform.position.y; // Keep the y position the same
        Vector3 spawnPosition = transform.position + randomOffset;

        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        PhotonNetwork.InstantiateRoomObject(enemyPrefab.name, spawnPosition, randomRotation);
    }
}
