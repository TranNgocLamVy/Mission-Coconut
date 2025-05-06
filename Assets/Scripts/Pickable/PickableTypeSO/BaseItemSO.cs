using UnityEngine;


public class BaseItemSO : ScriptableObject
{
    public virtual string itemName => "Unnamed Item";
    public AudioClip pickupSound;
    public AudioClip useSound;

    public virtual bool Pickup(GameObject player) { return false; }
    public virtual bool Use(GameObject player) { return false; }
}
