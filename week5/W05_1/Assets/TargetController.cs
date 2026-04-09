using UnityEngine;

public class TargetController : MonoBehaviour
{
    public ParticleSystem targetExplosion;
    public int health = 2;          // ← 여기서 1로 하면 1번, 2로 하면 2번 맞아야 폭파

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("SHELL"))
        {
            health--;                   // 체력 감소

            if (health <= 0)            // 체력 0 이하일 때만 폭파
            {
                ParticleSystem fire = Instantiate(targetExplosion, transform.position, Quaternion.identity);
                fire.Play();
                Destroy(gameObject);
            }
        }
    }
}