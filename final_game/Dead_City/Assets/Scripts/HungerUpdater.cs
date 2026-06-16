using UnityEngine;

// ============================================================
// HungerUpdater - 배고픔 바 실시간 갱신
// PlayerController에 함께 붙이는 보조 컴포넌트
// ============================================================
public class HungerUpdater : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (playerController == null) return;

        // UIManager 싱글턴으로 배고픔바 업데이트 (17주차)
        UIManager.instance?.UpdateHungerBar(
            playerController.hunger,
            playerController.maxHunger);
    }
}
