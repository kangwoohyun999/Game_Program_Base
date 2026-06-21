using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 5층 빌딩 전투 관리 (17주차 - 싱글턴, 이벤트)
// 1층: 2마리 ~ 5층: 10마리
public class FloorManager : MonoBehaviour
{
    private static FloorManager m_instance;
    public static FloorManager instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<FloorManager>();
            return m_instance;
        }
    }

    [Header("층 설정")]
    public int totalFloors = 5;
    public int currentFloor = 1;

    [Header("좀비 설정")]
    public Zombie zombiePrefab;
    public ZombieData[] zombieDatas;
    public Transform[] spawnPoints;

    // 1층:2, 2층:4, 3층:6, 4층:8, 5층:10
    private int[] zombieCountPerFloor = { 2, 4, 6, 8, 10 };

    private List<Zombie> aliveZombies = new List<Zombie>();
    private bool floorCleared = false;
    private bool waitingForNextFloor = false;

    private void Start()
    {
        if (zombiePrefab == null)
            Debug.LogError("[FloorManager] zombiePrefab이 연결되지 않았습니다!");
        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogError("[FloorManager] spawnPoints가 비어 있습니다!");

        StartFloor(currentFloor);
    }

    private void Update()
    {
        // 층 클리어 후 F키 대기
        if (waitingForNextFloor && Input.GetKeyDown(KeyCode.F))
        {
            waitingForNextFloor = false;
            UIManager.instance?.HideInteractHint();
            GoToNextFloor();
        }
    }

    public void StartFloor(int floor)
    {
        currentFloor        = floor;
        floorCleared        = false;
        waitingForNextFloor = false;
        aliveZombies.Clear();

        UIManager.instance?.UpdateFloorText(currentFloor);
        UIManager.instance?.HideInteractHint();

        int count = zombieCountPerFloor[Mathf.Clamp(floor - 1, 0, zombieCountPerFloor.Length - 1)];
        UIManager.instance?.UpdateZombieCount(count);

        Debug.Log($"[FloorManager] {floor} Floor Start : {count} Zombies");
        StartCoroutine(SpawnZombies(count));
    }

    private IEnumerator SpawnZombies(int count)
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < count; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(0.4f);
        }
    }

    private void SpawnZombie()
    {
        if (zombiePrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Zombie zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

        if (zombieDatas != null && zombieDatas.Length > 0)
        {
            ZombieData data = zombieDatas[Random.Range(0, zombieDatas.Length)];
            ZombieData scaledData = ScriptableObject.CreateInstance<ZombieData>();
            scaledData.health    = data.health    + (currentFloor - 1) * 30f;
            scaledData.damage    = data.damage    + (currentFloor - 1) * 5f;
            scaledData.speed     = data.speed     + (currentFloor - 1) * 0.3f;
            scaledData.skinColor = data.skinColor;
            zombie.Setup(scaledData);
        }

        zombie.onDeath += () => OnZombieDied(zombie);
        aliveZombies.Add(zombie);
    }

    private void OnZombieDied(Zombie zombie)
    {
        aliveZombies.Remove(zombie);
        UIManager.instance?.UpdateZombieCount(aliveZombies.Count);

        if (aliveZombies.Count <= 0 && !floorCleared)
        {
            floorCleared = true;
            OnFloorCleared();
        }
    }

    private void OnFloorCleared()
    {
        Debug.Log($"[FloorManager] {currentFloor} Floor Clear !");
        UIManager.instance?.ShowFloorClearedMessage(currentFloor);

        // 마지막 층이면 게임 클리어
        if (currentFloor >= totalFloors)
        {
            GameManager.instance?.GameClear();
            return;
        }

        // F키 안내 표시
        waitingForNextFloor = true;
        UIManager.instance?.ShowInteractHint("Press to F : Next Stage");
    }

    public void GoToNextFloor()
    {
        if (currentFloor >= totalFloors) return;
        StartFloor(currentFloor + 1);
    }
}