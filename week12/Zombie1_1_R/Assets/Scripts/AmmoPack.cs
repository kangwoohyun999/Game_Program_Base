using UnityEngine;

// 총알을 충전하는 아이템
public class AmmoPack : MonoBehaviour, IItem
{
    public int ammo = 30; // 충전할 총알 수

    public void Use(GameObject target)
    {
        // 전달 받은 게임 오브젝트로부터 PlayerShooter 컴포넌트를 가져오기
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();

        // PlayerShooter가 있고, 현재 장착된 총이 있으면 탄약 충전
        if (playerShooter != null && playerShooter.currentGun != null)
        {
            playerShooter.currentGun.ammoRemain += ammo;

            // 탄약 UI 즉시 갱신
            if (UIManager.instance != null)
            {
                UIManager.instance.UpdateAmmoText(
                    playerShooter.currentGun.magAmmo,
                    playerShooter.currentGun.ammoRemain);
            }
        }

        // 사용 후 아이템 파괴
        Destroy(gameObject);
    }
}