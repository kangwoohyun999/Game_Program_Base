using UnityEngine;
using UnityEngine.UI;
using TMPro;

// UI 통합 관리 - 싱글턴 (17주차)
public class UIManager : MonoBehaviour
{
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

    [Header("=== 좌상단: 플레이어 상태 ===")]
    public Slider healthSlider;
    public Slider hungerSlider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI hungerText;

    [Header("=== 우상단: 전투 정보 ===")]
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI zombieCountText;
    public TextMeshProUGUI ammoText;

    [Header("=== 하단 핫바 ===")]
    public Image[] hotbarSlots;
    public Color selectedColor = Color.yellow;
    public Color normalColor   = Color.white;

    [Header("=== 중앙 메시지 ===")]
    public TextMeshProUGUI interactHintText;
    public TextMeshProUGUI floorClearedText;

    [Header("=== 게임오버 / 클리어 ===")]
    public GameObject gameoverPanel;
    public GameObject gameClearPanel;

    private void Start()
    {
        // 시작 시 숨겨야 할 UI 초기화
        if (gameoverPanel  != null) gameoverPanel.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.SetActive(false);
        if (interactHintText != null) interactHintText.gameObject.SetActive(false);
        if (floorClearedText != null) floorClearedText.gameObject.SetActive(false);
    }

    // ── 체력 ──────────────────────────────────────
    public void UpdateHealthBar(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        if (healthSlider != null) healthSlider.value = ratio;
        if (healthText   != null) healthText.text    = $"HP  {Mathf.RoundToInt(ratio * 100)} / 100";
    }

    // ── 배고픔 ────────────────────────────────────
    public void UpdateHungerBar(float hunger, float maxHunger)
    {
        if (hungerSlider != null) hungerSlider.value = hunger / maxHunger;
        if (hungerText   != null) hungerText.text    = $"Food  {Mathf.RoundToInt(hunger)} / {Mathf.RoundToInt(maxHunger)}";
    }

    // ── 핫바 ──────────────────────────────────────
    public void UpdateHotbar(int selectedIndex)
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
            if (hotbarSlots[i] != null)
                hotbarSlots[i].color = (i == selectedIndex) ? selectedColor : normalColor;
    }

    // ── 탄약 (PlayerShooter: magAmmo / ammoRemain) ──
    public void UpdateAmmoText(int magAmmo, int ammoRemain)
    {
        if (ammoText != null) ammoText.text = $"{magAmmo} / {ammoRemain}";
    }

    // ── 층 / 좀비 수 ──────────────────────────────
    public void UpdateFloorText(int floor)
    {
        if (floorText != null) floorText.text = $"{floor} / 5 Floor";
    }

    public void UpdateZombieCount(int count)
    {
        if (zombieCountText != null) zombieCountText.text = $"Zombie : {count}";
    }

    // ── 층 클리어 메시지 ──────────────────────────
    public void ShowFloorClearedMessage(int floor)
    {
        if (floorClearedText == null) return;
        floorClearedText.text = $"{floor} Floor Clear !";
        floorClearedText.gameObject.SetActive(true);
        Invoke(nameof(HideFloorClearedMessage), 2.5f);
    }

    private void HideFloorClearedMessage()
    {
        if (floorClearedText != null)
            floorClearedText.gameObject.SetActive(false);
    }

    // ── 문 앞 상호작용 힌트 ───────────────────────
    public void ShowInteractHint(string hint)
    {
        if (interactHintText == null) return;
        interactHintText.text = hint;
        interactHintText.gameObject.SetActive(true);
    }

    public void HideInteractHint()
    {
        if (interactHintText != null)
            interactHintText.gameObject.SetActive(false);
    }

    // ── 게임오버 / 클리어 ─────────────────────────
    public void ShowGameover()
    {
        if (gameoverPanel != null) gameoverPanel.SetActive(true);
    }

    public void ShowGameClear()
    {
        if (gameClearPanel != null) gameClearPanel.SetActive(true);
    }

    // ── Zombie2_1_R 원본 호환 ─────────────────────
    public void UpdateWaveText(int wave, int count)
    {
        if (zombieCountText != null)
            zombieCountText.text = $"Wave {wave},  Zombie : {count}";
    }

    public void SetActiveGameoverUI(bool active)
    {
        if (gameoverPanel != null) gameoverPanel.SetActive(active);
    }

    public void UpdateScoreText(int score) { }
}
