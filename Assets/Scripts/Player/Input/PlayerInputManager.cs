using UnityEngine;
public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerMovement movement;
    private PlayerLook look;
    private PlayerShoot shoot;
    private PlayerInteract interact;
    private PlayerInventory inventory;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();
        shoot = GetComponent<PlayerShoot>();
        interact = GetComponent<PlayerInteract>();
        inventory = GetComponent<PlayerInventory>();

        onFoot.Jump.performed += _ => movement.Jump();
        onFoot.SprintStart.performed += _ => movement.SprintStart();
        onFoot.SprintEnd.performed += _ => movement.SprintEnd();
        onFoot.ShootStart.performed += _ => shoot.ShootStart();
        onFoot.ShootEnd.performed += _ => shoot.ShootEnd();
        onFoot.ChangeGunMode.performed += _ => shoot.ChangeGunMode();
        onFoot.Reload.performed += _ => shoot.Reload();
        onFoot.Interact.performed += _ => interact.Interact();
        onFoot.UseItem.performed += _ => inventory.UseItem();
    }
    void FixedUpdate()
    {
        movement.ProcessMovement(onFoot.Movement.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable(); 
    }

}
