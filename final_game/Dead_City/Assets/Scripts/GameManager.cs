using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 상태 관리 - 싱글턴 (17주차)
public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<GameManager>();
            return m_instance;
        }
    }

    public bool isGameover { get; private set; } = false;

    private void Awake()
    {
        if (instance != this) { Destroy(gameObject); return; }
    }

    private void Start()
    {
        // 플레이어 사망 이벤트 구독 (17주차 - 이벤트)
        PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
        if (ph != null) ph.onDeath += EndGame;

        // UI 초기화
        UIManager.instance?.UpdateHealthBar(1f);
        UIManager.instance?.UpdateFloorText(1);
        UIManager.instance?.UpdateHotbar(0);
        UIManager.instance?.UpdateZombieCount(0);
    }

    public void EndGame()
    {
        if (isGameover) return;
        isGameover = true;
        Time.timeScale = 0f;
        UIManager.instance?.ShowGameover();
    }

    public void GameClear()
    {
        Time.timeScale = 0f;
        UIManager.instance?.ShowGameClear();
    }

    // 재시작 버튼 OnClick 연결
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
