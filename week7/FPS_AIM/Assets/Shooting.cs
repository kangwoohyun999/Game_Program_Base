using UnityEngine;

/// <summary>
/// 좌클릭 → 총알 발사.
/// - 게임이 진행 중일 때만 발사
/// - Pause 중이면 발사 불가
/// - ResultUI가 열려 있으면 발사 불가  (클릭 = 결과창 닫기로 사용)
/// - Input.GetMouseButtonDown(0) 사용 (New Input System 의존 제거)
/// </summary>
public class Shooting : MonoBehaviour
{
    [Header("총 설정")]
    public GameObject bulletPrefab;
    public Transform  firePoint;
    public float      fireRate = 0.15f;

    private float nextFireTime = 0f;

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        if (ResultUI.Instance != null && ResultUI.Instance.gameObject.activeSelf) return;
        if (Time.time < nextFireTime) return;

        if (Input.GetMouseButtonDown(0))
        {
            nextFireTime = Time.time + fireRate;
            GameManager.Instance.RegisterShot();

            if (bulletPrefab != null && firePoint != null)
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
