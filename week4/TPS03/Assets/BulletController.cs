using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;

    // Awake()로 미리 Rigidbody를 가져오면 Instantiate 직후에도 안전
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("BulletPrefab에 Rigidbody가 없습니다! 추가해주세요.");
        }
    }

    // 총알 발사 함수 (8장 1~3번 모두 사용)
    public void Shoot(Vector3 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction;   // 이제 null이 아님
        }
        else
        {
            Debug.LogError("BulletController: Rigidbody를 찾을 수 없습니다.");
        }
    }

    // 8장 2번 9번에서 사용한 Player 충돌 처리 (그대로 유지)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ENEMY"))
        {
            Rigidbody enemyRd = collision.gameObject.GetComponent<Rigidbody>();
            if (enemyRd != null)
                enemyRd.constraints = RigidbodyConstraints.None;

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRd = GameObject.Find("Player").GetComponent<Rigidbody>();
            if (playerRd != null)
                playerRd.constraints = RigidbodyConstraints.None;

            Destroy(gameObject);
        }
    }
}