using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 15f;   // �¿� �̵� �ӵ� (Inspector���� ���� ����)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal") * speed;

        // X�ุ �����̰�, Y/Z ��ġ�� Freeze �Ǿ� �����Ƿ� velocity X�� ����
        Vector3 velocity = rb.linearVelocity;
        velocity.x = move;
        rb.linearVelocity = velocity;
    }
}