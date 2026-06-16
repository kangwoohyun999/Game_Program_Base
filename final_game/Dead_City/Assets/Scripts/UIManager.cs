using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ============================================================
// UIManager - 17주차 싱글턴 패턴
// 화면 좌상단: 체력바 + 배고픔바
// 핫바: 하단 중앙 1, 2, 3번 슬롯
// 우상단: 층 정보, 남은 좀비 수
// ============================================================
public class UIManager : MonoBehaviour
{
    // 싱글턴 프로퍼티 (17주차 - 싱글턴)
    private static UIManager m_instance;
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<UIManager>();
            return m_instance;
        }
    }

    [Header("=== 좌상단 플레이어 상태 ===")]
    public Slider healthSlider;         // 체력바
    public Slider hungerSlider;         // 배고픔바
    public TextMeshProUGUI healthText;  // "HP: 80 / 100"
    public TextMeshProUGUI hungerText;  // "배고픔: 60 / 100"

    [Header("=== 하단 핫바 ===")]
    public Image[] hotbarSlots;         // 3개 슬롯 이미지
    public TextMeshProUGUI[] hotbarNames; // 무기 이름 텍스트
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    public TextMeshProUGUI ammoText;    // 탄약 표시

    [Header("=== 우상단 전투 정보 ===")]
    public TextMeshProUGUI floorText;       // "3층"
    public TextMeshProUGUI zombieCountText; // "남은 좀비: 5"

    [Header("=== 중앙 메시지 ===")]
    public TextMeshProUGUI interactHintText; // "F: 다음 층으로"
    public TextMeshProUGUI floorClearedText; // "층 클리어!"

    [Header("=== 게임오버 ===")]
    public GameObject gameoverPanel;

    // ----------------------------------------------------------
    // 체력 / 배고픔 업데이트
    // ----------------------------------------------------------
    public void UpdateHealthBar(float ratio)
    {
        if (healthSlider != null)
            healthSlider.value = ratio;

        // Mathf.RoundToInt로 정수 표시
        if (healthText != null)
            healthText.text = $"HP: {Mathf.RoundToInt(ratio * 100)} / 100";
    }

    public void UpdateHungerBar(float hunger, float maxHunger)
    {
        if (hungerSlider != null)
            hungerSlider.value = hunger / maxHunger;

        if (hungerText != null)
            hungerText.text = $"Food: {Mathf.RoundToInt(hunger)} / {Mathf.RoundToInt(maxHunger)}";
    }

    // ----------------------------------------------------------
    // 핫바 업데이트 (무기 선택)
    // ----------------------------------------------------------
    public void UpdateHotbar(int selectedIndex)
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] != null)
            {
                // 선택된 슬롯은 노란색, 나머지는 흰색
                hotbarSlots[i].color = (i == selectedIndex) ? selectedColor : normalColor;
            }
        }
    }

    // ----------------------------------------------------------
    // 탄약 표시
    // ----------------------------------------------------------
    public void UpdateAmmo(int current, int max)
    {
        if (ammoText != null)
            ammoText.text = $"{current} / {max}";
    }

    // ----------------------------------------------------------
    // 층 / 좀비 정보
    // ----------------------------------------------------------
    public void UpdateFloorText(int floor)
    {
        if (floorText != null)
            floorText.text = $"{floor} Floor";
    }

    public void UpdateZombieCount(int count)
    {
        if (zombieCountText != null)
            zombieCountText.text = $"Left : {count}";
    }

    // ----------------------------------------------------------
    // 층 클리어 메시지 (코루틴으로 잠깐 표시)
    // ----------------------------------------------------------
    public void ShowFloorClearedMessage(int floor)
    {
        if (floorClearedText == null) return;

        floorClearedText.text = $"{floor} Floor Clear !";
        floorClearedText.gameObject.SetActive(true);
        Invoke(nameof(HideFloorClearedMessage), 2f); // 2초 후 숨기기
    }

    private void HideFloorClearedMessage()
    {
        if (floorClearedText != null)
            floorClearedText.gameObject.SetActive(false);
    }

    // ----------------------------------------------------------
    // 상호작용 힌트 (엘리베이터 등)
    // ----------------------------------------------------------
    public void ShowInteractHint(string hint)
    {
        if (interactHintText != null)
        {
            interactHintText.text = hint;
            interactHintText.gameObject.SetActive(true);
        }
    }

    public void HideInteractHint()
    {
        if (interactHintText != null)
            interactHintText.gameObject.SetActive(false);
    }

    // ----------------------------------------------------------
    // 게임오버 UI
    // ----------------------------------------------------------
    public void ShowGameover()
    {
        if (gameoverPanel != null)
            gameoverPanel.SetActive(true);
    }
}
