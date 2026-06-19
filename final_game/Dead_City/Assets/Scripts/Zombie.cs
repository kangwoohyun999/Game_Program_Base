using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// 좀비 AI (Zombie2_1_R 기반 + 머리 위 체력바 + 층별 스케일)
// 16주차 - LivingEntity 상속, NavMesh AI
public class Zombie : LivingEntity
{
    [Header("AI 설정")]
    public LayerMask whatIsTarget;
    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;

    [Header("이펙트 / 사운드")]
    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;

    [Header("머리 위 체력바")]
    public Slider healthBarSlider;
    public Transform healthBarTransform;

    // 컴포넌트 참조
    private LivingEntity targetEntity;
    private NavMeshAgent navMeshAgent;
    private Animator zombieAnimator;
    private AudioSource zombieAudioPlayer;
    private Renderer zombieRenderer;
    private Camera mainCamera;

    // 추적 대상 존재 여부
    private bool hasTarget =>
        targetEntity != null && !targetEntity.dead;

    private void Awake()
    {
        navMeshAgent    = GetComponent<NavMeshAgent>();
        zombieAnimator  = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();
        zombieRenderer  = GetComponentInChildren<Renderer>();
        mainCamera      = Camera.main;
    }

    // 층 스케일 Setup (FloorManager에서 호출)
    public void Setup(ZombieData data)
    {
        startingHealth = data.health;
        health         = data.health;
        damage         = data.damage;
        navMeshAgent.speed = data.speed;
        if (zombieRenderer != null)
            zombieRenderer.material.color = data.skinColor;

        // 체력바 초기화
        if (healthBarSlider != null)
            healthBarSlider.value = 1f;
    }

    private void Start()
    {
        // Rigidbody가 있으면 Kinematic으로 (NavMesh와 충돌 방지)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (dead) return;

        // 체력바 빌보드 (항상 카메라를 향함)
        if (healthBarTransform != null && mainCamera != null)
            healthBarTransform.rotation = mainCamera.transform.rotation;

        // Animator 파라미터
        if (zombieAnimator != null)
            zombieAnimator.SetBool("HasTarget", hasTarget);
    }

    // 코루틴 - 경로 갱신 (15주차 - 코루틴)
    private IEnumerator UpdatePath()
    {
        while (!dead)
        {
            if (hasTarget)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                navMeshAgent.isStopped = true;

                // 범위 안의 타겟 탐색 (Physics.OverlapSphere - 15주차)
                Collider[] colliders =
                    Physics.OverlapSphere(transform.position, 20f, whatIsTarget);

                foreach (Collider col in colliders)
                {
                    LivingEntity entity = col.GetComponent<LivingEntity>();
                    if (entity != null && !entity.dead)
                    {
                        targetEntity = entity;
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    // 피격 처리 (16주차 - override)
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            // 이펙트
            if (hitEffect != null)
            {
                hitEffect.transform.position = hitPoint;
                hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
                hitEffect.Play();
            }
            if (zombieAudioPlayer != null && hitSound != null)
                zombieAudioPlayer.PlayOneShot(hitSound);
        }

        base.OnDamage(damage, hitPoint, hitNormal);

        // 머리 위 체력바 갱신
        if (healthBarSlider != null)
            healthBarSlider.value = health / startingHealth;
    }

    // 사망 처리 (16주차 - override)
    public override void Die()
    {
        base.Die(); // onDeath 이벤트 발생 → FloorManager가 카운트 차감

        // 콜라이더 전부 비활성화
        foreach (Collider col in GetComponents<Collider>())
            col.enabled = false;

        navMeshAgent.isStopped = true;
        navMeshAgent.enabled   = false;

        if (zombieAnimator != null)
            zombieAnimator.SetTrigger("Die");

        if (zombieAudioPlayer != null && deathSound != null)
            zombieAudioPlayer.PlayOneShot(deathSound);

        // 체력바 숨기기
        if (healthBarTransform != null)
            healthBarTransform.gameObject.SetActive(false);

        Destroy(gameObject, 3f);
    }

    // 근접 공격 (OnTriggerStay - 8주차)
    private void OnTriggerStay(Collider other)
    {
        if (dead) return;
        if (Time.time < lastAttackTime + timeBetAttack) return;

        LivingEntity target = other.GetComponent<LivingEntity>();
        if (target != null && target == targetEntity)
        {
            lastAttackTime = Time.time;
            Vector3 hitPoint  = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - other.transform.position;
            target.OnDamage(damage, hitPoint, hitNormal);
        }
    }
}
