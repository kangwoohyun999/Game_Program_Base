using System.Collections.Generic;
using UnityEngine;

// 좀비 게임 오브젝트를 주기적으로 생성
public class ZombieSpawner : MonoBehaviour 
{
    public Zombie zombiePrefab;           // 생성할 좀비 원본 프리팹
    public ZombieData[] zombieDatas;      // 사용할 좀비 셋업 데이터들
    public Transform[] spawnPoints;       // 좀비 AI를 소환할 위치들

    private List<Zombie> zombies = new List<Zombie>(); // 생성된 좀비들을 담는 리스트
    private int wave = 0;                 // 현재 웨이브

    private void Update() 
    {
        // 게임 오버 상태일때는 생성하지 않음
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        // 좀비를 모두 물리친 경우 다음 웨이브 실행
        if (zombies.Count <= 0)
        {
            SpawnWave();
        }

        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() 
    {
        UIManager.instance.UpdateWaveText(wave, zombies.Count);
    }

    // 현재 웨이브에 맞춰 좀비들을 생성
    private void SpawnWave() 
    {
        wave++;  // 웨이브 증가

        // 이번 웨이브에 생성할 좀비 수 (예: 웨이브당 3~8마리)
        int spawnCount = Mathf.RoundToInt(wave * 1.5f) + 3;

        for (int i = 0; i < spawnCount; i++)
        {
            CreateZombie();
        }
    }

    // 좀비를 생성하고 생성한 좀비에게 추적할 대상을 할당
    private void CreateZombie() 
    {
        // 랜덤한 스폰 위치 선택
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 좀비 생성
        Zombie zombie = Instantiate(zombiePrefab, 
                                    spawnPoint.position, 
                                    spawnPoint.rotation);

        // ZombieData 중 랜덤으로 하나 선택해서 Setup
        ZombieData data = zombieDatas[Random.Range(0, zombieDatas.Length)];
        zombie.Setup(data);

        // 리스트에 추가 (죽었을 때 제거하기 위해)
        zombies.Add(zombie);

        // 좀비가 죽었을 때 리스트에서 제거하는 이벤트 등록
        zombie.onDeath += () => zombies.Remove(zombie);
    }
}