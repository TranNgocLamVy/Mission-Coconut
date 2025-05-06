using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.LowLevel;

public class PlayerLook : MonoBehaviourPun
{
    [Header("Camera")]
    [SerializeField] private PlayerSettingsSO playerSettings;
    [SerializeField] public Camera playerCamera;
    [NonSerialized] public bool isRecoiling = false;

    private float xRotation = 0f;

    void Start()
    {
        if (!photonView.IsMine) return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!playerCamera)
        {
            Debug.LogError("Player camera not assigned in PlayerLook script.");
        }
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= mouseY * playerSettings.yMouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.parent.gameObject.transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * playerSettings.xMouseSensitivity);
    }

    public void AddRecoil(float recoilAmount)
    {
        Quaternion recoilRotation = Quaternion.Euler(-recoilAmount, 0f, 0f);
        playerCamera.transform.localRotation = recoilRotation * playerCamera.transform.localRotation;
        isRecoiling = true;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (isRecoiling)
        {
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, 10f * Time.deltaTime);

            if (Quaternion.Angle(playerCamera.transform.localRotation, targetRotation) < 0.1f)
            {
                playerCamera.transform.localRotation = targetRotation;
                isRecoiling = false;
            }
        }
        else
        {
            playerCamera.transform.localRotation = targetRotation;
        }




        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
