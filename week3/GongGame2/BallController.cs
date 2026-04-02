using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 심화: 게임 시작 시 임의의 위치·방향으로 출발 (PDF 요구사항)
        float randomX = Random.Range(-10f, 10f);
        float randomZ = Random.Range(-15f, -10f);
        rb.linearVelocity = new Vector3(randomX, 0f, randomZ).normalized * 15f;  // 초기 속도 15
    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        // 1. 벽 또는 패들과 부딪히면 강제로 튕겨나가게 (Vector3.Reflect)
        if (tag == "WALL" || tag == "PLAYER")
        {
            if (collision.contacts.Length > 0)
            {
                Vector3 normal = collision.contacts[0].normal;
                rb.linearVelocity = Vector3.Reflect(rb.linearVelocity, normal);

                // 살짝 속도 보정 (더 빠르고 경쾌하게 튕김)
                rb.linearVelocity = rb.linearVelocity.normalized * 16f;
            }
        }

        // 2. 벽돌(BLOCK)과 부딪히면 파괴 + 튕김
        else if (tag == "BLOCK")
        {
            if (collision.contacts.Length > 0)
            {
                Vector3 normal = collision.contacts[0].normal;
                rb.linearVelocity = Vector3.Reflect(rb.linearVelocity, normal);
                rb.linearVelocity = rb.linearVelocity.normalized * 16f;
            }

            Destroy(collision.gameObject);   // PDF 요구사항
        }

        // 공이 바닥 아래로 떨어지면 제거
        if (transform.position.y < -2f)
            Destroy(gameObject);
    }

    void Update()
    {
        // 심화: 모든 블록이 없어지면 공도 사라짐 (PDF 요구사항)
        if (GameObject.FindGameObjectsWithTag("BLOCK").Length == 0)
        {
            Destroy(gameObject);
        }
    }
}