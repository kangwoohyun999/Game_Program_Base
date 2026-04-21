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
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.IsPlaying)
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pausePanel != null) pausePanel.SetActive(true);
        if (CrosshairManager.Instance != null)
            CrosshairManager.Instance.HideCrosshair();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (CrosshairManager.Instance != null)
            CrosshairManager.Instance.ShowCrosshair();
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        GameManager.Instance.ReturnToSelection();
        SelectionUI.Instance.gameObject.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // GameManager에서 호출할 수 있게 public 메서드 추가
    public void ResetPauseState()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
}