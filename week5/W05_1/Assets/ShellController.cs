using UnityEngine;

public class ShellController : MonoBehaviour
{
    public ParticleSystem shellExplosion;

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
        ParticleSystem fire = Instantiate(shellExplosion, 
            transform.position, Quaternion.identity /*transform.rotation*/); // 차이점 확인 
        fire.Play();

        Destroy(fire.gameObject, 2.0f);
        Destroy(gameObject); 
    }
}
