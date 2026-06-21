using UnityEngine;

// 문 앞 상호작용 인터페이스 (15주차 - 인터페이스)
public interface IInteractable
{
    void Interact();
    string GetInteractHint();
}

// 문 앞에서 F키 → 다음 층 이동 (8주차 - OnTrigger)
public class ElevatorInteraction : MonoBehaviour, IInteractable
{
    public KeyCode interactKey = KeyCode.F;
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
            Interact();
    }

    public void Interact()
    {
        FloorManager.instance?.GoToNextFloor();
    }

    public string GetInteractHint()
    {
        return "F 를 눌러 다음 층으로 이동하세요";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        UIManager.instance?.ShowInteractHint(GetInteractHint());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        UIManager.instance?.HideInteractHint();
    }
}
