using UnityEngine;

// 16주차 - PlayerHealth: LivingEntity 상속 + override
public class PlayerHealth : LivingEntity
{
    [Header("무적 시간")]
    public float invincibleTime = 1f;
    private float lastDamageTime = -10f;

    // 컴포넌트 (3주차)
    private Animator animator;

    protected override void OnEnable()
    {
        base.OnEnable(); // 부모 OnEnable 실행 (16주차 - base 키워드)
        animator = GetComponent<Animator>();
    }

    // 16주차 - override로 부모 메서드 재정의
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // 무적 시간 체크
        if (Time.time < lastDamageTime + invincibleTime) return;

        base.OnDamage(damage, hitPoint, hitNormal); // 부모의 체력 감소 로직 실행

        lastDamageTime = Time.time;

        // UIManager 싱글턴으로 HP바 업데이트 (17주차)
        UIManager.instance?.UpdateHealthBar(health / startingHealth);

        if (animator != null)
            animator.SetTrigger("Hit");
    }

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        UIManager.instance?.UpdateHealthBar(health / startingHealth);
    }

    public override void Die()
    {
        base.Die(); // 부모의 Die() 실행 (이벤트 발생)
        if (animator != null)
            animator.SetTrigger("Die");

        // 게임 매니저에 게임오버 알림 (17주차)
        GameManager.instance?.EndGame();
    }
}
