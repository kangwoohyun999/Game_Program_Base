using UnityEngine;
using UnityEngine.Events;

// IDamageable 인터페이스 (15주차 - 느슨한 커플링)
public interface IDamageable
{
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}

// LivingEntity - 16주차 기반 클래스 (다형성, 상속)
// 플레이어와 좀비가 공통으로 상속
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;
    public float health { get; protected set; }
    public bool dead { get; protected set; }

    // 이벤트 - 17주차 (익명 함수, 람다식 활용)
    public event UnityAction onDeath;

    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    // virtual - 자식 클래스에서 override 가능 (16주차)
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return;
        health += newHealth;
        health = Mathf.Clamp(health, 0f, startingHealth);
    }

    public virtual void Die()
    {
        onDeath?.Invoke(); // 이벤트 실행
        dead = true;
    }
}
