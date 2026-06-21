using UnityEngine;
using UnityEngine.UI;

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity
{
    public AudioClip deathClip;
    public AudioClip hitClip;
    public AudioClip itemPickupClip;

    private AudioSource playerAudioPlayer;
    private Animator playerAnimator;
    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;

    private void Awake()
    {
        playerAnimator    = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerMovement    = GetComponent<PlayerMovement>();
        playerShooter     = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // UIManager로 체력바 초기화 (healthSlider 직접 참조 제거)
        UIManager.instance?.UpdateHealthBar(1f);

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerShooter  != null) playerShooter.enabled  = true;
    }

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        UIManager.instance?.UpdateHealthBar(health / startingHealth);
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead && playerAudioPlayer != null && hitClip != null)
            playerAudioPlayer.PlayOneShot(hitClip);

        base.OnDamage(damage, hitPoint, hitNormal);

        // UIManager로 체력바 갱신
        UIManager.instance?.UpdateHealthBar(health / startingHealth);
    }

    public override void Die()
    {
        base.Die(); // onDeath 이벤트 → GameManager.EndGame 호출

        if (playerAudioPlayer != null && deathClip != null)
            playerAudioPlayer.PlayOneShot(deathClip);

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Die");

        if (playerMovement != null) playerMovement.enabled = false;
        if (playerShooter  != null) playerShooter.enabled  = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!dead)
        {
            IItem item = other.GetComponent<IItem>();
            if (item != null)
            {
                item.Use(gameObject);
                if (playerAudioPlayer != null && itemPickupClip != null)
                    playerAudioPlayer.PlayOneShot(itemPickupClip);
            }
        }
    }
}
