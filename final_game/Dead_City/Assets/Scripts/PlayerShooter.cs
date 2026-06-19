using UnityEngine;

// 총 발사 / 재장전 / IK 처리 (15주차 - 레이캐스트, IK)
public class PlayerShooter : MonoBehaviour
{
    public Gun gun;
    public Transform gunPivot;
    public Transform leftHandMount;
    public Transform rightHandMount;
    public GunData[] invenGunData; // 무기 인벤토리 (1, 2번 슬롯)

    private PlayerInput playerInput;
    private Animator playerAnimator;

    private void Start()
    {
        playerInput    = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();

        // 시작 시 핫바 0번 강조
        UIManager.instance?.UpdateHotbar(0);
    }

    private void OnEnable()
    {
        if (gun != null) gun.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (gun != null) gun.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 발사
        if (playerInput.fire)
            gun.Fire();
        // 재장전
        else if (playerInput.reload)
        {
            if (gun.Reload())
                playerAnimator.SetTrigger("Reload");
        }

        // 무기 교체 (1, 2번 키)
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);

        // 3점사 토글 (3번: on, 4번: off)
        if (Input.GetKeyDown(KeyCode.Alpha3)) gun.bBurst = true;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) gun.bBurst = false;

        // 탄약 UI 매 프레임 갱신
        UIManager.instance?.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
    }

    private void ChangeWeapon(int i)
    {
        if (invenGunData == null || i < 0 || i >= invenGunData.Length) return;
        if (invenGunData[i] == null) return;

        gun.ChangeGun(invenGunData[i]);
        UIManager.instance?.UpdateHotbar(i);
    }

    // IK - 양손이 총 손잡이에 위치하도록 (11주차 - 애니메이션 IK)
    private void OnAnimatorIK(int layerIndex)
    {
        if (gunPivot != null)
            gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        if (leftHandMount != null)
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);
        }

        if (rightHandMount != null)
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);
        }
    }
}
