using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player/Player Stats")]
public class PlayerSO : ScriptableObject
{
    [Header("Health")]
    public int maxHp = 100;

    [Header("Movement")]
    public Vector3 maxWalkSpeed = new Vector3(4.5f, 0f, 6.75f);
    public Vector3 maxSprintSpeed = new Vector3(6f, 0f, 9f);
    public float acceleration = 10f;

    [Header("Jump")]
    public float jumpForce = 6f;
    public LayerMask groundLayer;

    [Header("Shoot")]
    public float shootRange = 100f;
    public int damage = 100;
    public int defaultMagazineSize = 30;
    public int defaultNumberOfBullet = 90;
    public float reloadTime = 2f;
    public float fireRate = 0.1f;
    public LayerMask enemyLayer;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;

}
