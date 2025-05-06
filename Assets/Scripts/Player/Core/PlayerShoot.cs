using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Threading.Tasks;

public class PlayerShoot : MonoBehaviourPunCallbacks
{
    [Header("Main")]
    [SerializeField] private PlayerSO playerStats;
    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerLook playerLook;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI currentTotalAmmoText;
    [SerializeField] private TextMeshProUGUI gunModeText;

    [Header("Audio Effects")]
    [SerializeField] private AudioSource gunAudioSource;
    [SerializeField] private AudioClip gunShotClip;
    [SerializeField] private AudioClip emptyClip;
    [SerializeField] private AudioClip reloadClip;
    [SerializeField][Range(0f, 1f)] private float volume = 0.1f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 100f;

    [Header("Ammo")]
    [SerializeField] private int currentAmmo;
    [SerializeField] private int currentTotalAmmo;

    [Header("State")]
    [SerializeField] private GunMode gunMode = GunMode.Single;
    [SerializeField] private bool isShooting = false;
    private int burstCount = 3;
    private bool isReloading = false;
    private float fireTimer = 0f;

    [Header("Particle")]
    [SerializeField] private ParticleSystem hitParticle;

    new private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponentInParent<PhotonView>();
        ConfigureAudioSource();

        if (playerStats)
        {
            currentAmmo = playerStats.defaultMagazineSize;
            currentTotalAmmo = playerStats.defaultNumberOfBullet;
        }
        UpdateBulletText();
    }

    private void Update()
    {
        if (!isShooting || !photonView.IsMine) return;
        fireTimer -= Time.deltaTime;

        if (gunMode == GunMode.Auto)
        {
            if (fireTimer > 0f) return;
            fireTimer = playerStats.fireRate;
            ProcessShot();
            return;
        }

        if (gunMode == GunMode.Burst)
        {
            if (fireTimer > 0f) return;
            if (burstCount > 0)
            {
                fireTimer = playerStats.fireRate;
                ProcessShot();
                burstCount--;
            }
            else
            {
                StopShoot();
            }
            return;
        }
        fireTimer = 0;
    }

    private void ConfigureAudioSource()
    {
        if (gunAudioSource == null)
        {
            gunAudioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource component was added automatically to the player.");
        }

        // Set up 3D sound properties
        gunAudioSource.spatialBlend = 1f; // Full 3D sound
        gunAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        gunAudioSource.minDistance = minDistance;
        gunAudioSource.maxDistance = maxDistance;
        
        gunAudioSource.playOnAwake = false;
    }

    public void ShootStart()
    {
        if (isReloading) return;
        isShooting = true;
        if (gunMode == GunMode.Single) ProcessShot();
    }

    public void ShootEnd()
    {
        if (isReloading || gunMode == GunMode.Burst) return;
        StopShoot();
    }

    private void StopShoot()
    {
        isShooting = false;
        burstCount = 3;
    }

    public async void Reload()
    {
        if (isReloading || currentTotalAmmo <= 0) return;
        isReloading = true;
        StopShoot();
        gunAudioSource.PlayOneShot(reloadClip, volume);
        await Task.Delay((int)(playerStats.reloadTime * 1000));
        int totalAmmo = currentTotalAmmo + currentAmmo;
        int neededAmmo = playerStats.defaultMagazineSize - currentAmmo;
        if (totalAmmo <= 0)
        {
            return;
        }
        else if (totalAmmo <= 30)
        {
            currentAmmo = totalAmmo;
            currentTotalAmmo = 0;
        }
        else
        {
            currentAmmo = playerStats.defaultMagazineSize;
            currentTotalAmmo -= neededAmmo;
        }
        UpdateBulletText();
        isReloading = false;
    }

    public void AddAmmos(int amount)
    {
        currentTotalAmmo += amount;
        UpdateBulletText();
    }

    private enum GunMode
    {
        Single,
        Burst,
        Auto
    }

    public void ChangeGunMode()
    {
        gunMode = (GunMode)(((int)gunMode + 1) % 3);
        switch(gunMode)
        {
            case GunMode.Single:
                gunModeText.text = "Single";
                break;
            case GunMode.Burst:
                gunModeText.text = "Burst";
                break;
            case GunMode.Auto:
                gunModeText.text = "Auto";
                break;
        }
        gunAudioSource.PlayOneShot(emptyClip, volume);
    }

    private void ProcessShot()
    {
        if (currentAmmo <= 0)
        {
            gunAudioSource.PlayOneShot(emptyClip, volume);
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, playerStats.shootRange, playerStats.enemyLayer))
        {
            IEnemyDamageable enemy = hit.collider.GetComponentInParent<IEnemyDamageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(playerStats.damage, hit.collider, transform.parent.gameObject);

                if (hitParticle != null)
                {
                    ParticleSystem hitEffect = Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
        currentAmmo--;
        UpdateBulletText();

        float recoil = Random.Range(1f, 3f);
        playerLook.AddRecoil(recoil);

        if (animator != null)
        {
            animator.SetTrigger("FiringSingle");
        }
        photonView.RPC("RPC_PlayShootSound", RpcTarget.All);
    }

    private void UpdateBulletText()
    {
        if (currentAmmoText != null)
        {
            currentAmmoText.text = $"{currentAmmo}";

            if (currentAmmo <= 0)
            {
                currentAmmoText.color = Color.red;
            }
            else if (currentAmmo <= 10)
            {
                currentAmmoText.color = Color.yellow;
            }
            else
            {
                currentAmmoText.color = Color.white;
            }
        }
        if (currentTotalAmmoText != null)
        {
            currentTotalAmmoText.text = $"{currentTotalAmmo}";

            if (currentTotalAmmo <= 0)
            {
                currentTotalAmmoText.color = Color.red;
            }
            else if (currentTotalAmmo <= 10)
            {
                currentTotalAmmoText.color = Color.yellow;
            }
            else
            {
                currentTotalAmmoText.color = Color.white;
            }
        }
    }

    [PunRPC]
    public void RPC_PlayShootSound()
    {
        gunAudioSource.PlayOneShot(gunShotClip, volume);
    }
}