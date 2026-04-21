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

    [Header("타겟 개수 버튼")]
    public Button btn10;
    public Button btn20;
    public Button btn30;

    [Header("시작 버튼")]
    public Button btnStart;

    [Header("경고 메시지")]
    public TextMeshProUGUI warningText;

    private Mode selectedMode = (Mode)(-1);
    private int selectedCount = 0;

    private void Awake()
    {
        Instance = this;

        // Null 체크 추가
        if (warningText != null)
            warningText.text = "";
    }

    private void Start()
    {
        // 버튼 리스너 등록
        btnEasy.onClick.AddListener(() => SelectMode(Mode.Easy));
        btnNormal.onClick.AddListener(() => SelectMode(Mode.Normal));
        btnHard.onClick.AddListener(() => SelectMode(Mode.Hard));

        btn10.onClick.AddListener(() => SelectCount(10));
        btn20.onClick.AddListener(() => SelectCount(20));
        btn30.onClick.AddListener(() => SelectCount(30));

        if (btnStart != null)
            btnStart.onClick.AddListener(OnStartButtonClick);
    }

    private void SelectMode(Mode mode)
    {
        selectedMode = mode;
        if (warningText != null) warningText.text = "";
    }

    private void SelectCount(int count)
    {
        selectedCount = count;
        if (warningText != null) warningText.text = "";
    }

    private void OnStartButtonClick()
    {
        if (selectedMode == (Mode)(-1))
        {
            if (warningText != null)
            {
                warningText.text = "모드를 선택해주세요!";
                warningText.color = Color.red;
            }
            return;
        }

        if (selectedCount == 0)
        {
            if (warningText != null)
            {
                warningText.text = "원하는 개수를 선택해주세요!";
                warningText.color = Color.red;
            }
            return;
        }

        if (warningText != null) warningText.text = "";

        gameObject.SetActive(false);
        InGameUI.Instance.Show();
        GameManager.Instance.StartGame(selectedMode, selectedCount);
    }
}