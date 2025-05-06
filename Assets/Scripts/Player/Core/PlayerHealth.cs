using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun
{
    [Header("Main")]
    [SerializeField] private PlayerSO playerStats;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;

    [Header("Health")]
    [SerializeField] private int currentHp;

    [Header("Damaged")]
    [SerializeField] private RawImage bloodOverlay;
    [SerializeField] private AudioClip hurtSound;
    private AudioSource audioSource;

    void Start()
    {
        currentHp = playerStats.maxHp;
        SetBloodOverlayAlpha(0f);
        UpdateHpText();
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        photonView.RPC("RequestChangeHp", RpcTarget.AllBuffered, -damage);
        SetBloodOverlayAlpha(1f);
        audioSource.PlayOneShot(hurtSound, 0.5f);
    }

    private void SetBloodOverlayAlpha(float alpha)
    {
        Color color = bloodOverlay.color;
        color.a = alpha;
        bloodOverlay.color = color;
    }

    public bool Heal(int amount)
    {
        if (currentHp >= playerStats.maxHp)
        {
            return false;
        }

        photonView.RPC("RequestChangeHp", RpcTarget.AllBuffered, amount);
        return true;
    }

    [PunRPC]
    public void RequestChangeHp(int amount)
    {
        currentHp += amount;
        if (currentHp > playerStats.maxHp)
        {
            currentHp = playerStats.maxHp;
        }
        else if (currentHp < 0)
        {
            currentHp = 0;
            // Handle player death here if needed
        }
        UpdateHpText();
    }

    private void UpdateHpText()
    {
        healthText.text = $"{currentHp}/{playerStats.maxHp}";
        float healthPercentage = (float)currentHp / playerStats.maxHp;
        healthSlider.value = healthPercentage;

        if (healthPercentage < 0.25f)
        {
            healthText.color = Color.red;
            healthSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else if (healthPercentage < 0.5f)
        {
            healthText.color = Color.yellow;
            healthSlider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            healthText.color = Color.white;
            healthSlider.fillRect.GetComponent<Image>().color = Color.white;
        }
    }

    private void Update()
    {
        if (bloodOverlay.color.a > 0.01f)
        {
            Color color = bloodOverlay.color;
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * 3f);
            bloodOverlay.color = color;
        }
        else
        {
            SetBloodOverlayAlpha(0f);
        }
    }
}
