using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;      // 적 오브젝트 배열
    public Transform[] spawnPoints;     // 스폰 위치 배열

    public float spawnInterval;         // 스폰 주기 (초 단위)
    private Coroutine spawnCoroutine;   // 스폰 코루틴을 저장할 변수
    private bool isSpawningPaused = false; // 스폰 일시정지 여부

    private Background[] backgrounds;   // 배경 스크롤 스크립트 배열

    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnEnemiesRoutine()); // Coroutine 시작
        backgrounds = FindObjectsOfType<Background>(); // 모든 배경 스크롤 스크립트 가져오기
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            if (!isSpawningPaused)
            {
                SpawnEnemies(); // 적 스폰 함수 호출
            }
            yield return new WaitForSeconds(spawnInterval); // 스폰 주기만큼 대기
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

    // 스폰 일시정지
    public void PauseSpawning()
    {
        isSpawningPaused = true;
    }

    // 스폰 일시정지 해제
    public void ResumeSpawning()
    {
        isSpawningPaused = false; 
    }
    
    // 배경 스크롤 일시정지
    public void StopBackground()
    {
        foreach (Background background in backgrounds)
        {
            background.StopMovement();
        }
    }
    
    // 배경 스크롤 일시정지 해제
    public void ResumeBackground()
    {
        foreach (Background background in backgrounds)
        {
            background.ResumeMovement();
        }
    }
}