using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float power;         // 총알 파워
    public float maxShotDelay;  // 최대 총알 발사 딜레이
    public float curShotDelay;  // 현재 총알 발사 딜레이
    public float attackRange;   // 공격 범위
    public float speed;         // 이동 속도 추가

    public bool isTouchTop;     // 위쪽 벽과 닿았는지 여부
    public bool isTouchBottom;  // 아래쪽 벽과 닿았는지 여부
    public bool isTouchRight;   // 오른쪽 벽과 닿았는지 여부
    public bool isTouchLeft;    // 왼쪽 벽과 닿았는지 여부

    public GameObject bulletObjA; // 총알 프리팹 A
    public GameObject bulletObjB; // 총알 프리팹 B

    private GameManager gameManager;    // 게임 매니저
    private bool isGamePaused = false;  // 게임 일시정지 여부
    private int enemyCollisionCount = 0; // 적과의 충돌 횟수 추적

    private bool isCollidingWithEnemy = false; // 적과 충돌 중인지 여부
    private float collisionDamageTimer = 0.5f; // 충돌 데미지 타이머

    public GameObject[] followerPrefabs; // Follower 프리팹 배열
    private List<GameObject> followers = new List<GameObject>(); // 생성된 Follower 리스트
    
    Animator anim;              // 애니메이터

    public GameObject hpBarPrefab; // HPbar 프리팹 참조
    private GameObject hpBarInstance; // 생성된 HPbar 인스턴스
    private Slider hpBarSlider; // HPbar Slider 컴포넌트
    private bool isHpBarVisible = false; // HPbar가 보이는지 여부
    public int maxHealth = 100; // 최대 체력
    public int health; // 현재 체력

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager 인스턴스 가져오기
        anim = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기

        // Follower 생성 및 초기화
        GameObject previousFollower = this.gameObject; // 첫 번째 Follower는 Player를 따라감
        for (int i = 0; i < followerPrefabs.Length; i++)
        {
            Vector3 spawnPosition = previousFollower.transform.position - new Vector3(0, (i + 1) * 1.0f, 0); // 각 Follower가 이전 Follower의 뒤에 생성되도록 위치 조정
            GameObject follower = Instantiate(followerPrefabs[i], spawnPosition, Quaternion.identity);
            follower.GetComponent<Follower>().target = previousFollower; // 이전 Follower를 따라가도록 설정
            followers.Add(follower);
            previousFollower = follower; // 다음 Follower는 현재 Follower를 따라감
        }

        // HPbar 생성 및 초기화
        hpBarInstance = Instantiate(hpBarPrefab, transform.position + Vector3.down * 1.0f, Quaternion.identity, transform);
        hpBarInstance.SetActive(true); // 초기에는 활성화

        // HPbar가 올바르게 설정되었는지 확인
        Canvas hpBarCanvas = hpBarInstance.GetComponentInChildren<Canvas>();
        if (hpBarCanvas != null)
        {
            hpBarCanvas.renderMode = RenderMode.WorldSpace;
        }
        else
        {
            Debug.LogError("HPbar 프리팹에 Canvas 컴포넌트가 없습니다.");
        }

        // HPbar Slider 컴포넌트 가져오기
        hpBarSlider = hpBarInstance.GetComponentInChildren<Slider>();
        if (hpBarSlider == null)
        {
            Debug.LogError("HPbar 프리팹에 Slider 컴포넌트가 없습니다.");
        }

        // 초기 체력 설정
        health = maxHealth;
        if (hpBarSlider != null)
        {
            health = maxHealth;
            hpBarSlider.minValue = 0.0f;
            hpBarSlider.maxValue = 1.0f;
            hpBarSlider.value = 1.0f; // 초기 값은 최대 체력 비율
        }
    }

    void Update()
    {
        Move();     // 이동 함수
        AutoFire(); // 자동 발사 함수
        Reload();   // 재장전 함수 (총알 발사 딜레이 설정)

        // HPbar 위치 업데이트
        if (hpBarInstance != null)
        {
            hpBarInstance.transform.position = transform.position + Vector3.down * 0.7f;
        }

        if (isCollidingWithEnemy)
        {
            collisionDamageTimer -= Time.deltaTime;
            if (collisionDamageTimer <= 0)
            {
                OnHit((int)(maxHealth * 0.1f)); // 체력의 10% 데미지
                collisionDamageTimer = 0.5f; // 타이머 초기화
            }
        }
    }
    
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if ((isTouchRight && h > 0) || (isTouchLeft && h < 0)) h = 0;
        if ((isTouchTop && v > 0) || (isTouchBottom && v < 0)) v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, 0, 0).normalized * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        // 매 프레임마다 애니메이터 파라미터 업데이트
        UpdateAnimatorParameters(h);
    }

    void UpdateAnimatorParameters(float h)
    {
        anim.SetInteger("Input", (int)h);
    }

    void AutoFire()
    {
        curShotDelay += Time.deltaTime;  // 현재 총알 발사 딜레이에 시간을 더함

        if (curShotDelay >= maxShotDelay)
        {
            Fire();  // 발사 함수 호출
            curShotDelay = 0;  // 현재 총알 발사 딜레이 초기화
        }
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        switch(power)   // 파워에 따라 총알 오브젝트를 다르게 생성
        {
            case 1: // 일반 공격
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);    // 총알 오브젝트 생성
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                 // 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                   // 위쪽으로 힘을 가함
                break;
            case 2: // 따블 공격
                GameObject bulletR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.15f, transform.rotation);    // 오른쪽 총알 오브젝트 생성
                GameObject bulletL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.15f, transform.rotation);     // 왼쪽 총알 오브젝트 생성
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();                                                        // 오른쪽 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();                                                        // 왼쪽 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                break;
            case 3: // 트리플 공격
                GameObject bulletRR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.3f, transform.rotation);    // 오른쪽 총알 오브젝트 생성
                GameObject bulletCC = Instantiate(bulletObjB, transform.position, transform.rotation);                            // 중앙 총알 오브젝트 생성
                GameObject bulletLL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.3f, transform.rotation);     // 왼쪽 총알 오브젝트 생성
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();                                                        // 오른쪽 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();                                                        // 중앙 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();                                                        // 왼쪽 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                break;
            default:
                break;
        }
        curShotDelay = 0; // 현재 총알 발사 딜레이 초기화
    }


    void Reload()
    {
        curShotDelay += Time.deltaTime;  // 현재 총알 발사 딜레이에 시간을 더함
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // 충돌한 오브젝트의 태그가 Enemy이면
        {
            if (enemyCollisionCount == 0)
            {
                isGamePaused = true;
                gameManager.PauseSpawning(); // 적 스폰 일시정지
                StopAllEnemies(); // 모든 적의 움직임 정지
                StopAllBackgrounds(); // 모든 배경의 움직임 정지
                OnHit((int)(maxHealth * 0.1f)); // 최초 충돌 시 체력의 10% 데미지
                isCollidingWithEnemy = true; // 충돌 상태 설정
            }
            enemyCollisionCount++;
        }

        if (collision.gameObject.CompareTag("BossBullet")) // 충돌한 오브젝트의 태그가 BossBullet이면
        {
            BulletBoss bulletBoss = collision.gameObject.GetComponent<BulletBoss>(); // 충돌한 오브젝트의 BulletBoss 컴포넌트 가져오기
            if (bulletBoss != null)
            {
                OnHit(bulletBoss.dmg); // 피격 함수 호출
                Destroy(collision.gameObject); // 충돌한 오브젝트 삭제
            }
        }

        if (collision.gameObject.tag == "Border")    // Border 태그와 충돌 시
        {
            switch (collision.gameObject.name)       // 충돌한 오브젝트의 이름으로 분기
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // 충돌한 오브젝트의 태그가 Enemy이면
        {
            enemyCollisionCount--;
            if (enemyCollisionCount == 0)
            {
                isGamePaused = false;
                gameManager.ResumeSpawning(); // 적 스폰 재시작
                ResumeAllEnemies(); // 모든 적의 움직임 재시작
                isCollidingWithEnemy = false; // 충돌 상태 해제
                collisionDamageTimer = 0.5f; // 타이머 초기화
            }
        }

        if (collision.gameObject.tag == "Border")    // Border 태그와 충돌 시
        {
            switch (collision.gameObject.name)       // 충돌한 오브젝트의 이름으로 분기
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }

    public void OnHit(int dmg)
    {
        StartCoroutine(DecreaseHealthOverTime(dmg)); // 체력을 가변적으로 줄이는 코루틴 시작

        // HPbar 활성화
        if (!isHpBarVisible)
        {
            hpBarInstance.SetActive(true);
            isHpBarVisible = true;
        }
    }

    private IEnumerator DecreaseHealthOverTime(int dmg)
    {
        int damagePerTick = 1; // 한 틱당 감소할 데미지
        while (dmg > 0)
        {
            health -= damagePerTick;
            dmg -= damagePerTick;

            // HPbar Slider 값 업데이트
            if (hpBarSlider != null)
            {
                hpBarSlider.value = (float)health / maxHealth; // 체력 비율로 Slider 값 설정
            }

            // 체력이 0 이하이면 게임 오브젝트 삭제
            if (health <= 0)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return new WaitForSeconds(0.05f); // 0.05초마다 데미지 감소
        }
    }

    void StopAllEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>(); // 모든 적 오브젝트 가져오기
        foreach (Enemy enemy in enemies)
        {
            enemy.StopMovement(); // 모든 적의 움직임 정지
        }
    }

    void ResumeAllEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>(); // 모든 적 오브젝트 가져오기
        foreach (Enemy enemy in enemies)
        {
            enemy.ResumeMovement(); // 모든 적의 움직임 재시작
        }
    }

    void StopAllBackgrounds()
    {
        Background[] backgrounds = FindObjectsOfType<Background>(); // 모든 배경 오브젝트 가져오기
        foreach (Background background in backgrounds)
        {
            background.StopMovement(); // 모든 배경의 움직임 정지
        }
    }

    void ResumeAllBackgrounds()
    {
        Background[] backgrounds = FindObjectsOfType<Background>(); // 모든 배경 오브젝트 가져오기
        foreach (Background background in backgrounds)
        {
            background.ResumeMovement(); // 모든 배경의 움직임 재시작
        }
    }
}