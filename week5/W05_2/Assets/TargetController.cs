using Tanks.Complete;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public ParticleSystem targetExplosion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "SHELL")
        {
            ParticleSystem fire = Instantiate(targetExplosion,
                transform.position, Quaternion.identity);
            fire.Play();

            Destroy(fire.gameObject, 2.0f);
            Destroy(gameObject);
        }

    }
}
