using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Slider를 사용하기 위해 추가

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Normal, Boss }
    public EnemyType enemyType; // 적의 타입

    public float speed; // 이동 속도
    public int maxHealth; // 최대 체력
    public int health; // 현재 체력
    public float dmg; // 데미지
    public Sprite[] sprites; // 스프라이트 배열
    private Rigidbody2D rigid; // 리지드바디 컴포넌트
    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러 컴포넌트
    private Vector2 savedVelocity; // 정지 전 속도를 저장할 변수
    private bool isPaused = false; // 일시정지 여부
    private GameManager gameManager;

    public GameObject normalHpBarPrefab; // 일반 Enemy HPbar 프리팹 참조
    public GameObject bossHpBarPrefab; // Boss HPbar 프리팹 참조
    private GameObject hpBarInstance; // 생성된 HPbar 인스턴스
    private Slider hpBarSlider; // HPbar Slider 컴포넌트
    private bool isHpBarVisible = false; // HPbar가 보이는지 여부
    private bool isBoss => enemyType == EnemyType.Boss; // 적이 보스인지 여부

    public GameObject deathAnimationPrefab; // 죽을 때 애니메이션 프리팹

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>(); // 리지드바디 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 컴포넌트 가져오기
        rigid.velocity = Vector2.down * speed; // 아래쪽으로 이동
        savedVelocity = rigid.velocity; // 초기 속도 저장
        gameManager = FindObjectOfType<GameManager>();

        // HPbar 생성 및 초기화
        if (isBoss)
        {
            hpBarInstance = Instantiate(bossHpBarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            hpBarInstance = Instantiate(normalHpBarPrefab, transform.position + Vector3.up * 1.0f, Quaternion.identity, transform);
        }
        hpBarInstance.SetActive(false); // 초기에는 비활성화

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

        // 초기 체력 설정 (랜덤으로 플러스 마이너스 30%)
        float randomFactor = Random.Range(0.7f, 1.3f);
        health = Mathf.RoundToInt(maxHealth * randomFactor);
        if (hpBarSlider != null)
        {
            hpBarSlider.value = (float)health / maxHealth; // 초기 체력 비율로 Slider 값 설정
        }
    }

    void Update()
    {
        if (isPaused)
        {
            return;
        }

        // 적의 움직임 로직 추가
        MoveChildren();

        // HPbar 위치 업데이트
        if (hpBarInstance != null && !isBoss)
        {
            hpBarInstance.transform.position = transform.position + Vector3.up * 1.0f;
        }
    }

    public void OnHit(int dmg)
    {
        health -= dmg; // 체력 감소
        spriteRenderer.sprite = sprites[1]; // 피격당했을때 스프라이트 변경
        Invoke("ReturnSprite", 0.1f); // 0.1초 후 원래 스프라이트로 변경

        // HPbar 활성화
        if (!isHpBarVisible)
        {
            hpBarInstance.SetActive(true);
            isHpBarVisible = true;
        }

        // HPbar Slider 값 업데이트
        if (hpBarSlider != null)
        {
            hpBarSlider.value = (float)health / maxHealth; // 체력 비율로 Slider 값 설정
        }

        if (health <= 0) // 체력이 0 이하이면
        {
            // 충돌 처리 해제
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // DeadEnemyController의 isDead 활성화
            DeadEnemyController deadEnemyController = GetComponent<DeadEnemyController>();
            if (deadEnemyController != null)
            {
                deadEnemyController.isDead = true;
            }

            if (hpBarInstance != null)
            {
                hpBarInstance.SetActive(false);
            }

            StartCoroutine(PlayDeathAnimationAndDestroy()); // 죽는 애니메이션 재생 후 삭제
        }
    }

    private IEnumerator PlayDeathAnimationAndDestroy()
    {
        // 죽을 때 애니메이션 생성
        GameObject deathAnimation = Instantiate(deathAnimationPrefab, transform.position, Quaternion.identity);
        Animator animator = deathAnimation.GetComponent<Animator>();

        // 부모 오브젝트의 속도를 가져옴
        Rigidbody2D parentRigidbody = GetComponent<Rigidbody2D>();
        if (parentRigidbody != null)
        {
            Rigidbody2D deathAnimationRigidbody = deathAnimation.AddComponent<Rigidbody2D>();
            deathAnimationRigidbody.velocity = parentRigidbody.velocity;

            // 중력 영향을 받지 않도록 설정
            deathAnimationRigidbody.gravityScale = 0;

            // 하드코딩으로 속도를 조정 (예: 부모 속도의 0.5배)
            deathAnimationRigidbody.velocity = new Vector2(parentRigidbody.velocity.x, parentRigidbody.velocity.y * 0.5f);
        }

        // 애니메이션 재생 속도를 2배로 설정
        if (animator != null)
        {
            animator.speed = 2.0f; // 애니메이션 재생 속도를 2배로 설정
            animator.Play(0, -1, 0);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2); // 애니메이션 길이를 절반으로 줄임
        }

        // 애니메이션 오브젝트 삭제
        Destroy(deathAnimation);

        // 적 오브젝트 삭제
        Destroy(gameObject);
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0]; // 원래 스프라이트로 변경
    }

    void MoveChildren()
    {
        foreach (Transform child in transform)
        {
            child.position += (Vector3)(Vector2.down * speed * Time.deltaTime);
        }
    }

    public void StopMovement()
    {
        isPaused = true;
        savedVelocity = rigid.velocity;
        rigid.velocity = Vector2.zero;
        gameManager.StopBackground();
    }

    public void ResumeMovement()
    {
        isPaused = false;
        rigid.velocity = savedVelocity;
        gameManager.ResumeBackground();
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BorderBullet")) // 충돌한 오브젝트의 태그가 BorderBullet이면
        {
            Destroy(gameObject); // 게임 오브젝트 삭제
        }
        else if (collision.gameObject.CompareTag("PlayerBullet")) // 충돌한 오브젝트의 태그가 PlayerBullet이면
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>(); // 충돌한 오브젝트의 Bullet 컴포넌트 가져오기
            OnHit(bullet.dmg); // 피격 함수 호출
            Destroy(collision.gameObject); // 충돌한 오브젝트 삭제
        }
    }
}