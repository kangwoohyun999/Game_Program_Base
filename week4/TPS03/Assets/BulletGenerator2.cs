using UnityEngine;

public class BulletGenerator2 : MonoBehaviour
{
    public GameObject bulletPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

            // PDF 지시: Player를 향한 방향으로 발사
            Vector3 dir = GameObject.Find("Player").transform.position - this.transform.position;
            bullet.GetComponent<BulletController>().Shoot(dir);
        }
    }
}