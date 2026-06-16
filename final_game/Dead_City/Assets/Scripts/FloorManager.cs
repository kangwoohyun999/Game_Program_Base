using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ============================================================
// FloorManager - 층 관리
// 층마다 좀비 수 증가, 엘리베이터 상호작용으로 다음 층 이동
// 17주차 - 좀비 생성기 + 게임 매니저 구조 참고
// ============================================================
public class FloorManager : MonoBehaviour
{
    // 싱글턴 (17주차 - 싱글턴 프로퍼티)
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
    public int currentFloor = 1;
    public int zombiesPerFloorBase = 3;   // 1층 기본 좀비 수
    public int zombiesPerFloorAdd = 2;    // 층마다 추가되는 좀비 수

    [Header("좀비 프리팹")]
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;       // 스폰 포인트들

    [Header("엘리베이터")]
    public GameObject elevatorInteractUI; // "F키: 다음 층으로" UI

    // 현재 층의 살아있는 좀비 수
    private int remainingZombies = 0;
    private bool floorCleared = false;

    private void Start()
    {
        StartFloor(currentFloor);
    }

    // 층 시작 - 좀비 스폰
    public void StartFloor(int floor)
    {
        currentFloor = floor;
        floorCleared = false;
        remainingZombies = 0;

        // 엘리베이터 UI 비활성화
        if (elevatorInteractUI != null)
            elevatorInteractUI.SetActive(false);

        // 층 정보 UI 업데이트 (17주차 - UIManager)
        UIManager.instance?.UpdateFloorText(currentFloor);

        // 스폰할 좀비 수 계산 (층마다 증가)
        int spawnCount = zombiesPerFloorBase + (floor - 1) * zombiesPerFloorAdd;

        StartCoroutine(SpawnZombies(spawnCount));
    }

    // 코루틴 - 좀비 순차 스폰 (15주차 - 코루틴)
    private IEnumerator SpawnZombies(int count)
    {
        yield return new WaitForSeconds(1f); // 잠깐 대기 후 스폰

        for (int i = 0; i < count; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(0.3f);
        }
    }

    // 좀비 하나 스폰 (8주차 - 프리팹 인스턴스화)
    private void SpawnZombie()
    {
        if (zombiePrefab == null || spawnPoints.Length == 0) return;

        // 랜덤 스폰 포인트 선택 (17주차 - 랜덤 위치)
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject zombieObj = Instantiate(zombiePrefab,
                                           spawnPoint.position,
                                           spawnPoint.rotation);

        // 좀비 능력치를 층에 따라 스케일업
        Zombie zombie = zombieObj.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.startingHealth = 50f + (currentFloor - 1) * 20f;
            zombie.damage = 10f + (currentFloor - 1) * 5f;
            zombie.onDeath += OnZombieDied; // 이벤트 구독 (17주차)
        }

        remainingZombies++;
    }

    // 좀비 사망 이벤트 핸들러 (17주차 - 이벤트)
    private void OnZombieDied()
    {
        remainingZombies--;

        UIManager.instance?.UpdateZombieCount(remainingZombies);

        // 모든 좀비 처치 시 층 클리어
        if (remainingZombies <= 0 && !floorCleared)
        {
            floorCleared = true;
            OnFloorCleared();
        }
    }

    // 층 클리어 처리
    private void OnFloorCleared()
    {
        UIManager.instance?.ShowFloorClearedMessage(currentFloor);

        // 엘리베이터 상호작용 UI 활성화
        if (elevatorInteractUI != null)
            elevatorInteractUI.SetActive(true);
    }

    // 다음 층으로 이동 (엘리베이터 상호작용 후 호출)
    public void GoToNextFloor()
    {
        StartFloor(currentFloor + 1);
    }
}
