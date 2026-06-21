using UnityEngine;

// 총 발사 / 재장전 / 무기 교체 / IK 처리
// 1번: 권총, 2번: 라이플, 3번: 샷건
public class PlayerShooter : MonoBehaviour
{
    [Header("총 컴포넌트")]
    public Gun gun;           // 발사 로직 담당 (하나만 사용)
    public Transform gunPivot;
    public Transform leftHandMount;
    public Transform rightHandMount;

    [Header("무기 슬롯 (1=권총, 2=라이플, 3=샷건)")]
    public GunData[]      invenGunData;   // GunData ScriptableObject 3개
    public GameObject[]   weaponModels;   // 실제 무기 3D 모델 3개

    private int currentWeaponIndex = 0;
    private PlayerInput playerInput;
    private Animator playerAnimator;

    private void Start()
    {
        playerInput    = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();

        // 시작 시 1번 무기(권총) 장착
        EquipWeapon(0);
    }

    private void OnEnable()
    {
        // 현재 무기 모델만 활성화
        ShowWeaponModel(currentWeaponIndex);
    }

    private void OnDisable()
    {
        // 전체 무기 모델 비활성화
        HideAllWeaponModels();
    }

    private void Update()
    {
        if (gun == null) return;

        // 발사
        if (playerInput.fire)
            gun.Fire();
        // 재장전
        else if (playerInput.reload)
        {
            if (gun.Reload())
                playerAnimator?.SetTrigger("Reload");
        }

        // 1 / 2 / 3 키로 무기 교체
        if (Input.GetKeyDown(KeyCode.Alpha1))      EquipWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        // 탄약 UI 갱신
        UIManager.instance?.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
    }

    // 무기 교체 핵심 메서드
    private void EquipWeapon(int index)
    {
        // 범위 체크
        if (invenGunData == null || index >= invenGunData.Length) return;
        if (invenGunData[index] == null) return;

        currentWeaponIndex = index;

        // 1. GunData 교체 (능력치 변경)
        gun.ChangeGun(invenGunData[index]);

        // 2. 무기 모델 교체 (해당 모델만 활성화)
        ShowWeaponModel(index);

        // 3. 핫바 UI 강조
        UIManager.instance?.UpdateHotbar(index);

        // 4. IK 타겟 갱신 - 현재 무기 모델 안의 HandMount를 찾아서 교체
        if (weaponModels != null && index < weaponModels.Length && weaponModels[index] != null)
        {
            Transform model = weaponModels[index].transform;

            // 모델 안에 LeftHandMount, RightHandMount가 있으면 자동 연결
            Transform lh = model.Find("LeftHandMount");
            Transform rh = model.Find("RightHandMount");
            if (lh != null) leftHandMount  = lh;
            if (rh != null) rightHandMount = rh;
        }

        Debug.Log($"[PlayerShooter] 무기 교체: {invenGunData[index].name}");
    }

    // 해당 인덱스 모델만 켜고 나머지 끄기
    private void ShowWeaponModel(int index)
    {
        if (weaponModels == null) return;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i] != null)
                weaponModels[i].SetActive(i == index);
        }
    }

    private void HideAllWeaponModels()
    {
        if (weaponModels == null) return;
        foreach (GameObject model in weaponModels)
            if (model != null) model.SetActive(false);
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