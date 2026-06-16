using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// 16주차 - Zombie: LivingEntity 상속, NavMesh AI
// 다형성 - LivingEntity 타입으로 관리 가능
public class Zombie : LivingEntity
{
    [Header("AI 설정")]
    public float damage = 20f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public float updatePathInterval = 0.5f; // 경로 갱신 주기

    [Header("머리 위 체력바")]
    public Slider healthBarSlider;    // 프리팹에 미리 연결된 Slider
    public Canvas healthBarCanvas;    // 빌보드용 Canvas

    private LivingEntity targetEntity;
    private NavMeshAgent navAgent;
    private Animator animator;
    private float lastAttackTime;
    private float lastPathUpdateTime;

    protected override void OnEnable()
    {
        base.OnEnable(); // LivingEntity OnEnable (체력 초기화)
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 체력바 초기화
        if (healthBarSlider != null)
            healthBarSlider.value = 1f;
    }

    private void Start()
    {
        // 플레이어를 타겟으로 설정
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            targetEntity = player.GetComponent<LivingEntity>();
    }

    private void Update()
    {
        if (dead) return;

        // 체력바가 항상 카메라를 바라보도록 (빌보드 효과)
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.rotation =
                Camera.main.transform.rotation;
        }

        // 주기적 경로 갱신 (16주차 - 내비게이션 시스템)
        if (Time.time >= lastPathUpdateTime + updatePathInterval)
        {
            lastPathUpdateTime = Time.time;
            ChaseTarget();
        }

        // 공격 범위 확인
        if (targetEntity != null && !targetEntity.dead)
        {
            float dist = Vector3.Distance(transform.position,
                                         targetEntity.transform.position);
            if (dist <= attackRange)
            {
                Attack();
            }
        }

        // 애니메이션 (11주차)
        if (animator != null && navAgent != null)
        {
            animator.SetFloat("Speed", navAgent.velocity.magnitude);
        }
    }

    // NavMesh로 플레이어 추적 (16주차)
    private void ChaseTarget()
    {
        if (targetEntity != null && !targetEntity.dead && navAgent != null)
        {
            navAgent.SetDestination(targetEntity.transform.position);
        }
    }

    // 플레이어 공격
    private void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        // 느슨한 커플링 - IDamageable 인터페이스로 공격 (15주차)
        if (targetEntity != null)
        {
            targetEntity.OnDamage(damage, transform.position, Vector3.zero);
        }

        if (animator != null)
            animator.SetTrigger("Attack");
    }

    // 16주차 - override 피격 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);

        // 머리 위 체력바 업데이트
        if (healthBarSlider != null)
            healthBarSlider.value = health / startingHealth;

        if (animator != null)
            animator.SetTrigger("Hit");
    }

    // 16주차 - override 사망 처리
    public override void Die()
    {
        base.Die(); // LivingEntity.Die() → onDeath 이벤트 발생

        if (animator != null)
            animator.SetTrigger("Die");

        if (navAgent != null)
            navAgent.enabled = false;

        // 체력바 숨기기
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        // 콜라이더 비활성화
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 3초 후 오브젝트 제거 (코루틴 - 15주차)
        Destroy(gameObject, 3f);
    }
}
