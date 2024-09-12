using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;      // 적 오브젝트 배열
    public Transform[] spawnPoints;     // 스폰 위치 배열

    public float maxSpawnDelay;         // 최대 스폰 딜레이
    public float curSpawnDelay;         // 현재 스폰 딜레이

    void Update()
    {
        curSpawnDelay += Time.deltaTime;    // 현재 스폰 딜레이 증가

        if (curSpawnDelay > maxSpawnDelay)  // 현재 스폰 딜레이가 최대 스폰 딜레이보다 크면
        {
            SpawnEnemies();                               // 적 스폰 함수 호출
            maxSpawnDelay = Random.Range(0.5f, 3.0f);     // 최대 스폰 딜레이 랜덤 설정
            curSpawnDelay = 0;                            // 현재 스폰 딜레이 초기화
        }
    }

    void SpawnEnemies()
    {
        int baseEnemyCount = 3; // 최소 스폰 적 수
        int additionalEnemyCount = Random.Range(0, 3); // 추가 스폰 적 수 (0~2)
        int totalEnemyCount = baseEnemyCount + additionalEnemyCount; // 총 스폰 적 수

        List<int> availablePoints = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availablePoints.Add(i);
        }

        for (int i = 0; i < totalEnemyCount; i++)
        {
            if (availablePoints.Count == 0) break; // 사용 가능한 스폰 지점이 없으면 종료

            int ranEnemy = Random.Range(0, enemyObjs.Length); // 랜덤 적 오브젝트 인덱스
            int ranIndex = Random.Range(0, availablePoints.Count); // 랜덤 스폰 위치 인덱스
            int ranPoint = availablePoints[ranIndex];

            Instantiate(enemyObjs[ranEnemy], 
                        spawnPoints[ranPoint].position, 
                        spawnPoints[ranPoint].rotation); // 적 오브젝트 생성

            availablePoints.RemoveAt(ranIndex); // 사용한 스폰 지점 제거
        }
    }
}