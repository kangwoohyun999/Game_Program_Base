using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// 좀비 AI 구현
public class Zombie : LivingEntity
{
    public LayerMask whatIsTarget; // 추적 대상 레이어

    private LivingEntity targetEntity; // 추적 대상
    private NavMeshAgent navMeshAgent; // 경로 계산 AI 에이전트

    public ParticleSystem hitEffect; // 피격 시 재생할 파티클 효과
    public AudioClip deathSound; // 사망 시 재생할 소리
    public AudioClip hitSound; // 피격 시 재생할 소리

    private Animator zombieAnimator; // 애니메이터 컴포넌트
    private AudioSource zombieAudioPlayer; // 오디오 소스 컴포넌트
    private Renderer zombieRenderer; // 렌더러 컴포넌트

    public float damage = 20f; // 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime; // 마지막 공격 시점

    // ==================== 새로 추가된 부분 ====================
    // 상태 관리
    public enum State { Idle, Chase, Attack, Flee }
    public State currentState = State.Idle;

    // 도망(Flee) 관련
    private Transform playerTransform;
    private float fleeDistance = 6f;           // 도망 거리
    private float fleeSpeedMultiplier = 1.3f;  // 30% 더 빠르게

    // =========================================================

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget {
        get
        {
            if (targetEntity != null && !targetEntity.dead)
                return true;
            return false;
        }
    }

    private void Awake() 
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();
        zombieRenderer = GetComponentInChildren<Renderer>();
    }

    // 좀비 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(ZombieData zombieData) 
    {
        startingHealth = zombieData.health;
        health = zombieData.health;
        damage = zombieData.damage;
        navMeshAgent.speed = zombieData.speed;
        zombieRenderer.material.color = zombieData.skinColor;
    }

    private void Start() 
    {
        StartCoroutine(UpdatePath());

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            StartCoroutine(UpdatePath());
    }

    private void Update() 
    {
        if (dead) return;

        // ==================== 체력 체크 → Flee 상태 전환 ====================
        if (health <= startingHealth * 0.3f && currentState != State.Flee)
        {
            EnterFleeState();
        }

        zombieAnimator.SetBool("HasTarget", hasTarget);
    }

    // ==================== Flee 상태 진입 ====================
    private void EnterFleeState()
    {
        currentState = State.Flee;
        navMeshAgent.speed = navMeshAgent.speed * fleeSpeedMultiplier; // 도망 속도 증가
        zombieRenderer.material.color = Color.blue;                   // 파란색으로 변경
    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로 갱신
    private IEnumerator UpdatePath() 
    {
        while (!dead)
        {
            if (currentState == State.Flee)
            {
                UpdateFleeBehavior();
            }
            else if (hasTarget)
            {
                currentState = State.Chase;
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                currentState = State.Idle;
                navMeshAgent.isStopped = true;

                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);

                for (int i = 0; i < colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();

                    if (livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    // 도망(Flee) 행동 업데이트
    private void UpdateFleeBehavior()
    {
        if (playerTransform == null) return;

        Vector3 awayFromPlayer = (transform.position - playerTransform.position).normalized;
        Vector3 fleeTarget = transform.position + awayFromPlayer * fleeDistance;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(fleeTarget);
    }

    // 데미지를 입었을 때 실행할 처리 (넉백 + 스턴 추가)
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) 
    {
        if (!dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            zombieAudioPlayer.PlayOneShot(hitSound);
        }
        
        // LivingEntity의 OnDamage() 실행 (체력 감소, 사망 체크)
        base.OnDamage(damage, hitPoint, hitNormal);

        // 피격 시 넉백 + 일시 정지 효과
        if (!dead)
        {
            StartCoroutine(KnockbackAndStun(hitNormal));
        }
    }

    // 넉백 + 스턴 코루틴
    private IEnumerator KnockbackAndStun(Vector3 hitNormal)
    {
        navMeshAgent.isStopped = true;                    // 이동 멈춤

        // 살짝 뒤로 밀림 (넉백)
        transform.position += hitNormal * -3f * Time.deltaTime;

        yield return new WaitForSeconds(0.2f);            // 0.2초 동안 스턴

        // 죽지 않았고, Flee 상태가 아니라면 다시 이동 허용
        if (!dead && currentState != State.Flee)
        {
            navMeshAgent.isStopped = false;
        }
    }

    // 사망 처리
    public override void Die() 
    {
        base.Die();

        Collider[] zombieColliders = GetComponents<Collider>();
        for (int i = 0; i < zombieColliders.Length; i++)
        {
            zombieColliders[i].enabled = false;
        }

        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;

        zombieAnimator.SetTrigger("Die");
        zombieAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other) 
    {
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();

            if (attackTarget != null && attackTarget == targetEntity)
            {
                lastAttackTime = Time.time;
                
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;

                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}