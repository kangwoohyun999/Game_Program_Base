using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRigidbody;   // 이동에 사용할 리지드바디
    public float speed = 8f;             // 이동 속력

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();  // 자동으로 가져오기
    }

    void Update()
    {
        // 입력 받기 (WASD/화살표 모두 지원)
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // 속도 계산
        float xSpeed = xInput * speed;
        float zSpeed = zInput * speed;

        Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed);
        playerRigidbody.linearVelocity = newVelocity;   // 즉시 반영
    }

    // 탄알에 맞으면 호출되는 사망 메서드
    public void Die()
    {
        gameObject.SetActive(false);   // 플레이어 비활성화
    }
}