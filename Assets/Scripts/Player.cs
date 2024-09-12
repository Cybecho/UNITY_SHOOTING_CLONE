using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;         // 이동 속도
    public float power;         // 총알 파워
    public float maxShotDelay;  // 최대 총알 발사 딜레이
    public float curShotDelay;  // 현재 총알 발사 딜레이
    public float attackRange;   // 공격 범위

    public bool isTouchTop;     // 위쪽 벽과 닿았는지 여부
    public bool isTouchBottom;  // 아래쪽 벽과 닿았는지 여부
    public bool isTouchRight;   // 오른쪽 벽과 닿았는지 여부
    public bool isTouchLeft;    // 왼쪽 벽과 닿았는지 여부

    public GameObject bulletObjA; // 총알 프리팹 A
    public GameObject bulletObjB; // 총알 프리팹 B

    private GameManager gameManager;    // 게임 매니저
    private bool isGamePaused = false;  // 게임 일시정지 여부
    private int enemyCollisionCount = 0; // 적과의 충돌 횟수 추적

    Animator anim;              // 애니메이터

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager 인스턴스 가져오기
        anim = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
    }

    void Update()
    {
        Move();     // 이동 함수
        //Fire();     // 발사 함수
        AutoFire(); // 자동 발사 함수
        Reload();   // 재장전 함수 (총알 발사 딜레이 설정)
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
        // 특정 y축 범위 내에 적이 있는지 확인
        foreach (GameObject enemy in gameManager.enemyObjs)
        {
            if (enemy != null && Mathf.Abs(enemy.transform.position.y - transform.position.y) < attackRange) // y축 범위 설정
            {
                Fire();
                break;
            }
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
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   
                break;
            default:
                break;
        }

        curShotDelay = 0;
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
            }
            enemyCollisionCount++;
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
}