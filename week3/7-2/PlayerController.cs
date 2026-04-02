using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;   // 좌우 이동 속도

    void Update()
    {
        // PDF 지시대로 transform.Translate 사용 (AddForce와 비교 테스트 가능)
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(move, 0f, 0f);
    }
}