using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;

    // ★★★ 8-3 심화: Enemy가 3번 맞아야 넘어지게 하는 카운트 ★★★
    private static int enemyHitCount = 0;   // static으로 해서 모든 총알이 공유

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Enemy와 충돌 (3번 맞아야 넘어짐)
        if (collision.gameObject.CompareTag("ENEMY"))
        {
            enemyHitCount++;                    // 맞은 횟수 증가
            Debug.Log("Enemy Hit Count: " + enemyHitCount);   // 확인용 (나중에 지워도 OK)

            if (enemyHitCount >= 2)
            {
                Rigidbody enemyRd = collision.gameObject.GetComponent<Rigidbody>();
                if (enemyRd != null)
                {
                    enemyRd.constraints = RigidbodyConstraints.None;  // Freeze Rotation 해제 → 넘어짐
                }
                enemyHitCount = 0;   // 게임 재시작 대비 초기화
            }

            Destroy(gameObject);   // 총알 사라짐
        }

        // 2. Player와 충돌 (기존처럼 1발 맞으면 바로 넘어짐)
        else if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRd = GameObject.Find("Player").GetComponent<Rigidbody>();
            if (playerRd != null)
            {
                playerRd.constraints = RigidbodyConstraints.None;
            }
            Destroy(gameObject);
        }
    }
}