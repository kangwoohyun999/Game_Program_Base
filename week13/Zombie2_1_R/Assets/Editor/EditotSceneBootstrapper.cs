using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// 에디터가 로딩되거나 스크립트가 컴파일될 때 자동으로 실행되도록 설정
[InitializeOnLoad]
public static class EditorSceneBootstrapper
{
    static EditorSceneBootstrapper()
    {
        // 에디터가 완전히 켜지고 난 뒤(프레임 업데이트가 시작될 때) 실행되도록 이벤트를 등록
        EditorApplication.delayCall += LoadDefaultEditorScene;
    }

    private static void LoadDefaultEditorScene()
    {
        // 현재 에디터에 열려있는 씬이 없거나, 저장되지 않은 빈 씬(Untitled)인 경우에만 작동
        if (EditorSceneManager.GetSceneAt(0).path == "" && EditorSceneManager.sceneCount == 1)
        {
            // 씬의 경로
            string defaultScenePath = "Assets/Scenes/Main.unity";

            // 해당 경로에 진짜 씬 파일이 존재하는지 확인 후 에디터에서 오픈
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(defaultScenePath) != null)
            {
                Debug.Log($"[EditorBootstrapper] 빈 씬 대신 기본 씬을 로드합니다: {defaultScenePath}");
                EditorSceneManager.OpenScene(defaultScenePath);
            }
            else
            {
                Debug.LogWarning($"[EditorBootstrapper] 지정한 씬 경로를 찾을 수 없습니다: {defaultScenePath}. 경로를 확인해주세요.");
            }
        }
    } 
}