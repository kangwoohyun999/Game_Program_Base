using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance;

    [Header("Pause Panel")]
    public GameObject pausePanel;

    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
        // 반드시 비활성화 상태로 시작
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        // ResultUI가 열려 있으면 ESC 무시
        if (ResultUI.Instance != null && ResultUI.Instance.gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.IsPlaying)
        {
            if (IsPaused) ResumeGame();
            else          PauseGame();
        }
    }

    private void PauseGame()
    {
        IsPaused         = true;
        Time.timeScale   = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        if (pausePanel != null) pausePanel.SetActive(true);
        if (CrosshairManager.Instance != null) CrosshairManager.Instance.HideCrosshair();
    }

    public void ResumeGame()
    {
        IsPaused         = false;
        Time.timeScale   = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (CrosshairManager.Instance != null) CrosshairManager.Instance.ShowCrosshair();
    }

    public void ReturnToMenu()
    {
        ResetPauseState();
        GameManager.Instance.ReturnToSelection();

        InGameUI.Instance.gameObject.SetActive(false);
        SelectionUI.Instance.gameObject.SetActive(true);

        if (pausePanel != null) pausePanel.SetActive(false);
    }

    /// <summary>게임 종료 / 재시작 시 호출</summary>
    public void ResetPauseState()
    {
        IsPaused       = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }
}
