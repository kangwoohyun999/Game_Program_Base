using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]
    public float speed    = 120f;
    public float lifeTime = 2f;

    private Rigidbody rb;

    private void Awake() => rb = GetComponent<Rigidbody>();

    private void Start()
    {
        if (rb != null)
        {
            rb.useGravity   = false;   // 중력 무시 (직선 비행)
            rb.linearVelocity = transform.forward * speed;
        }
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            Target t = other.GetComponent<Target>();
            if (t != null) t.Hit();
            Destroy(gameObject);
        }
    }
}
