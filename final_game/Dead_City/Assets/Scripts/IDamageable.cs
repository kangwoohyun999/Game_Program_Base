using UnityEngine;

// 데미지를 입을 수 있는 타입 인터페이스 (15주차 - 느슨한 커플링)
// Zombie2_1_R 원본 그대로 유지
public interface IDamageable
{
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}
