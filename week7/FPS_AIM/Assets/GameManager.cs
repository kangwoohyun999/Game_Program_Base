using UnityEngine;
using System.Collections.Generic;

public enum Mode { Easy, Normal, Hard }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("타겟 프리팹")]
    public GameObject targetPrefab;
    public Transform targetsParent;

    [Header("난이도별 스폰 설정")]
    public float easySpawnZ = 15f;
    public float normalSpawnZ = 18f;
    public float hardSpawnZ = 22f;
    public Vector2 normalXRange = new Vector2(-12f, 12f);
    public Vector2 hardXRange = new Vector2(-15f, 15f);
    public float normalY = 1f;
    public Vector2 precisionYRange = new Vector2(0.8f, 4.2f);

    [Header("게임 상태")]
    public bool IsPlaying { get; private set; } = false;
    public float ElapsedTime { get; private set; } = 0f;
    public int CurrentHits { get; private set; } = 0;
    public int TotalTargets { get; private set; } = 10;
    public int ShotsFired { get; private set; } = 0;
    public float Accuracy { get; private set; } = 0f;
    public string FinalRank { get; private set; } = "";

    private Mode currentMode;
    private int targetsOnScreen;
    private List<GameObject> activeTargets = new List<GameObject>();
    private float startTime;
    private float spawnZ;
    private Vector2 xRange;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame(Mode mode, int targetCount)
    {
        currentMode = mode;
        TotalTargets = targetCount;
        CurrentHits = 0;
        ShotsFired = 0;
        IsPlaying = true;
        startTime = Time.time;
        activeTargets.Clear();

        targetsOnScreen = (mode == Mode.Normal) ? 2 : 3;

        // 난이도별 설정 적용
        spawnZ = mode == Mode.Easy ? easySpawnZ : (mode == Mode.Normal ? normalSpawnZ : hardSpawnZ);
        xRange = mode == Mode.Hard ? hardXRange : normalXRange;

        SpawnInitialTargets();
    }

    private void SpawnInitialTargets()
    {
        for (int i = 0; i < targetsOnScreen; i++)
            SpawnOneTarget();
    }

    private void SpawnOneTarget()
    {
        Vector3 pos = GetRandomPosition();
        GameObject target = Instantiate(targetPrefab, pos, Quaternion.identity, targetsParent);

        Target t = target.GetComponent<Target>();
        if (t != null) t.SetMoving(currentMode == Mode.Normal);

        activeTargets.Add(target);
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(xRange.x, xRange.y);
        float z = spawnZ;
        float y = (currentMode == Mode.Hard) ? Random.Range(precisionYRange.x, precisionYRange.y) : normalY;

        return new Vector3(x, y, z);
    }

    public void RegisterShot() => ShotsFired++;

    public void RegisterHit()
    {
        CurrentHits++;
        if (CurrentHits < TotalTargets)
            SpawnOneTarget();
        else
            EndGame();
    }

    private void Update()
    {
        if (IsPlaying)
            ElapsedTime = Time.time - startTime;
    }

    private void EndGame()
    {
        IsPlaying = false;

        foreach (var t in activeTargets)
            if (t != null) Destroy(t);
        activeTargets.Clear();

        Accuracy = (ShotsFired > 0) ? (CurrentHits * 100f / ShotsFired) : 100f;

        float score = Accuracy * (60f / (ElapsedTime + 1f));
        if (score > 280) FinalRank = "상위 1%";
        else if (score > 180) FinalRank = "상위 10%";
        else if (score > 120) FinalRank = "상위 30%";
        else FinalRank = "상위 50%";

        // 결과 UI 표시
        ResultUI.Instance.ShowResult();

        // Pause 상태 초기화
        if (PauseUI.Instance != null)
            PauseUI.Instance.ResetPauseState();
    }

    public void ReturnToSelection()
    {
        IsPlaying = false;
    }

    public string GetCurrentModeName()
    {
        return currentMode switch
        {
            Mode.Easy => "Easy Mode",
            Mode.Normal => "Normal Mode",
            Mode.Hard => "Hard Mode",
            _ => ""
        };
    }
}