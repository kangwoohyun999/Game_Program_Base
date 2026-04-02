using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ��ȭ: ������ ��ġ���������� ���
        float randomX = Random.Range(-8f, 8f);
        float randomZ = Random.Range(-12f, -8f);   // �Ʒ������� ���ư���
        rb.linearVelocity = new Vector3(randomX, 0f, randomZ);
    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        // PDF ����: PLAYER �±׵� WALL�� �����ϰ� ó��
        if (tag == "WALL" || tag == "PLAYER")
        {
            // ���� �浹�� �ڿ������� ƨ��� �� (�ʿ�� velocity ���� �ڵ� �߰� ����)
            Debug.Log("�� �Ǵ� �÷��̾�� �浹");
        }
    }

    void Update()
    {
        // ���� �ٴ� �Ʒ��� �������� ���� (���� ���� ����)
        if (transform.position.y < -2f)
            Destroy(gameObject);
    }
}