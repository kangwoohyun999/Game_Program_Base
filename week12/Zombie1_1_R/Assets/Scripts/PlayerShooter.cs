using UnityEngine;
using System.Collections;

public enum FireMode { Single, Burst }

public class PlayerShooter : MonoBehaviour
{
    [Header("기본 참조")]
    public Transform gunPivot;
    public Transform leftHandMount;
    public Transform rightHandMount;

    [Header("총기 목록")]
    public Gun uzi;
    public Gun shotgun;
    public Gun sniper;

    [Header("현재 총")]
    public Gun currentGun;

    private PlayerInput playerInput;
    private Animator playerAnimator;

    private FireMode currentFireMode = FireMode.Single;  // 기본은 단발
    private bool isBurstFiring = false;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();

        DeactivateAllGuns();
        EquipGun(uzi);
    }

    private void DeactivateAllGuns()
    {
        if (shotgun != null) shotgun.gameObject.SetActive(false);
        if (sniper != null) sniper.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 총기 교체
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipGun(uzi);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipGun(shotgun);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipGun(sniper);

        // 모드 전환 : 마우스 우클릭
        if (Input.GetMouseButtonDown(1) && currentGun == uzi)
        {
            ToggleFireMode();
        }

        // 발사 처리
        if (playerInput.fire && currentGun != null)
        {
            if (currentFireMode == FireMode.Single)
            {
                currentGun.Fire();
            }
            else if (currentFireMode == FireMode.Burst && !isBurstFiring)
            {
                StartCoroutine(BurstFireRoutine());
            }
        }

        // 재장전
        if (playerInput.reload && currentGun != null)
        {
            if (currentGun.Reload())
            {
                playerAnimator.SetTrigger("Reload");
            }
        }

        UpdateUI();
    }

    // 모드 전환 (우클릭)
    private void ToggleFireMode()
    {
        currentFireMode = (currentFireMode == FireMode.Single) ? FireMode.Burst : FireMode.Single;

        string modeName = currentFireMode == FireMode.Burst ? "3점사 모드" : "연사 모드";
        Debug.Log($"사격 모드 변경 : {modeName}");

        // 선택: UI에 모드 표시 기능 추가 가능
    }

    private void EquipGun(Gun newGun)
    {
        if (newGun == null || newGun == currentGun) return;

        if (currentGun != null)
            currentGun.gameObject.SetActive(false);

        currentGun = newGun;
        currentGun.gameObject.SetActive(true);

        Debug.Log($"총기 교체 : {currentGun.name}");
    }

    // 3점사 코루틴
    private IEnumerator BurstFireRoutine()
    {
        isBurstFiring = true;

        for (int i = 0; i < 3; i++)
        {
            if (currentGun == null) break;

            if (currentGun.magAmmo > 0)
            {
                currentGun.Fire();
                // Debug.Log($"Burst Shot {i + 1}/3 발사됨");
            }
            else
            {
                currentGun.Reload();
                break;
            }

            yield return new WaitForSeconds(0.05f);   // 3점사 간격 (조정 가능)
        }

        isBurstFiring = false;
    }

    private void UpdateUI()
    {
        if (currentGun != null && UIManager.instance != null)
        {
            UIManager.instance.UpdateAmmoText(currentGun.magAmmo, currentGun.ammoRemain);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (currentGun == null) return;

        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);
    }
}