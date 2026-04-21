using UnityEngine;
using System.Collections.Generic;

public enum Mode { Easy, Normal, Hard }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("타겟 프리팹")]
    public GameObject targetPrefab;
    public Transform  targetsParent;

    [Header("스폰 위치 설정")]
    public float   easySpawnZ   = 15f;
    public float   normalSpawnZ = 18f;
    public float   hardSpawnZ   = 22f;
    public Vector2 normalXRange = new Vector2(-12f, 12f);
    public Vector2 hardXRange   = new Vector2(-15f, 15f);
    public float   normalY      = 1f;
    public Vector2 hardYRange   = new Vector2(0.8f, 4.2f);

    [Header("Hard 모드 타겟 크기 배율 (기본 1.0)")]
    public float hardTargetScale = 0.35f;

    [Header("게임 상태 (읽기 전용)")]
    public bool   IsPlaying    { get; private set; } = false;
    public float  ElapsedTime  { get; private set; } = 0f;
    public int    CurrentHits  { get; private set; } = 0;
    public int    TotalTargets { get; private set; } = 10;
    public int    ShotsFired   { get; private set; } = 0;
    public float  Accuracy     { get; private set; } = 0f;
    public string FinalRank    { get; private set; } = "";

    private Mode             currentMode;
    private int              targetsOnScreen;
    private List<GameObject> activeTargets = new List<GameObject>();
    private float            startTime;
    private float            spawnZ;
    private Vector2          xRange;

    private void Awake() => Instance = this;

    // ─────────────────────────────────────────────────
    public void StartGame(Mode mode, int targetCount)
    {
        currentMode    = mode;
        TotalTargets   = targetCount;
        CurrentHits    = 0;
        ShotsFired     = 0;
        IsPlaying      = true;
        startTime      = Time.time;
        activeTargets.Clear();

        targetsOnScreen = (mode == Mode.Normal) ? 2 : 3;

        spawnZ = mode == Mode.Easy   ? easySpawnZ  :
                 mode == Mode.Normal ? normalSpawnZ : hardSpawnZ;
        xRange = mode == Mode.Hard   ? hardXRange  : normalXRange;

        SpawnInitialTargets();

        // 게임 시작 시 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void SpawnInitialTargets()
    {
        for (int i = 0; i < targetsOnScreen; i++)
            SpawnOneTarget();
    }

    private void SpawnOneTarget()
    {
        Vector3    pos    = GetRandomPosition();
        GameObject target = Instantiate(targetPrefab, pos, Quaternion.identity, targetsParent);

        // Hard 모드: 작은 구
        if (currentMode == Mode.Hard)
            target.transform.localScale *= hardTargetScale;

        Target t = target.GetComponent<Target>();
        if (t != null) t.SetMode(currentMode);

        activeTargets.Add(target);
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(xRange.x, xRange.y);
        float z = spawnZ;
        float y = (currentMode == Mode.Hard)
                  ? Random.Range(hardYRange.x, hardYRange.y)
                  : normalY;
        return new Vector3(x, y, z);
    }

    // ─────────────────────────────────────────────────
    public void RegisterShot() => ShotsFired++;

    public void RegisterHit()
    {
        if (!IsPlaying) return;

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

        InGameUI.Instance.ForceUpdateHitText(CurrentHits, TotalTargets);

        foreach (var t in activeTargets)
            if (t != null) Destroy(t);
        activeTargets.Clear();

        Accuracy = (ShotsFired > 0) ? (CurrentHits * 100f / ShotsFired) : 100f;

        float score = Accuracy * (60f / (ElapsedTime + 1f));
        if      (score > 280) FinalRank = "Top 1%";
        else if (score > 180) FinalRank = "Top 10%";
        else if (score > 120) FinalRank = "Top 30%";
        else                  FinalRank = "Top 50%";

        ResultUI.Instance.ShowResult();

        if (PauseUI.Instance != null)
            PauseUI.Instance.ResetPauseState();

        // 결과 화면 커서
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void ReturnToSelection()
    {
        IsPlaying = false;

        foreach (var t in activeTargets)
            if (t != null) Destroy(t);
        activeTargets.Clear();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public string GetCurrentModeName() => currentMode switch
    {
        Mode.Easy   => "Easy Mode",
        Mode.Normal => "Normal Mode",
        Mode.Hard   => "Hard Mode",
        _           => ""
    };
}
