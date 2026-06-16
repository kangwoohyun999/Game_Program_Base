using UnityEngine;

// ============================================================
// ElevatorInteraction - 엘리베이터 상호작용
// 플레이어가 범위 안에서 F키를 누르면 다음 층으로 이동
// 8주차 - OnTriggerEnter/Exit, 15주차 - 인터페이스(IInteractable)
// ============================================================

// 상호작용 인터페이스 (15주차 - 느슨한 커플링)
public interface IInteractable
{
    void Interact();
    string GetInteractHint(); // 힌트 텍스트 반환 ("F: 다음 층으로" 등)
}

public class ElevatorInteraction : MonoBehaviour, IInteractable
{
    [Header("상호작용 설정")]
    public KeyCode interactKey = KeyCode.F;

    // 플레이어가 범위 안에 있는지 여부
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    // IInteractable 구현
    public void Interact()
    {
        // FloorManager에게 다음 층 이동 요청
        FloorManager.instance?.GoToNextFloor();
    }

    public string GetInteractHint()
    {
        return "F : 다음 층으로 이동";
    }

    // 트리거 충돌 처리 (8주차 - OnTrigger)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // 상호작용 힌트 UI 표시
            UIManager.instance?.ShowInteractHint(GetInteractHint());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            UIManager.instance?.HideInteractHint();
        }
    }
}
