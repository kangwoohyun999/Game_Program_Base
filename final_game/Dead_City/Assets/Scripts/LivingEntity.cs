using System;
using UnityEngine;

// 생명체 기반 클래스 (16주차 - 상속, 다형성)
// Zombie2_1_R 원본 기반 유지
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;
    public float health { get; protected set; }
    public bool dead { get; protected set; }
    public event Action onDeath;

    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;
        if (health <= 0 && !dead)
            Die();
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return;
        health += newHealth;
    }

    public virtual void Die()
    {
        if (onDeath != null) onDeath();
        dead = true;
    }
}
