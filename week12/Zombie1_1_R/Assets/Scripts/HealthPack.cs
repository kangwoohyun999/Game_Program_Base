using UnityEngine;

// 체력을 회복하는 아이템
public class HealthPack : MonoBehaviour, IItem
{
    public float health = 50f; // 체력을 회복할 수치

    public void Use(GameObject target)
    {
        // 전달받은 게임 오브젝트로부터 LivingEntity 컴포넌트 가져오기
        LivingEntity life = target.GetComponent<LivingEntity>();

        // LivingEntity 컴포넌트가 있다면 체력 회복
        if (life != null)
        {
            life.RestoreHealth(health);

            // 선택사항: 회복 효과음이나 파티클 재생 가능
            // AudioSource나 ParticleSystem 등을 여기서 호출할 수 있음
        }

        // 사용 후 아이템 파괴
        Destroy(gameObject);
    }
}