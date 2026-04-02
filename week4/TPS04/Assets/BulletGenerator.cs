using UnityEngine;

public class BulletGenerator : MonoBehaviour   
{
    public GameObject bulletPrefab;

    public float bulletSpeed = 20f;     

    void Update()
    {
        if (Input.GetMouseButtonDown(0))   
        {

            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

            bullet.GetComponent<BulletController>().Shoot(transform.forward * bulletSpeed);
        }
    }
}