using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================
// GameManager - 17주차 게임 매니저 + 싱글턴
// 게임 전체 상태(시작, 게임오버, 재시작) 관리
// ============================================================
public class GameManager : MonoBehaviour
{
    // 싱글턴 (17주차)
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

    public bool isGameOver { get; private set; } = false;

    private void Start()
    {
        // 게임 시작 시 UI 초기화
        UIManager.instance?.UpdateHealthBar(1f);
        UIManager.instance?.UpdateFloorText(1);
        UIManager.instance?.UpdateHotbar(0);
    }

    // 플레이어가 사망하면 호출됨 (PlayerHealth에서 호출)
    public void EndGame()
    {
        if (isGameOver) return;

        isGameOver = true;

        // 게임 멈춤
        Time.timeScale = 0f;

        // 게임오버 UI 표시 (17주차 - UIManager)
        UIManager.instance?.ShowGameover();
    }

    // 재시작 버튼 OnClick에 연결 (17주차 - UI 버튼 OnClick 이벤트)
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 메인 메뉴로 (씬 전환)
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
