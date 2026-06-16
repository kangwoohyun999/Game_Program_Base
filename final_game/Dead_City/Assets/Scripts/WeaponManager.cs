using UnityEngine;
using System.Collections;

// 무기 데이터 (15주차 - GunData 스크립트 참고)
[System.Serializable]
public class WeaponData
{
    public string weaponName;       // 무기 이름 (핫바에 표시)
    public float damage;            // 데미지
    public float fireRate;          // 연사 속도 (초당 발사 횟수)
    public float range;             // 사거리
    public int maxAmmo;             // 최대 탄약
    public int currentAmmo;         // 현재 탄약
}

// ============================================================
// WeaponManager - 핫바 무기 관리 (1, 2, 3번 슬롯)
// ============================================================
public class WeaponManager : MonoBehaviour
{
    [Header("무기 슬롯 (1, 2, 3번)")]
    public WeaponData[] weapons = new WeaponData[3];
    public GameObject[] weaponObjects;  // 실제 무기 게임오브젝트

    [Header("발사 설정")]
    public Transform firePoint;         // 총구 위치
    public LayerMask hitLayers;         // 맞을 수 있는 레이어
    public ParticleSystem muzzleFlash;  // 총구 이펙트
    public LineRenderer bulletTrail;    // 탄도 라인 (15주차)

    private int currentIndex = 0;
    private float lastFireTime;

    private void Start()
    {
        // 탄약 초기화
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].currentAmmo = weapons[i].maxAmmo;
        }

        EquipWeapon(0); // 기본 1번 무기 장착
    }

    private void Update()
    {
        // 마우스 좌클릭으로 발사
        if (Input.GetMouseButton(0))
        {
            Fire();
        }
    }

    // 무기 교체
    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponObjects.Length) return;

        currentIndex = index;

        // 해당 무기만 활성화
        for (int i = 0; i < weaponObjects.Length; i++)
        {
            if (weaponObjects[i] != null)
                weaponObjects[i].SetActive(i == index);
        }

        // UI 탄약 표시 갱신
        UIManager.instance?.UpdateAmmo(
            weapons[currentIndex].currentAmmo,
            weapons[currentIndex].maxAmmo);
    }

    // 발사 (15주차 - 레이캐스트 탄알 발사)
    private void Fire()
    {
        WeaponData currentWeapon = weapons[currentIndex];
        if (currentWeapon == null) return;
        if (currentWeapon.currentAmmo <= 0) return;
        if (Time.time < lastFireTime + 1f / currentWeapon.fireRate) return;

        lastFireTime = Time.time;
        currentWeapon.currentAmmo--;

        // 레이캐스트로 발사 (15주차 핵심)
        RaycastHit hit;
        Vector3 hitPoint = firePoint.position + firePoint.forward * currentWeapon.range;

        if (Physics.Raycast(firePoint.position, firePoint.forward,
                            out hit, currentWeapon.range, hitLayers))
        {
            hitPoint = hit.point;

            // IDamageable 인터페이스로 데미지 (15주차 - 느슨한 커플링)
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(currentWeapon.damage, hit.point, hit.normal);
            }
        }

        // 이펙트
        if (muzzleFlash != null) muzzleFlash.Play();
        StartCoroutine(ShowBulletTrail(firePoint.position, hitPoint)); // 코루틴 (15주차)

        // UI 탄약 업데이트
        UIManager.instance?.UpdateAmmo(
            currentWeapon.currentAmmo,
            currentWeapon.maxAmmo);
    }

    // 코루틴 - 탄도 잠깐 표시 후 숨기기 (15주차 - 코루틴으로 대기 시간)
    private IEnumerator ShowBulletTrail(Vector3 start, Vector3 end)
    {
        if (bulletTrail == null) yield break;

        bulletTrail.enabled = true;
        bulletTrail.SetPosition(0, start);
        bulletTrail.SetPosition(1, end);

        yield return new WaitForSeconds(0.05f);

        bulletTrail.enabled = false;
    }

    // 재장전
    public void Reload()
    {
        WeaponData currentWeapon = weapons[currentIndex];
        if (currentWeapon == null) return;

        currentWeapon.currentAmmo = currentWeapon.maxAmmo;
        UIManager.instance?.UpdateAmmo(currentWeapon.currentAmmo, currentWeapon.maxAmmo);
    }
}
