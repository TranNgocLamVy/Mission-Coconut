using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/Player/Player Settings")]
public class PlayerSettingsSO : ScriptableObject
{
    [Header("Camera")]
    [SerializeField][Range(0, 100)] public float xMouseSensitivity = 10f;
    [SerializeField][Range(0, 100)] public float yMouseSensitivity = 10f;
}
