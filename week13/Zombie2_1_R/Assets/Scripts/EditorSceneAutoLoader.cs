using UnityEngine;
using UnityEngine.SceneManagement;

public static class EditorSceneAutoLoader
{
    // 게임이 런타임에 시작되기 '직전'에 자동으로 실행되는 메서드
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void FirstSceneLoad()
    {
        // [File]메뉴의 Build Profiles의 'Scene List'의 첫번째
        int firstSceneIndex = 0;

        // 현재 이미 0번째 씬이 로드되어 있는 상태가 아니라면, 0번째 씬을 강제로 로드
        if (SceneManager.GetActiveScene().buildIndex != firstSceneIndex)
        {
            SceneManager.LoadScene(firstSceneIndex);
        }
    }
}
