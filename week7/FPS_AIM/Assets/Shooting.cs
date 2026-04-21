using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [Header("ÃÑ ¼³Á¤")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.15f;

    private float nextFireTime = 0f;

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        if (Time.time < nextFireTime) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            nextFireTime = Time.time + fireRate;
            GameManager.Instance.RegisterShot();

            if (bulletPrefab != null && firePoint != null)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            }
        }
    }
}