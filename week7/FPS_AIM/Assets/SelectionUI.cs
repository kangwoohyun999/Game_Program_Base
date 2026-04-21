using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionUI : MonoBehaviour
{
    public static SelectionUI Instance;

    [Header("모드 버튼")]
    public Button btnEasy;
    public Button btnNormal;
    public Button btnHard;

    [Header("타겟 수 버튼")]
    public Button btn10;
    public Button btn20;
    public Button btn30;

    [Header("시작 버튼")]
    public Button btnStart;

    [Header("경고 텍스트")]
    public TextMeshProUGUI warningText;

    [Header("선택 색상")]
    public Color selectedColor   = new Color(0.2f, 0.8f, 0.2f, 1f);   // 초록
    public Color deselectedColor = new Color(0.3f, 0.3f, 0.3f, 1f);   // 회색

    private Mode selectedMode  = (Mode)(-1);
    private int  selectedCount = 0;

    private void Awake()
    {
        Instance = this;
        if (warningText != null) warningText.text = "";
    }

    private void Start()
    {
        btnEasy.onClick.AddListener(  () => SelectMode(Mode.Easy));
        btnNormal.onClick.AddListener(() => SelectMode(Mode.Normal));
        btnHard.onClick.AddListener(  () => SelectMode(Mode.Hard));

        btn10.onClick.AddListener(() => SelectCount(10));
        btn20.onClick.AddListener(() => SelectCount(20));
        btn30.onClick.AddListener(() => SelectCount(30));

        if (btnStart != null)
            btnStart.onClick.AddListener(OnStartButtonClick);

        // 초기 색상 리셋
        ResetModeColors();
        ResetCountColors();
    }

    // 모드 버튼 진입할 때마다 색 초기화 (다른 씬에서 돌아올 때)
    private void OnEnable()
    {
        selectedMode  = (Mode)(-1);
        selectedCount = 0;
        ResetModeColors();
        ResetCountColors();
        if (warningText != null) warningText.text = "";
    }

    // ── 모드 선택 ──────────────────────────────────
    private void SelectMode(Mode mode)
    {
        selectedMode = mode;
        if (warningText != null) warningText.text = "";

        ResetModeColors();
        Button target = mode == Mode.Easy   ? btnEasy  :
                        mode == Mode.Normal ? btnNormal : btnHard;
        SetButtonColor(target, selectedColor);
    }

    // ── 타겟 수 선택 ────────────────────────────────
    private void SelectCount(int count)
    {
        selectedCount = count;
        if (warningText != null) warningText.text = "";

        ResetCountColors();
        Button target = count == 10 ? btn10 :
                        count == 20 ? btn20 : btn30;
        SetButtonColor(target, selectedColor);
    }

    // ── 시작 ────────────────────────────────────────
    private void OnStartButtonClick()
    {
        if (selectedMode == (Mode)(-1))
        {
            ShowWarning("모드를 선택해 주세요!");
            return;
        }
        if (selectedCount == 0)
        {
            ShowWarning("타겟 수를 선택해 주세요!");
            return;
        }

        if (warningText != null) warningText.text = "";

        gameObject.SetActive(false);
        InGameUI.Instance.Show();
        GameManager.Instance.StartGame(selectedMode, selectedCount);
    }

    // ── 헬퍼 ────────────────────────────────────────
    private void ShowWarning(string msg)
    {
        if (warningText != null)
        {
            warningText.text  = msg;
            warningText.color = Color.red;
        }
    }

    private void ResetModeColors()
    {
        SetButtonColor(btnEasy,   deselectedColor);
        SetButtonColor(btnNormal, deselectedColor);
        SetButtonColor(btnHard,   deselectedColor);
    }

    private void ResetCountColors()
    {
        SetButtonColor(btn10, deselectedColor);
        SetButtonColor(btn20, deselectedColor);
        SetButtonColor(btn30, deselectedColor);
    }

    private void SetButtonColor(Button btn, Color color)
    {
        if (btn == null) return;
        ColorBlock cb = btn.colors;
        cb.normalColor      = color;
        cb.highlightedColor = color * 1.2f;
        cb.pressedColor     = color * 0.8f;
        cb.selectedColor    = color;
        btn.colors          = cb;
    }
}
