using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 전환을 위해 추가

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab; // 플레이어 프리팹
    public Transform playerSpawnPoint; // 플레이어 스폰 지점

    public GameObject[] enemyObjs;      // 적 오브젝트 배열
    public Transform[] spawnPoints;     // 스폰 위치 배열
    public float spawnInterval;         // 스폰 주기 (초 단위)
    private Coroutine spawnCoroutine;   // 스폰 코루틴을 저장할 변수
    private bool isSpawningPaused = false; // 스폰 일시정지 여부
    private Background[] backgrounds;   // 배경 스크롤 스크립트 배열
    private int spawnCount = 0;         // 스폰 카운트 변수

    public Slider goalBarSlider;        // GoalBarSlider 참조
    public int goalCount;               // 목표 스폰 횟수

    private bool isSpawningEnabled = true; // 몬스터 소환 플래그
    private bool isBossMode = false;       // 보스모드 플래그
    private bool isGameCleared = false;    // 게임 클리어 플래그 추가

    public GameObject[] bossBulletPrefabs; // 보스 총알 프리팹 배열
    public float bossBulletSpawnInterval = 1f; // 보스 총알 스폰 주기

    public GameObject bossPrefab;       // 보스 프리팹
    public Transform bossSpawnPoint;    // 보스 스폰 지점
    public GameObject gameClearPrefab;  // 게임 클리어 프리팹 추가
    public Transform gameClearSpawnPoint; // 게임 클리어 스폰 지점 추가

    public string gameOverSceneName = "GameOver";
    public string gameClearSceneName = "GameClear";

    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnEnemiesRoutine()); // Coroutine 시작
        backgrounds = FindObjectsOfType<Background>(); // 모든 배경 스크롤 스크립트 가져오기

        // GoalBarSlider 초기화
        goalBarSlider.minValue = 0;
        goalBarSlider.maxValue = goalCount;
        goalBarSlider.value = 0;
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            if (!isSpawningPaused && isSpawningEnabled)
            {
                SpawnEnemies(); // 적 스폰 함수 호출
                spawnCount++; // 스폰 카운트 증가
                Debug.Log("적 스폰: " + spawnCount + "회");

                // GoalBarSlider 업데이트
                goalBarSlider.value = spawnCount;

                // GoalBarSlider가 가득 찼을 때 처리
                if (spawnCount >= goalCount)
                {
                    Debug.Log("GoalBarSlider가 가득 찼습니다!");
                    isSpawningEnabled = false; // 몬스터 소환 중지
                    StartCoroutine(CheckAllEnemiesCleared()); // 모든 몬스터가 제거되었는지 확인
                }
            }
            yield return new WaitForSeconds(spawnInterval); // 스폰 주기만큼 대기
        }
    }

    IEnumerator CheckAllEnemiesCleared()
    {
        while (true)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
            {
                isBossMode = true; // 보스모드 플래그 설정
                Debug.Log("모든 몬스터가 제거되었습니다. 보스모드 시작!");
                SpawnBoss(); // 보스 생성
                StartCoroutine(SpawnBossBullets()); // 보스 총알 스폰 코루틴 시작
                break;
            }
            yield return new WaitForSeconds(1f); // 1초마다 확인
        }
    }

    void SpawnBoss()
    {
        Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation); // 보스 생성
        Debug.Log("보스가 생성되었습니다!");
    }

    IEnumerator SpawnBossBullets()
    {
        yield return new WaitForSeconds(2f); // 2초 대기

        while (isBossMode)
        {
            int ranPoint = Random.Range(0, spawnPoints.Length); // 랜덤 스폰 위치 인덱스
            int ranBullet = Random.Range(0, bossBulletPrefabs.Length); // 랜덤 보스 총알 인덱스
            Instantiate(bossBulletPrefabs[ranBullet], 
                        spawnPoints[ranPoint].position, 
                        spawnPoints[ranPoint].rotation); // 보스 총알 생성

            yield return new WaitForSeconds(bossBulletSpawnInterval); // 보스 총알 스폰 주기만큼 대기
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

    // 보스 사망 처리 함수 수정
    public void OnBossDeath()
    {
        isGameCleared = true; // 게임 클리어 플래그 활성화
        Debug.Log("게임 클리어!");
        Instantiate(gameClearPrefab, gameClearSpawnPoint.position, gameClearSpawnPoint.rotation); // 게임 클리어 프리팹 스폰
    
        // 4초 후 초기 상태로 돌아가는 코루틴 시작
        StartCoroutine(ResetGameAfterClear());
    }


    // 게임 클리어 후 초기 상태로 돌아가는 코루틴 추가
    private IEnumerator ResetGameAfterClear()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(gameClearSceneName); // 게임 클리어 씬으로 전환
    }
    
    public void OnPlayerDeath()
    {
        // 모든 스폰된 객체 삭제
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        GameObject[] bossBullets = GameObject.FindGameObjectsWithTag("BossBullet");
        foreach (GameObject bullet in bossBullets)
        {
            Destroy(bullet);
        }

        // GameOver 씬으로 전환
        SceneManager.LoadScene(gameOverSceneName);
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1f); // 잠시 대기

        // 게임 상태 초기화
        isGameCleared = false;
        isBossMode = false;
        isSpawningEnabled = true;
        spawnCount = 0;
        goalBarSlider.value = 0;

        // 배경 스크롤 재시작
        ResumeBackground();

        // 적 스폰 재시작
        spawnCoroutine = StartCoroutine(SpawnEnemiesRoutine());
    }

    // 게임오버를 처리하는 메서드
    public void GameOver()
    {
        Debug.Log("게임 오버!");
        SceneManager.LoadScene(gameOverSceneName); // 게임 오버 씬으로 전환
    }

    // 게임클리어를 처리하는 메서드
    public void GameClear()
    {
        Debug.Log("Game Clear 메서드 호출됨"); // 디버그 로그 추가
        // 게임클리어 씬으로 전환
        SceneManager.LoadScene(gameClearSceneName);
    }
}