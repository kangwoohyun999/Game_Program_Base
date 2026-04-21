using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    public static ResultUI Instance;

    [Header("결과 UI 텍스트")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI rankText;

    // 클릭 처리를 한 프레임 지연하기 위한 플래그
    private bool waitingForClick = false;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowResult()
    {
        gameObject.SetActive(true);
        waitingForClick = false;

        timeText.text = $"Time : {GameManager.Instance.ElapsedTime:F2}s";
        accuracyText.text = $"Accuracy : {GameManager.Instance.Accuracy:F1}%";
        rankText.text = GameManager.Instance.FinalRank;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (CrosshairManager.Instance != null)
            CrosshairManager.Instance.HideCrosshair();
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        // ShowResult()가 호출된 프레임은 건너뜀 (총 발사 클릭과 겹침 방지)
        if (!waitingForClick)
        {
            waitingForClick = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.SetActive(false);
            InGameUI.Instance.gameObject.SetActive(false);
            GameManager.Instance.ReturnToSelection();
            SelectionUI.Instance.gameObject.SetActive(true);
        }
    }
}
