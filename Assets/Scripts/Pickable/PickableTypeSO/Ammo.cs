using UnityEngine;

[CreateAssetMenu(fileName = "Ammo", menuName = "Scriptable Objects/PickableItems/Ammo")]
public class Ammo : BaseItemSO
{
    [Header("Item Information")]
    [SerializeField] public override string itemName => "Ammo";
    [SerializeField] private int ammoAmount = 30;

    public override bool Pickup(GameObject player)
    {
        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
        if (playerShoot != null)
        {
            playerShoot.AddAmmos(ammoAmount);
            return true;
        }
        return false;
    }
}
