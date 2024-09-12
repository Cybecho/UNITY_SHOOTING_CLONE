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
            SpawnEnemy();                               // 적 스폰 함수 호출
            maxSpawnDelay = Random.Range(0.5f, 3.0f);   // 최대 스폰 딜레이 랜덤 설정
            curSpawnDelay = 0;                          // 현재 스폰 딜레이 초기화
        }
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);                      // 랜덤 적 오브젝트 인덱스
        int ranPoint = Random.Range(0, 5);                      // 랜덤 스폰 위치 인덱스
        Instantiate(enemyObjs[ranEnemy], 
                    spawnPoints[ranPoint].position, 
                    spawnPoints[ranPoint].rotation);            // 적 오브젝트 생성
        
    }
}
